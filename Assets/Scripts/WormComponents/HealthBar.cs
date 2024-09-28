using System;
using TMPro;
using UnityEngine;

namespace WormComponents
{
    public class HealthBar : MonoBehaviour
    {
        public TextMeshProUGUI m_Text;
        public Health m_Health;

        private void Start()
        {
            m_Health.OnChange += health => m_Text.text = health.ToString();
        }
    }
}