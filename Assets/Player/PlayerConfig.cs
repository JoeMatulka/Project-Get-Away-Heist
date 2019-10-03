using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    public Player Player;

    public int playerItemId = 0;
    private PlayerItem playerItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void ApplyPlayerItemById(int itemId) {
        switch ((PlayerItemLibrary) itemId) {
            case PlayerItemLibrary.ROCKET_BOOST:
                playerItem = Player.gameObject.AddComponent<RocketBoost>();
                break;
            case PlayerItemLibrary.JUMP:
                playerItem = Player.gameObject.AddComponent<Jump>();
                break;
            case PlayerItemLibrary.NONE:
            default:
                break;
                
        }
    }

    public PlayerItem PlayerItem {
        get {
            return playerItem;
        }
    }
}
