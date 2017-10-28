using UnityEngine;
using System;
using System.Collections.Generic;
using Infra.Collections;
using Infra.Utils;
using Infra;
using Infra.Audio;
using StarWars.UI;

namespace StarWars {
public class Game : MonoBehaviour {
    public const int SCORE_FOR_BASHING = 1;
    public const int SCORE_FOR_SHOOTING = 2;
    public const int DEATH_SCORE_PENALTY = -1;
    private readonly Space.Mutable space = new Space.Mutable();

    [Serializable]
    public class BodyTypeToSprite {
        public SpaceshipBody.Type bodyType;
        public Sprite primaryImage;
        public Sprite secondaryImage;
        public AdjustableAudioClip shotSound;
    }

    [SerializeField] GameObjectPool shotBodiesPool;
    [SerializeField] GameObjectPool explosionEffectsPool;
    [SerializeField] GameObjectPool respawnEffectsPool;
    [SerializeField] ScoreBoard scoreBoard;

    [SerializeField] AdjustableAudioClip killSound;
    [SerializeField] AdjustableAudioClip collisionSound;
    [SerializeField] AdjustableAudioClip shieldBurnShipSound;
    [SerializeField] AdjustableAudioClip shieldBurnShotSound;
    [SerializeField] AdjustableAudioClip shieldUpSound;

    [SerializeField] BodyTypeToSprite[] _spaceshipImages;
    public static Dictionary<SpaceshipBody.Type, BodyTypeToSprite> spaceshipImages =
        new Dictionary<SpaceshipBody.Type, BodyTypeToSprite>();

    [SerializeField] Color[] secondaryColors;

    private readonly HashSet<Spaceship.Mutable> deadShips = new HashSet<Spaceship.Mutable>();
    private readonly HashSet<Shot.Mutable> deadShots = new HashSet<Shot.Mutable>();

    [SerializeField] int deathsPerShip = 10;
    private int deaths;

    private static Game instance;

    public static Vector2 Size {
        get {
            var res = UIUtils.ScreenResolution;
            return Camera.main.ScreenToWorldPoint(new Vector3(res.width, res.height)) * 2f;
        }
    }

    protected void Awake() {
        instance = this;

        foreach (var entry in _spaceshipImages) {
            spaceshipImages[entry.bodyType] = entry;
        }
    }
    
    protected void Start() {
        // Create all spaceships.
        var secondaryColorIndex = 0;
        var type2colors = new Dictionary<SpaceshipBody.Type, HashSet<Color>>();
        var nameCount = new Dictionary<string, int>();
        var spaceships = FindObjectsOfType<SpaceshipBody>();

        DebugUtils.Assert(0 < spaceships.Length, "Define spaceship names at 'Game'");
        DebugUtils.Assert(spaceships.Length <= 6, "Max 6 spaceships allowed!");

        foreach (var body in spaceships) {
            var spawnPoint = Space.GetSpawnPoint();
            var angle = UnityEngine.Random.Range(1, 360 / Spaceship.ROTATION_PER_ACTION) * Spaceship.ROTATION_PER_ACTION;
            body.Activate(spawnPoint, angle, space);

            // Check for duplicate type and color.
            var bodyType = body.spaceshipType;
            // Further normaliziation of color to distinguish between similar colors.
            var color = SpaceshipBody.NormalizeColor(body.primaryColor, 0x20);
            if (!type2colors.ContainsKey(bodyType)) {
                type2colors[bodyType] = new HashSet<Color>();
                type2colors[bodyType].Add(color);
            } else if (type2colors[bodyType].Contains(color)) {
                // Color clash! Set secondary color.
                body.SetSecondaryColor(secondaryColors[secondaryColorIndex++]);
            } else {
                type2colors[bodyType].Add(color);
            }

            var brain = body.Brain;
            if (nameCount.ContainsKey(brain.DefaultName)) {
                ++nameCount[brain.DefaultName];
                brain.Name = brain.DefaultName + " " + nameCount[brain.DefaultName];
            } else {
                nameCount[brain.DefaultName] = 1;
            }

            // Register to score board.
            scoreBoard.Add(brain.Name, color, body.secondaryColor);
        }

        deaths = 0;
    }

