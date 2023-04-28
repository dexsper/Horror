public class ManiacState : State
{
    public ManiacBehaviour Behavior { get; private set; }

    public ManiacState(ManiacBehaviour behavior)
    {
        Behavior = behavior;
    }
}