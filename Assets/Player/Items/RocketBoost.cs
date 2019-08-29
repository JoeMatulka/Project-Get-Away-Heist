using System.Collections;
using UnityEngine;

public class RocketBoost : PlayerItem
{
    private const string NAME = "ROCKET BOOST";
    private const float COOLDOWN = 30;
    private const float DURATION = 3;

    public RocketBoost()
    {
        itemName = NAME;
        cooldown = COOLDOWN;
    }
    public override void Use()
    {
        if (CoolDownTimer <= 0) {
            coolDownTimer = COOLDOWN;
            StartCoroutine(Boost());
        }
    }
    private IEnumerator Boost()
    {
        float boostTime = 0;
        while (boostTime < DURATION)
        {
            boostTime += Time.deltaTime;
            Debug.Log("BOOST");
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }
}
