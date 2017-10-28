using UnityEngine;
using StarWars.Actions;
using Infra.Utils;

namespace StarWars.Brains {
public class EvaderBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "Evader";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0x38 / 0xFF, (float)0x76 / 0xFF, (float)0x35 / 0xFF, 1f);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.TieFighter;
        }
    }

    private const float AHEAD_HALF_ANGLE = 45f;
    private const float BEHIND_HALF_ANGLE = 170f;
    private const float BLEED_FACTOR = 0.2f;

    private float maxThreatScore;
    private float shootingSqrDistance;

    private float ahead = 0f;
    private float behind = 0f;
    private float right = 0f;
    private float left = 0f;

    protected void Awake() {
        maxThreatScore = (Game.Size / 2).sqrMagnitude;
        shootingSqrDistance = Shot.SPEED_PER_TURN * (Shot.TURNS_TO_LIVE + 5);
        shootingSqrDistance *= shootingSqrDistance;
    }

    /// <summary>
    /// The evader calculates the threat of every ship and shot and divides the
    /// threats to sectors.
    /// Then it selects the safest sector and tries to move this direction.
    /// It also tries to shoot if a target is ahead and close enough.
    /// </summary>
    public override Action NextAction() {
        ahead = 0f;
        behind = 0f;
        right = 0f;
        left = 0f;
        var forwardVector = spaceship.Forward;
        bool shouldShoot = false;

        foreach (var threat in Space.Spaceships) {
            // Make sure not to run from self or dead spaceships.
            if (spaceship == threat || !threat.IsAlive) continue;
            var pos = spaceship.ClosestRelativePosition(threat);
            var angle = pos.GetAngle(forwardVector);
            shouldShoot |= CalculateThreat(pos, angle);
        }

        foreach (var threat in Space.Shots) {
            // Make sure not to run from dead shots.
            if (!threat.IsAlive) continue;
            var pos = spaceship.ClosestRelativePosition(threat);
            var angle = pos.GetAngle(forwardVector);
            CalculateThreat(pos, angle);
        }

        // Find safest sector.
        var min = Mathf.Min(ahead, behind, right, left);
        if (Mathf.Approximately(min, ahead)) {
            // Safest ahead - don't move.
            if (shouldShoot && spaceship.CanShoot) {
                return Shoot.action;
            }
            if (behind > maxThreatScore * 0.8f) {
                // Very dangerous behind - raise shield!
                if (spaceship.CanRaiseShield) {
                    return ShieldUp.action;
                }
            } else if (spaceship.IsShieldUp) {
                // Not so dangerous behind - lower shield.
                return ShieldDown.action;
            }
            return DoNothing.action;
        }
        // If safest behind, turn left or right - choose the safer side.
        // If safest left or right, turn to safer direction.
        return right < left ? TurnRight.action : TurnLeft.action;
    }

    private bool CalculateThreat(Vector2 pos, float angle) {
        // Add threat score to threat sector.
        var score = maxThreatScore - pos.sqrMagnitude;
        if (-AHEAD_HALF_ANGLE < angle && angle < AHEAD_HALF_ANGLE) {
            // Threat ahead.
            ahead += score;
            // Bleed some danger to neighboring sectors.
            if (angle < 0) {
                right += score * BLEED_FACTOR;
            } else {
                left += score * BLEED_FACTOR;
            }
            if (pos.sqrMagnitude <= shootingSqrDistance) {
                return true;
            }
        } else if (AHEAD_HALF_ANGLE <= angle && angle < BEHIND_HALF_ANGLE) {
            // Threat to the left.
            left += score;
            if (angle < 90f) {
                ahead += score * BLEED_FACTOR;
            } else {
                behind += score * BLEED_FACTOR;
            }
        } else if (-BEHIND_HALF_ANGLE < angle && angle <= -AHEAD_HALF_ANGLE) {
            // Threat to the right.
            right += score;
            ahead += score * BLEED_FACTOR;
            behind += score * BLEED_FACTOR;
            if (angle > -90f) {
                ahead += score * BLEED_FACTOR;
            } else {
                behind += score * BLEED_FACTOR;
            }
        } else {
            // Thread is behind.
            behind += score;
            if (angle < 0) {
                right += score * BLEED_FACTOR;
            } else {
                left += score * BLEED_FACTOR;
            }
        }
        return false;
    }
}
}