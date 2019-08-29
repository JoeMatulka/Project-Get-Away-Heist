using UnityEngine;
public abstract class PlayerItem: MonoBehaviour
{
    public Player Player;

    protected string itemName;
    protected float cooldown;

    protected float coolDownTimer = 0;

    public abstract void Use();

    public string ItemName {
        get { return itemName; }
    }

    public float CoolDownTimer {
        get { return coolDownTimer; }
    }
}
