using UnityEngine;
using Infra;
using StarWars.Actions;
using StarWars.Brains;

namespace StarWars {
/// <summary>
/// A read only spaceship. Can't be changed.
/// </summary>
public class Spaceship : SpaceObject {
    public const int INITIAL_HEALTH = 100;
    public const int INITIAL_ENERGY = 400;
    public const int SHOT_COOLDOWN = 30;
    public const int RESPAWN_COOLDOWN = 50;
    public const float ROTATION_PER_ACTION = 5f;
    public const float SPEED_PER_TURN = 0.1f;
    public const int SHIELD_UP_ENERGY_COST = 100;
    public const int SHIELD_ENERGY_COST_PER_TURN = 3;
    public const int ENERGY_REPLENISH_PER_TURN = 4;
    private SpaceshipBrain brain;
    private int health;
    private int energy;
    private int shotCooldown;
    private int turnsToRespawn;
    private bool isShieldUp;

    public override float Radius {
        get {
            return 0.6f;
        }
    }

    /// <summary>
    /// Trying to shoot during the cooldown will explode the spaceship instantly.
    /// </summary>
    public bool CanShoot {
        get {
            return IsAlive && shotCooldown <= 0;
        }
    }

    /// <summary>
    /// Need at least enough energy to raise the shield and leave it on for 5 turns.
    /// </summary>
    public bool CanRaiseShield {
        get {
            return IsAlive && !IsShieldUp && Energy > SHIELD_UP_ENERGY_COST + SHIELD_ENERGY_COST_PER_TURN * 5;
        }
    }

    public bool IsShieldUp {
        get {
            return isShieldUp;
        }
    }

    public int Health {
        get {
            return health;
        }
    }

    public int Energy {
        get {
            return energy;
        }
    }

    public override bool IsAlive {
        get {
            return health > 0;
        }
    }

    public override string Name {
        get {
            return brain.Name;
        }
    }

    public Spaceship() {
        // Empty on purpose.
    }

    private void Reset(SpaceshipBrain brain) {
        this.brain = brain;
        this.brain.Activate(this);
        health = INITIAL_HEALTH;
        energy = INITIAL_ENERGY;
        isShieldUp = false;
        shotCooldown = 0;
        turnsToRespawn = 0;
    }

    /// <summary>
    /// Allows mutating a spaceship.
    /// Doesn't inherit from Spaceship to not allow casting a Spaceship to a
    /// Spaceship.Mutable.
    /// </summary>
    public class Mutable : SpaceObject.Mutable<Spaceship> {
        public int ShotCooldown {
            get {
                return obj.shotCooldown;
            }
            set {
                obj.shotCooldown = value;
            }
        }

        public bool CanShoot {
            get {
                return obj.CanShoot;
            }
        }

        public bool CanRaiseShield {
            get {
                return obj.CanRaiseShield;
            }
        }

        public bool IsShieldUp {
            get {
                return obj.isShieldUp;
            }
        }

        public int Health {
            get {
                return obj.health;
            }
            set {
                obj.health = value;
            }
        }

        public int Energy {
            get {
                return obj.energy;
            }
            set {
                obj.energy = value;
            }
        }

        public SpaceshipBrain Brain {
            get {
                return obj.brain;
            }
        }

        private Action nextAction;

        public Mutable() {
            // Empty on purpose.
        }

        public void Activate(Spaceship spaceship, SpaceshipBody body, SpaceshipBrain brain) {
            base.Activate(spaceship, body);
            this.obj.Reset(brain);
            var shipBody = (SpaceshipBody)obj.body;
            var main = shipBody.energyEmitter.main;
            main.maxParticles = 20;
            ShieldDown();
        }

        public override void Deactivate() {
            obj.brain.gameObject.SetActive(false);
            obj.brain = null;
            base.Deactivate();
        }

        public override void BeDead() {
            base.BeDead();
            obj.health = 0;
            obj.turnsToRespawn = RESPAWN_COOLDOWN;
        }

        public override void Respawn() {
            DebugUtils.Log("Respawn! " + Name);
            base.Respawn();
            obj.health = INITIAL_HEALTH;
            obj.energy = INITIAL_ENERGY;
            var shipBody = (SpaceshipBody)obj.body;
            var main = shipBody.energyEmitter.main;
            main.maxParticles = 20;
            ShieldDown();
            obj.shotCooldown = 0;
            obj.turnsToRespawn = 0;
            Position = Space.GetSpawnPoint();
            Rotation = Random.value * 360;
            Space.RespawnedSpaceship(this);
            Game.RespawnedSpaceship(this);
        }

        public void ShieldUp() {
            Energy -= Spaceship.SHIELD_UP_ENERGY_COST;
            var shipBody = (SpaceshipBody)obj.body;
            shipBody.shieldImage.gameObject.SetActive(true);
            obj.isShieldUp = true;
            Game.OnShieldUp();
        }

        public void ShieldDown() {
            var shipBody = (SpaceshipBody)obj.body;
            shipBody.shieldImage.gameObject.SetActive(false);
            obj.isShieldUp = false;
        }

        public void SelectAction() {
            try {
                nextAction = obj.brain.NextAction();
            } catch (System.Exception e) {
                // Next action threw exception!
                DebugUtils.LogError("NextAction of " + Name + " threw exception!\n" + e);
                BeDead();
            }
        }

        public override void DoTurn() {
            if (IsAlive) {
                if (nextAction.CanDo(obj)) {
                    nextAction.Do(this);

                    obj.body.MoveForward(SPEED_PER_TURN);
                    var shipBody = (SpaceshipBody)obj.body;
                    var main = shipBody.energyEmitter.main;
                    main.maxParticles = 20 * Energy / INITIAL_ENERGY;

                    if (obj.shotCooldown > 0) {
                        --obj.shotCooldown;
                    }
                    if (IsShieldUp) {
                        Energy -= SHIELD_ENERGY_COST_PER_TURN;
                        if (Energy < 0) {
                            ShieldDown();
                            Energy = 0;
                        }
                    } else {
                        Energy += ENERGY_REPLENISH_PER_TURN;
                        if (Energy > INITIAL_ENERGY) {
                            Energy = INITIAL_ENERGY;
                        }
                    }
                } else {
                    // Illegal move!
                    DebugUtils.LogError("Illegal move for spaceship " + Name);
                    BeDead();
                }
            } else {
                --obj.turnsToRespawn;
                if (obj.turnsToRespawn <= 0) {
                    // Respawn.
                    Respawn();
                }
            }
        }
    }
}
}
