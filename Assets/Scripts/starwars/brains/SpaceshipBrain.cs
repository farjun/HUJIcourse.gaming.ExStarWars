using UnityEngine;
using StarWars.Actions;

namespace StarWars.Brains {
public abstract class SpaceshipBrain : MonoBehaviour {
    protected Spaceship spaceship;

    public void Activate(Spaceship spaceship) {
        this.spaceship = spaceship;
        Name = DefaultName;
    }

    /// <summary>
    /// Reset is called when a component is added to a game object (also in the
    /// editor).
    /// </summary>
    protected void Reset() {
        gameObject.name = DefaultName + "Brain";
        var body = GetComponent<SpaceshipBody>();
        if (body != null) {
            body.primaryColor = PrimaryColor;
            if (body.primaryImage != null) {
                body.primaryImage.color = body.primaryColor;
            }
            body.spaceshipType = BodyType;
        }
        name = DefaultName + "Spaceship";
    }

    public string Name { get; set; }
    public abstract string DefaultName { get; }
    public abstract Color PrimaryColor { get; }
    public abstract SpaceshipBody.Type BodyType { get; }
    public abstract Action NextAction();
}
}