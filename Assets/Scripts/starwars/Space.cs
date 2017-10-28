using UnityEngine;
using System.Collections.Generic;
using Infra.Collections;
using StarWars.Brains;
using MIConvexHull;
using System.Linq;
using Infra;

namespace StarWars {
/// <summary>
/// Read only space. Can't be changed.
/// Holds all the spaceships and shots.
/// </summary>
public static class Space {

    private static ObjectPool<Spaceship> spaceshipsPool = new ObjectPool<Spaceship>(3, 1);
    private static ObjectPool<Spaceship.Mutable> spaceshipMutablesPool = new ObjectPool<Spaceship.Mutable>(3, 1);
    private static ObjectPool<Shot> shotsPool = new ObjectPool<Shot>(10, 5);
    private static ObjectPool<Shot.Mutable> shotMutablesPool = new ObjectPool<Shot.Mutable>(10, 5);

    private static List<Spaceship> spaceships = new List<Spaceship>();
    private static List<Spaceship.Mutable> spaceshipMutables = new List<Spaceship.Mutable>();
    private static List<Shot> shots = new List<Shot>();
    private static List<Shot.Mutable> shotMutables = new List<Shot.Mutable>();

    public static IList<Spaceship> Spaceships {
        get {
            return Space.spaceships;
        }
    }

    public static IList<Shot> Shots {
        get {
            return Space.shots;
        }
    }

    public static Vector2 GetSpawnPoint() {
        var vertices = new Vertex2[4 + spaceships.Count + shots.Count];
        var index = 0;
        var spaceHalfSize = Game.Size / 2;
        vertices[index++] = new Vertex2(-spaceHalfSize.x, -spaceHalfSize.y);
        vertices[index++] = new Vertex2(spaceHalfSize.x, -spaceHalfSize.y);
        vertices[index++] = new Vertex2(-spaceHalfSize.x, spaceHalfSize.y);
        vertices[index++] = new Vertex2(spaceHalfSize.x, spaceHalfSize.y);
        foreach (var obj in spaceships) {
            vertices[index++] = new Vertex2(obj.Position.x, obj.Position.y);
        }
        foreach (var obj in shots) {
            vertices[index++] = new Vertex2(obj.Position.x, obj.Position.y);
        }
        var triangulation = Triangulation.CreateDelaunay<Vertex2, Cell2>(vertices);
        Cell2 maxTriangle = triangulation.Cells.First();
        float maxArea = 0;
        foreach (var triangle in triangulation.Cells) {
            var area = triangle.Area;
            if (area > maxArea) {
                maxArea = area;
                maxTriangle = triangle;
            }
        }
        return maxTriangle.Centroid;
    }

    public static void RespawnedSpaceship(Spaceship.Mutable spaceship) {
        Space.spaceships.Add(spaceship.obj);
    }

    /// <summary>
    /// Allows mutating the space.
    /// Doesn't inherit from Space to not allow casting the Space to Space.Mutable.
    /// </summary>
    public class Mutable {
        public IList<Spaceship.Mutable> Spaceships {
            get {
                return Space.spaceshipMutables;
            }
        }

        public IList<Shot.Mutable> Shots {
            get {
                return Space.shotMutables;
            }
        }

        public void RegisterSpaceship(SpaceshipBody body, SpaceshipBrain brain) {
            var spaceship = Space.spaceshipsPool.Borrow();
            var spaceshipMutable = Space.spaceshipMutablesPool.Borrow();
            spaceshipMutable.Activate(spaceship, body, brain);
            DebugUtils.Log("RegisterSpaceship " + spaceship.Name);

            Space.spaceships.Add(spaceship);
            Space.spaceshipMutables.Add(spaceshipMutable);
        }

        public void RegisterShot(ShotBody body, Spaceship.Mutable shooter) {
            var shot = Space.shotsPool.Borrow();
            var shotMutable = Space.shotMutablesPool.Borrow();
            shotMutable.Activate(shot, body, shooter);

            Space.shots.Add(shot);
            Space.shotMutables.Add(shotMutable);
        }

        /// <summary>
        /// Only removes the spaceship from the list of active spaceships.
        /// </summary>
        public void RemoveSpaceship(Spaceship.Mutable spaceship) {
            Space.spaceships.Remove(spaceship.obj);
        }

        /// <summary>
        /// Completely removes a shot.
        /// </summary>
        public void RemoveShot(Shot.Mutable shot) {
            Space.shots.Remove(shot.obj);
            Space.shotMutables.Remove(shot);
            Space.shotsPool.Return(shot.obj);
            Space.shotMutablesPool.Return(shot);
            shot.Deactivate();
        }

        public void ClearSpace() {
            foreach (var item in spaceshipMutables) {
                item.Deactivate();
            }
            spaceshipsPool.ReturnAll();
            spaceshipMutablesPool.ReturnAll();
        }
    }
}
}
