using TMPro;
using UnityEngine;

public class DonutStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;

    public void Setup(float health, int attack)
    {
        healthText.text = Mathf.CeilToInt(health).ToString();
        attackText.text = attack.ToString();
    }
}
