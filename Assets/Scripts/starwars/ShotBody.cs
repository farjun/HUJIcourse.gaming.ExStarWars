namespace StarWars {
/// <summary>
/// The visible body of the spaceship. Can move and rotate.
/// </summary>
public class ShotBody : PoolableBody {

    /// <summary>
    /// Expected params: (Vector2)position, (float)angle, (Space.Mutable)space, (Spaceship.Mutable)shooter.
    /// </summary>
    public override int Activate(params object[] activateParams) {
        var index = base.Activate(activateParams);

        var space = (Space.Mutable)activateParams[index++];
        var shooter = (Spaceship.Mutable)activateParams[index++];
        space.RegisterShot(this, shooter);

        return index;
    }
}
}
