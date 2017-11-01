namespace StarWars.Actions {
public class DoNothing : Action {
    public static Action action = new DoNothing();

    public void Do(Spaceship.Mutable spaceship) {
        // Empty on purpose.
    }

    public bool CanDo(Spaceship spaceship) {
        return true;
    }
}
}