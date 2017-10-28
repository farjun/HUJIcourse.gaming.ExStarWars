using UnityEngine;
using StarWars.Actions;
using Infra.Utils;

namespace StarWars.Brains {
public class HunterBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "Hunter";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0x76 / 0xFF, (float)0x35 / 0xFF, (float)0x35 / 0xFF, 1f);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.XWing;
        }
    }

    private Spaceship target = null;

    /// <summary>
    /// The hunter selects his target and follows it until it dies and tries to
    /// shoot it when it can.
    /// </summary>
    public override Action NextAction() {
        // Find a target.
        if (target == null || !target.IsAlive) {
            foreach (var ship in Space.Spaceships) {
                // Make sure not to target self or dead spaceships.
                if (spaceship != ship && ship.IsAlive) {
                    target = ship;
                    break;
                }
            }
        }

        if (target != null) {
            // Hunt!
            var pos = spaceship.ClosestRelativePosition(target);
            var forwardVector = spaceship.Forward;
            var angle = pos.GetAngle(forwardVector);
            if (angle >= 10) return TurnLeft.action;
            if (angle <= -10) return TurnRight.action;
        }

        return spaceship.CanShoot ? Shoot.action : DoNothing.action;

    }
}
}