    protected void Update() {
        if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) {
            if (Time.timeScale > 0.5f) {
                Time.timeScale = 0.5f;
            } else if (Time.timeScale > 0.2f) {
                Time.timeScale *= 0.4f;
            } else {
                Time.timeScale = 0.1f;
            }
            DebugUtils.LogError("Time Scale: " + Time.timeScale);
        }
        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus)
            || Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.KeypadEquals)) {
            if (Time.timeScale < 0.5f) {
                Time.timeScale *= 2.5f;
            } else {
                Time.timeScale = 1f;
            }
            DebugUtils.LogError("Time Scale: " + Time.timeScale);
        }
    }

    protected void FixedUpdate() {
        var spaceships = space.Spaceships;
        // First all alive spaceships select their next action.
        foreach (var spaceship in spaceships) {
            if (spaceship.IsAlive) {
                spaceship.SelectAction();
            }
        }
        // Then all spaceships do their turn.
        foreach (var spaceship in spaceships) {
            spaceship.DoTurn();
        }

        var shots = space.Shots;
        foreach (var shot in shots) {
            if (shot.IsAlive) {
                shot.DoTurn();
            }
        }

        deadShips.Clear();
        deadShots.Clear();
        // Check for collisions.
        // Spaceship to spaceship.
        for (int i = 0; i < spaceships.Count - 1; i++) {
            var ship = spaceships[i];
            if (!ship.IsAlive) continue;
            for (int j = i + 1; j < spaceships.Count; j++) {
                var other = spaceships[j];
                if (!other.IsAlive) continue;
                if (ship.obj.CheckCollision(other.obj)) {
                    DebugUtils.Log(ship.Name + " collided with ship " + other.Name);
                    if (ship.IsShieldUp != other.IsShieldUp) {
                        // The ship without the shield is dead.
                        var killer = ship.IsShieldUp ? ship : other;
                        var dead = ship.IsShieldUp ? other : ship;
                        scoreBoard.AddScore(killer.Name, SCORE_FOR_BASHING);
                        SpaceshipKilled(dead);
                        AudioManager.PlayClip(shieldBurnShipSound);
                        if (ship == dead) break;
                    } else {
                        SpaceshipKilled(other);
                        SpaceshipKilled(ship);
                        AudioManager.PlayClip(collisionSound);
                        break;
                    }
                }
            }
        }
        // Shot to spaceship.
        foreach (var ship in spaceships) {
            if (!ship.IsAlive) continue;
            foreach (var shot in shots) {
                if (!shot.IsAlive) continue;
                if (shot.obj.CheckCollision(ship.obj)) {
                    DebugUtils.Log(ship.Name + " collided with shot " + shot.Name);
                    if (!ship.IsShieldUp) {
                        scoreBoard.AddScore(shot.Shooter.Name, SCORE_FOR_SHOOTING);
                        SpaceshipKilled(ship);
                        AudioManager.PlayClip(killSound);
                    } else {
                        AudioManager.PlayClip(shieldBurnShotSound);
                    }
                    shot.BeDead();
                    break;
                }
            }
        }

        // Clear dead ships.
        foreach (var spaceship in spaceships) {
            if (!spaceship.IsAlive) {
                deadShips.Add(spaceship);
            }
        }
        foreach (var spaceship in deadShips) {
            space.RemoveSpaceship(spaceship);
        }

        // Clear dead shots.
        foreach (var shot in shots) {
            if (!shot.IsAlive) {
                deadShots.Add(shot);
            }
        }
        foreach (var shot in deadShots) {
            space.RemoveShot(shot);
        }
    }

    public static void SpawnShot(Spaceship.Mutable shooter) {
        instance.shotBodiesPool.Borrow<ShotBody>(shooter.Position, shooter.Rotation, instance.space, shooter);
        AudioManager.PlayClip(spaceshipImages[shooter.Brain.BodyType].shotSound);
    }

    public static void OnShieldUp() {
        AudioManager.PlayClip(instance.shieldUpSound);
    }

    public static void RespawnedSpaceship(Spaceship.Mutable spaceship) {
        instance.respawnEffectsPool.Borrow<IPoolable>(spaceship.Position);
    }

    private void SpaceshipKilled(Spaceship.Mutable spaceship) {
        scoreBoard.AddScore(spaceship.Name, DEATH_SCORE_PENALTY);
        spaceship.BeDead();
        instance.explosionEffectsPool.Borrow<IPoolable>(spaceship.Position);
        ++deaths;
        if (deaths == space.Spaceships.Count * deathsPerShip) {
            Debug.Break();
        }
    }
}
}
