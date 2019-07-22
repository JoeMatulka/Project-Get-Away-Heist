using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Player player;
    private const float CAM_REVERSE_VEL = -.75f;

    public static float DEF_SMOOTH_SPEED = 15f;
    public static Vector3 DEF_CAM_OFFEST = new Vector3(0, -1, 2);

    public float smoothSpeed = DEF_SMOOTH_SPEED;
    public Vector3 offset = DEF_CAM_OFFEST;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    void FixedUpdate()
    {
        if (player != null)
        {
            // Get which direction the player is going
            Vector3 position = player.transform.position;
            Vector3 playerVelocity = player.Rigidbody.velocity;
            var velocityDirection = Vector3.Dot(player.transform.forward, playerVelocity);
            
            // Look
            var newRotation = Quaternion.LookRotation(position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, smoothSpeed * Time.deltaTime);

            // Move
            float zOffset = offset.z;
            if (velocityDirection <= CAM_REVERSE_VEL) {
                // Apply reverse camera
                zOffset *= -1;
            }
            Vector3 newPosition = position - player.transform.forward * zOffset - player.transform.up * offset.y;
            transform.position = Vector3.Slerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
