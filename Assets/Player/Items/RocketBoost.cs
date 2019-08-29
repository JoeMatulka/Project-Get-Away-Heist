using System.Collections;
using UnityEngine;

public class RocketBoost : PlayerItem
{
    private const string NAME = "ROCKET BOOST";
    private const float COOLDOWN = 30;
    private const float DURATION = 3;

    private const float BOOST_POWER = 2500;

    public RocketBoost()
    {
        itemName = NAME;
        coolDown = COOLDOWN;
    }
    public override void Use()
    {
        if (CoolDownTimer <= 0) {
            StartCoroutine(StartCoolDownTimer());
            StartCoroutine(Boost());
        }
    }
    private IEnumerator Boost()
    {
        float boostTime = 0;
        while (boostTime < DURATION)
        {
            Player.gadgets.IsRocketBoosting = true;
            boostTime += Time.deltaTime;
            Player.Rigidbody.AddForce(BOOST_POWER * Player.transform.forward);
            yield return new WaitForEndOfFrame();
        }
        Player.gadgets.IsRocketBoosting = false;
        yield return 0;
    }
}
