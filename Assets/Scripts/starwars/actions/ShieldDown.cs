namespace StarWars.Actions {
public class ShieldDown : Action {
    public static Action action = new ShieldDown();

    public void Do(Spaceship.Mutable spaceship) {
        spaceship.ShieldDown();
    }

    public bool CanDo(Spaceship spaceship) {
        return spaceship.IsShieldUp;
    }
}
}