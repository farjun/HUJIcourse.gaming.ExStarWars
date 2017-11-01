namespace StarWars.Actions {
public class TurnRight : Action {
    public static Action action = new TurnRight();

    public void Do(Spaceship.Mutable spaceship) {
        spaceship.Rotation -= Spaceship.ROTATION_PER_ACTION;
    }

    public bool CanDo(Spaceship spaceship) {
        return spaceship.IsAlive;
    }
}
}