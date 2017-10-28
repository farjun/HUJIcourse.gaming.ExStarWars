using UnityEngine;
using StarWars.Actions;

namespace StarWars.Brains {
public class TwisterBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "Twister";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0xF9 / 0xFF, (float)0x6C / 0xFF, (float)0xC6 / 0xFF, 1f);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.TieFighter;
        }
    }

    public override Action NextAction() {
        return spaceship.CanShoot ? Shoot.action : TurnRight.action;
    }
}
}