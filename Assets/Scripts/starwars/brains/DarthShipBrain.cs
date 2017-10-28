// Author: Aviad & Ori
using UnityEngine;
using System.Collections.Generic;
using StarWars.Actions;
using Infra.Utils;

namespace StarWars.Brains {
public class DarthShipBrain : SpaceshipBrain {
    
    public override string DefaultName {
        get {
            return "DarthShip";
        }
    }
    
    public override Color PrimaryColor {
        get {
            return new Color((float)0xAA / 0xFF, (float)0xEE / 0xFF, (float)0xBB / 0xFF, 1f);
        }
    }
    
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.XWing;
        }
    }

    [SerializeField] float safeDistance = 3f;
    [SerializeField] float shieldUpTime = 20f;
    [SerializeField] float myShotLimit = 30f;
    [SerializeField] bool danger;
    
    private float timer;

    public override Action NextAction() {
        danger = IsBeingShotAt();
        if (danger) {
            if (spaceship.CanRaiseShield) {
                timer = shieldUpTime;
                return ShieldUp.action;
            } else if (spaceship.CanShoot) {
                return Shoot.action;
            } else {
                return TurnRight.action;
            }
        }
        if (spaceship.IsShieldUp) {
            --timer;
            if (timer <= 0) {
                return ShieldDown.action;
            }
        }
        var nearestShip = FindNearsetShip();
        if (nearestShip == null) {
            return DoNothing.action;
        }
        var pos = spaceship.ClosestRelativePosition(nearestShip);
        var forwardVector = spaceship.Forward;
        var angle = pos.GetAngle(forwardVector);
        if (angle >= 10f) {
            return TurnLeft.action;
        } else if (angle <= -10f) {
            return TurnRight.action;
        } else if (spaceship.CanShoot) {
            return Shoot.action;
        } else if (!spaceship.IsShieldUp && spaceship.CanRaiseShield && pos.magnitude <= safeDistance) {
            timer = shieldUpTime;
            return ShieldUp.action;
        } else {
            return TurnLeft.action;
        }

    }

    private Spaceship FindNearsetShip() {
        return FindNearest(Space.Spaceships, IsNotMe);
    }

    private bool IsBeingShotAt() {
        var nearestShot = FindNearest(Space.Shots, IsNotMyShot);
        if (nearestShot != null) {
            return spaceship.ClosestRelativePosition(nearestShot).magnitude <= safeDistance;
        }
        return false;
    }

    private T FindNearest<T>(IList<T> list, System.Func<T ,bool> predicate = null) where T : SpaceObject {
        T nearest = null;
        var minDistance = float.MaxValue;
        foreach (var obj in list) {
            if (!obj.IsAlive) continue;
            var distance = spaceship.ClosestRelativePosition(obj).magnitude;
            if ((nearest == null
                    || distance < minDistance)
                    && (predicate == null || predicate(obj))) {
                nearest = obj;
                minDistance = distance;
            }
        }
        return nearest;
    }

    private bool IsNotMyShot(Shot shot) {
        var angleToShot = shot.Forward.GetAngle(spaceship.Forward);
        return -myShotLimit > angleToShot || angleToShot > myShotLimit;
    }

    private bool IsNotMe(Spaceship ship) {
        return ship != spaceship;
    }
}
}
