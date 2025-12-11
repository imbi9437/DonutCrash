using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonutSelectCardUI : MonoBehaviour
{
    [SerializeField] private Text donutNameText;

    private DonutInstanceData _donutData;
    private DeckSelect _deckSelect;

    public void Initialize(DonutInstanceData data, DeckSelect deckSelect)
    {
        _donutData = data;
        _deckSelect = deckSelect;

        donutNameText.text = data.name;

        Button button = GetComponent<Button>();
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnCardClicked);
        }
    }
    private void OnCardClicked()
    {
        Debug.Log($"{_donutData} 도넛을 선택했습니다!");
    }
}
