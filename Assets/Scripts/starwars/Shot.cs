using Infra;

namespace StarWars {
/// <summary>
/// A read only shot. Can't be changed.
/// </summary>
public class Shot : SpaceObject {
    public const int DAMAGE = 100;
    public const float SPEED_PER_TURN = 0.3f;
    public const int TURNS_TO_LIVE = 40;
    private int turnsToLive;
    private Spaceship.Mutable shooter;

    public override float Radius {
        get {
            return 0.5f;
        }
    }

    public int TurnsToLive {
        get {
            return turnsToLive;
        }
    }

    public override bool IsAlive {
        get {
            return turnsToLive > 0;
        }
    }

    public override bool CheckCollision(SpaceObject other) {
        if (other == shooter.obj) return false;
        return base.CheckCollision(other);
    }

    private void Reset(Spaceship.Mutable shooter) {
        turnsToLive = TURNS_TO_LIVE;
        this.shooter = shooter;
    }

    /// <summary>
    /// Allows mutating a shot.
    /// Doesn't inherit from Shot to not allow casting a Shot to a Shot.Mutable.
    /// </summary>
    public class Mutable : SpaceObject.Mutable<Shot> {
        public int TurnsToLive {
            get {
                return obj.turnsToLive;
            }
            set {
                obj.turnsToLive = value;
            }
        }

        public Spaceship.Mutable Shooter {
            get {
                return obj.shooter;
            }
        }

        public void Activate(Shot obj, Body body, Spaceship.Mutable shooter) {
            base.Activate(obj, body);
            this.obj.Reset(shooter);
            obj.body.MoveForward(SPEED_PER_TURN * 2);
        }

        public override void DoTurn() {
            if (!IsAlive) return;
            obj.body.MoveForward(SPEED_PER_TURN);
            --obj.turnsToLive;
        }

        public override void BeDead() {
            base.BeDead();
            obj.turnsToLive = 0;
        }
    }
}
}
