using UnityEngine;

namespace StarWars {
/// <summary>
/// A read only space object. Can't be changed.
/// </summary>
public abstract class SpaceObject {
    public abstract bool IsAlive { get; }
    /// <summary>
    /// Radius used to calculate collisions.
    /// </summary>
    public abstract float Radius { get; }

    protected Body body;

    public float Rotation {
        get {
            return IsAlive ? body.Rotation : 0;
        }
    }

    public Vector2 Position {
        get {
            return IsAlive ? body.Position : Vector2.zero;
        }
    }

    /// <summary>
    /// Forward vector of the space object.
    /// </summary>
    public Vector2 Forward {
        get {
            return IsAlive ? body.Forward : Vector2.zero;
        }
    }

    /// <summary>
    /// Checks if this space object is colliding with another space object.
    /// Accounting for the fact that the space is cyclic around the sides.
    /// When in space, any collision usually means instant death.
    /// </summary>
    public virtual bool CheckCollision(SpaceObject other) {
        var posDiff = other.Position - Position;
        var radius = Radius + other.Radius;
        // Check for collisions that wrap over the edge of the world.
        var diffSize = Game.Size;
        diffSize.x -= radius;
        diffSize.y -= radius;
        bool wrapX = Mathf.Abs(posDiff.x) >= diffSize.x;
        bool wrapY = Mathf.Abs(posDiff.y) >= diffSize.y;
        if (wrapX || wrapY) {
            // There might be a collision over the edge.
            var pos = Position;
            var otherPos = other.Position;
            if (wrapX) {
                if (posDiff.x < 0) {
                    // This object is to the right of the other.
                    pos.x -= Game.Size.x;
                } else {
                    otherPos.x -= Game.Size.x;
                }
            }
            if (wrapY) {
                if (posDiff.y < 0) {
                    // This object is above the other.
                    pos.y -= Game.Size.y;
                } else {
                    otherPos.y -= Game.Size.y;
                }
            }
            posDiff = otherPos - pos;
        }
        var sqrDistance = posDiff.sqrMagnitude;
        return sqrDistance <= radius * radius;
    }

    /// <summary>
    /// Returns the shortest vector from this space object to another, accounting
    /// for the fact that the space is cyclic around the sides.
    /// </summary>
    public Vector2 ClosestRelativePosition(SpaceObject other) {
        var posDiff = other.Position - Position;
        var diffSize = Game.Size / 2;
        bool wrapX = Mathf.Abs(posDiff.x) >= diffSize.x;
        bool wrapY = Mathf.Abs(posDiff.y) >= diffSize.y;
        if (!wrapX && !wrapY) return posDiff;

        // There might be a closer position over the edge.
        var otherPos = other.Position;
        if (wrapX) {
            if (posDiff.x < 0) {
                // This object is to the right of the other.
                otherPos.x += Game.Size.x;
            } else {
                otherPos.x -= Game.Size.x;
            }
        }
        if (wrapY) {
            if (posDiff.y < 0) {
                // This object is above the other.
                otherPos.y += Game.Size.y;
            } else {
                otherPos.y -= Game.Size.y;
            }
        }
        return otherPos - Position;
    }

    public virtual string Name {
        get {
            return body.name;
        }
    }

    /// <summary>
    /// Allows mutating a space object.
    /// </summary>
    public abstract class Mutable<T> where T : SpaceObject {
        public T obj;

        public bool IsAlive {
            get {
                return obj.IsAlive;
            }
        }

        public float Rotation {
            get {
                return obj.body.Rotation;
            }
            set {
                obj.body.Rotation = value;
            }
        }

        public Vector2 Position {
            get {
                return obj.body.Position;
            }
            set {
                obj.body.Position = value;
            }
        }

        public string Name {
            get {
                return obj.Name;
            }
        }

        public virtual void Activate(T obj, Body body) {
            this.obj = obj;
            this.obj.body = body;
        }

        public virtual void Deactivate() {
            obj.body.gameObject.SetActive(false);
            obj.body = null;
            obj = null;
        }

        public virtual void BeDead() {
            obj.body.gameObject.SetActive(false);
        }

        public virtual void Respawn() {
            obj.body.gameObject.SetActive(true);
        }

        public abstract void DoTurn();
    }
}
}
