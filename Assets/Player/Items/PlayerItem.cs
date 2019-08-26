public abstract class PlayerItem
{
    protected string name;
    protected float cooldown;

    protected abstract void Use();

    public string Name {
        get { return name; }
    }
}
