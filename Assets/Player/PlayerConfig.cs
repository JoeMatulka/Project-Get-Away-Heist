using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    private Player player;

    private int playerItemId = 0;
    private PlayerItem playerItem;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        ApplyPlayerItemById(playerItemId);
    }

    private void ApplyPlayerItemById(int itemId)
    {
        switch ((PlayerItemLibrary)itemId)
        {
            case PlayerItemLibrary.ROCKET_BOOST:
                playerItem = player.gameObject.AddComponent<RocketBoost>();
                break;
            case PlayerItemLibrary.JUMP:
                playerItem = player.gameObject.AddComponent<Jump>();
                break;
            case PlayerItemLibrary.NONE:
            default:
                break;

        }
    }

    public PlayerItem PlayerItem
    {
        get
        {
            return playerItem;
        }
    }

    public int PlayerItemId
    {
        set { playerItemId = value; }
        get { return playerItemId; }
    }
}
