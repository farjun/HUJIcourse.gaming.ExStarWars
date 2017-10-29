using UnityEngine;
using StarWars.Actions;
using Infra.Utils;
using Action = StarWars.Actions.Action;

namespace StarWars.Brains
{
	public class FalafelBrain : SpaceshipBrain
	{
		public override string DefaultName
		{
			get { return "Falafel"; }
		}

		public override Color PrimaryColor
		{
			get { return new Color((float) 0x40 / 0xFF, (float) 0x60 / 0xFF, (float) 0x240 / 0xFF, 1f); }
		}

		public override SpaceshipBody.Type BodyType
		{
			get { return SpaceshipBody.Type.TieFighter; }
		}

		private Spaceship target;
		private Spaceship chaser;
		private float distance;


		public void SearchShip()
		{
			// Find current target and chaser.
			distance = float.MaxValue;
			foreach (var ship in Space.Spaceships)
			{
				// Make sure not to target self or dead spaceships and choose the closest one.
				if (spaceship == ship || !ship.IsAlive || ship.Name.Contains("Assassian_FlafelTeam")) continue;

				var angle = ship.Forward.GetAngle(ship.ClosestRelativePosition(spaceship));

				if (spaceship.ClosestRelativePosition(ship).magnitude < distance)
				{
					target = ship;
					distance = spaceship.ClosestRelativePosition(ship).magnitude;
				}

				//if there is a ship chasing it
				if (angle < 12f && angle > -12f)
				{
					chaser = ship;
				}
			}
		}

		public Action getBetterAime(float angle)
		{
			if(angle <= -10f) return TurnRight.action;

			return TurnLeft.action;
		}

		/// <summary>
		/// If the Falafel feels attacked - it turns on the shield if it can when a ship is on his tail.
		/// he will find a ship that is closest to him and start chasing it
		/// </summary>
		public override Action NextAction()
		{
			
			if(target==null || !target.IsAlive)
				SearchShip();
			
			//if someone is chasing the ship and gets close we will raise the shield
			if (chaser != null && (!spaceship.IsShieldUp) && spaceship.ClosestRelativePosition(chaser).magnitude < 3f &&
			    spaceship.CanRaiseShield)
			{
				return ShieldUp.action;
			}
			
			
            // Try to kill it.
            var pos = spaceship.ClosestRelativePosition(target);
            var forwardVector = spaceship.Forward;
            var angle = pos.GetAngle(forwardVector);
			
            if (angle >= 10f) return TurnLeft.action;
            if (angle <= -10f) return TurnRight.action;
			
            if (distance < 20f && (!target.IsShieldUp))
            {
                return spaceship.CanShoot ? Shoot.action : DoNothing.action;
            }
			

			return DoNothing.action;
		}
	}
}