namespace StarWars.Actions {
public class ShieldUp : Action {
    public static Action action = new ShieldUp();

    public void Do(Spaceship.Mutable spaceship) {
        spaceship.ShieldUp();
    }

    public bool CanDo(Spaceship spaceship) {
        return spaceship.CanRaiseShield;
    }
}
}