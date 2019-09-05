using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : PlayerItem
{
    private const string NAME = "JUMP";
    private const float COOLDOWN = 45;
    private const float DURATION = 5;

    private const float JUMP_POWER = 4500;
    private const float GLIDE_POWER = 480;

    public Jump()
    {
        itemName = NAME;
        coolDown = COOLDOWN;
    }
    public override void Use()
    {
        if (CoolDownTimer <= 0)
        {
            StartCoroutine(StartCoolDownTimer());
            StartCoroutine(ActivateJump());
        }
    }

    private IEnumerator ActivateJump()
    {
        Player.Rigidbody.AddForce(JUMP_POWER * Player.transform.up, ForceMode.Impulse);
        float jumpTime = 0;
        while (jumpTime < DURATION)
        {
            Player.gadgets.IsParachuting = true;
            jumpTime += Time.deltaTime;
            if (!Player.IsWheelsOnGround && Player.Rigidbody.velocity.y < 0)
            {
                // Slowly fall back down
                Player.Rigidbody.AddForce(GLIDE_POWER * Player.transform.up);
            }
            yield return new WaitForEndOfFrame();
        }
        Player.gadgets.IsParachuting = false;
        yield return 0;
    }
}
