using UnityEngine;
using TMPro;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleCounterService : MonoBehaviour, IBubbleCounterService, InfuseAs<IBubbleCounterService>
    {
        [SerializeField]
        private TMP_Text _bubbleCounterLabel;

        [SerializeField]
        private InfuseScriptableContext _infuseContext;
        
        private int _bubbleCount;
        
        private void Awake()
        {
            _infuseContext.Register(this);

            UpdateBubbleCount();
        }

        public void AddBubble()
        {
            ++_bubbleCount;
            UpdateBubbleCount();
        }

        public void RemoveBubble()
        {
            --_bubbleCount;
            UpdateBubbleCount();
        }

        private void UpdateBubbleCount()
        {
            _bubbleCounterLabel.text = $"Bubbles: {_bubbleCount}";
        }
    }
}
