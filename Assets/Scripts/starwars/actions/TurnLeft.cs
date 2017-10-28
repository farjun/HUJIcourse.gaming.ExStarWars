namespace StarWars.Actions {
public class TurnLeft : Action {
    public static Action action = new TurnLeft();
    
    public void Do(Spaceship.Mutable spaceship) {
        spaceship.Rotation += Spaceship.ROTATION_PER_ACTION;
    }

    public bool CanDo(Spaceship spaceship) {
        return spaceship.IsAlive;
    }
}
}