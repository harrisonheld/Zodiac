using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using static UnityEditor.Progress;
using Raws;

namespace UI
{
    class ConversationChoice : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        public void SetChoice(ConversationNode node)
        {
              this.text.text = node.PlayerText;
        }
    }
}