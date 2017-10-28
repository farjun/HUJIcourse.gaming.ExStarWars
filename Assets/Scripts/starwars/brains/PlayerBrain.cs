using UnityEngine;
using StarWars.Actions;

namespace StarWars.Brains {
/// <summary>
/// Controls:
/// Right/Left/A/D to turn.
/// Space/W to shoot.
/// Hold Shift/S to keep shield up, release to drop the shield.
/// </summary>
public class PlayerBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "Player";
        }
    }
    public override Color PrimaryColor {
        get {
            return Color.white;
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.XWing;
        }
    }

    public override Action NextAction() {
        var wantsShield = ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.S)));
        if (wantsShield && spaceship.CanRaiseShield) {
            return ShieldUp.action;
        }
        if (!wantsShield && spaceship.IsShieldUp) {
            return ShieldDown.action;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            return TurnRight.action;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            return TurnLeft.action;
        }
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && spaceship.CanShoot) {
            return Shoot.action;
        }
        return DoNothing.action;
    }
}
}