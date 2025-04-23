using UnityEngine;
using TMPro;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleCounter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _bubbleCounterLabel;
        
        private InfuseServiceCollection<Bubble> _bubbleCollection;
        
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        public void OnInfuse(InfuseServiceCollection<Bubble> bubbleCollection)
        {
            Debug.Log("BubbleCounter.OnInfuse()", gameObject);

            _bubbleCollection = bubbleCollection;
            _bubbleCollection.OnServiceAdded += BubbleAdded;
            _bubbleCollection.OnServiceRemoved += BubbleRemoved;

            UpdateBubbleCount();
        }

        private void OnDefuse()
        {
            Debug.Log("BubbleCounter.OnDefuse()", gameObject);

            _bubbleCollection.OnServiceAdded -= BubbleAdded;
            _bubbleCollection.OnServiceRemoved -= BubbleRemoved;
            _bubbleCollection = null;

            UpdateBubbleCount();
        }

        private void BubbleAdded(Bubble bubble)
        {
            Debug.Log("BubbleCounter.BubbleAdded()", bubble.gameObject);
            
            UpdateBubbleCount();
        }

        private void BubbleRemoved(Bubble bubble)
        {
            Debug.Log("BubbleCounter.BubbleRemoved()");
            
            UpdateBubbleCount();
        }

        private void UpdateBubbleCount()
        {
            if (_bubbleCollection == null)
            {
                _bubbleCounterLabel.text = "No Bubbles";
                return;
            }

            int bubbleCount = _bubbleCollection.Count;
            
            _bubbleCounterLabel.text = $"Bubbles: {bubbleCount}";
        }
    }
}
