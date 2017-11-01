namespace StarWars.Actions {
public class Shoot : Action {
    public static Action action = new Shoot();

    public void Do(Spaceship.Mutable spaceship) {
        spaceship.ShotCooldown = Spaceship.SHOT_COOLDOWN;
        Game.SpawnShot(spaceship);
    }

    public bool CanDo(Spaceship spaceship) {
        return spaceship.CanShoot;
    }
}
}