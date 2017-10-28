namespace StarWars.Actions {
public interface Action {
    void Do(Spaceship.Mutable spaceship);
    bool CanDo(Spaceship spaceship);
}
}