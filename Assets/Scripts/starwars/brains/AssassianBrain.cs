using System;
using System.IO;
using UnityEngine;
using Infra.Utils;
using StarWars.Actions;
using Action = StarWars.Actions.Action;

namespace StarWars.Brains
{
    public class AssassianBrain : SpaceshipBrain
    {
        private string _defaultName = "Assassian_FlafelTeam";
        private Color _primaryColor = Color.white;
        private SpaceshipBody.Type _bodyType = SpaceshipBody.Type.TieFighter;


        public override string DefaultName
        {
            get { return _defaultName; }
        }

        public override Color PrimaryColor
        {
            get { return _primaryColor; }
        }

        public override SpaceshipBody.Type BodyType
        {
            get { return _bodyType; }
        }

        private const float SafeDistance = 3f;
        private const float MyShotLimit = 30f;

        public override Action NextAction()
        {
            var shipDistance = GetClosestShipDistance();
            var shotDistance = GetClosestShotDistance();
            // First Shield
            if ((shotDistance < SafeDistance || shipDistance < SafeDistance) && spaceship.CanRaiseShield)
            {
                return ShieldUp.action;
            }
            if (spaceship.IsShieldUp)
            {
                return ShieldDown.action;
            }
            // Then Kill
            if (spaceship.CanShoot)
            {
                return Shoot.action;
            }
            return GoCloserToShip();
        }

        private Action GoCloserToShip()
        {
            float shipDistance = float.MaxValue;
            Spaceship closestShip = null;
            foreach (Spaceship ship in Space.Spaceships)
            {
                if (!ship.IsAlive || ship == spaceship)
                {
                    continue;
                }
                var distance = spaceship.ClosestRelativePosition(ship).magnitude;
                if (!(distance < shipDistance)) continue;
                shipDistance = distance;
                closestShip = ship;
            }

            if (closestShip == null) return DoNothing.action;
            var angle = spaceship.ClosestRelativePosition(closestShip).GetAngle(spaceship.Forward);
            return angle >= 10 ? TurnLeft.action : TurnRight.action;
        }

        private float GetClosestShipDistance()
        {
            float shipDistance = float.MaxValue;
            foreach (Spaceship ship in Space.Spaceships)
            {
                if (!ship.IsAlive || ship == spaceship)
                {
                    continue;
                }
                var distance = spaceship.ClosestRelativePosition(ship).magnitude;
                if (!(distance < shipDistance)) continue;
                shipDistance = distance;
            }
            return shipDistance;
        }

        private float GetClosestShotDistance()
        {
            float shotDistance = float.MaxValue;
            foreach (Shot shot in Space.Shots)
            {
                if (!shot.IsAlive || ShotIsMine(shot))
                {
                    continue;
                }
                var distance = spaceship.ClosestRelativePosition(shot).magnitude;
                if (!(distance < shotDistance)) continue;
                shotDistance = distance;
            }
            return shotDistance;
        }

        private bool ShotIsMine(Shot shot)
        {
            var angleToShot = shot.Forward.GetAngle(spaceship.Forward);
            return Math.Abs(angleToShot) < MyShotLimit;
        }
    }
}