using System.Collections;
using UnityEngine;
public abstract class PlayerItem: MonoBehaviour
{
    public Player Player;

    protected string itemName;
    protected float coolDown;

    protected float coolDownTimer = 0;

    public abstract void Use();

    public IEnumerator StartCoolDownTimer() {
        coolDownTimer = coolDown;
        while (coolDownTimer >= 0)
        {
            coolDownTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    public string ItemName {
        get { return itemName; }
    }

    public float CoolDownTimer {
        get { return coolDownTimer; }
    }
}
