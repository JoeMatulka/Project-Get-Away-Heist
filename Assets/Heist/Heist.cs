using UnityEngine;
using UnityEngine.Events;

namespace Heist
{
    public enum HeistEndState { 
        SUCCESS,
        FAILED
    }
    [System.Serializable]
    public class ScoreEvent : UnityEvent<float>
    {
    }
    [System.Serializable]
    public class EndHeistEvent : UnityEvent<HeistEndState> { }
    public class HeistManager : MonoBehaviour
    {
        public float Score = 0;

        // Heist life cycle
        private UnityEvent startHeistCountdown = new UnityEvent();
        private UnityEvent startHeist = new UnityEvent();
        private UnityEvent endMoneyTruckPhase = new UnityEvent();
        private EndHeistEvent endHeist = new EndHeistEvent();

        // Heist events
        private ScoreEvent addToScore = new ScoreEvent();
        private ScoreEvent removeFromScore = new ScoreEvent();

        void Awake()
        {
            addToScore.AddListener(AddAmountToScore);
            removeFromScore.AddListener(RemoveAmountFromScore);
        }

        private void AddAmountToScore(float amount)
        {
            Score += amount;
        }

        private void RemoveAmountFromScore(float amount) {
            Score -= amount;
            if (Score <= 0) { endHeist.Invoke(HeistEndState.FAILED); }
        }

        public void RemoveAllListeners()
        {
            startHeistCountdown.RemoveAllListeners();
            startHeist.RemoveAllListeners();
            endMoneyTruckPhase.RemoveAllListeners();
            endHeist.RemoveAllListeners();
            addToScore.RemoveAllListeners();
        }

        public ScoreEvent AddToScore
        {
            get { return addToScore; }
        }

        public UnityEvent StartHeistCountdown
        {
            get { return startHeistCountdown; }
        }

        public UnityEvent StartHeist
        {
            get { return startHeist; }
        }

        public UnityEvent EndMoneyTruckPhase
        {
            get { return endMoneyTruckPhase; }
        }

        public EndHeistEvent EndHeist
        {
            get { return endHeist; }
        }
    }
    public class HeistService : Singleton<HeistService>
    {
        private static readonly string HEIST_OBJ_NAME = "heist";

        protected HeistService() { }

        public void CreateHeist()
        {
            if (FindCurrentHeist() != null)
            {
                new GameObject(HEIST_OBJ_NAME).AddComponent<HeistManager>();
            }
            else
            {
                Debug.LogError("Tried instantiating more than one heist at a time.");
            }
        }

        public void DeleteHeist()
        {
            Destroy(FindCurrentHeist().gameObject);
        }

        public HeistManager FindCurrentHeist()
        {
            return GameObject.Find(HEIST_OBJ_NAME).GetComponent<HeistManager>();
        }
    }
}
