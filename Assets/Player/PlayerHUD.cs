using Heist;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PlayerHUD : MonoBehaviour
{
    private Text scoreDisplay;

    private HeistManager heistManager;

    void Awake()
    {
        scoreDisplay = GetComponentInChildren<Text>();
        scoreDisplay.text = "$ 0";
    }

    void Start()
    {
        heistManager = HeistService.Instance.FindCurrentHeist();
        if (heistManager != null)
        {
            heistManager.AddToScore.AddListener(UpdateScoreDisplay);
            heistManager.RemoveFromScore.AddListener(UpdateScoreDisplay);
        }
    }

    private void UpdateScoreDisplay(float amount)
    {
        // TODO: Add floating display to amount added or removed from the score
        scoreDisplay.text = $"$ {(double) heistManager.Score}";
    }
}
