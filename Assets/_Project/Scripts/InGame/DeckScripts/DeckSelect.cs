using _Project.Scripts.InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelect : MonoBehaviour
{
    public Transform donutSelectPanel;
    public GameObject donutSelectCardPrefab;
    private DonutInstanceData selectedInitialDonut;

    private List<DonutInstanceData> GetRandomDonuts(DeckData deckData, int count)
        {
            List<DonutInstanceData> availableDonuts = new List<DonutInstanceData>(deckData.waitingDonuts);
            List<DonutInstanceData> selected = new List<DonutInstanceData>();

            if(availableDonuts.Count < count)
            {
                Debug.LogWarning("대기 도넛이 3개 미만이므로, 모두 선택합니다.");
                return availableDonuts;
            }

            for(int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, availableDonuts.Count);
                selected.Add(availableDonuts[randomIndex]);
                availableDonuts.RemoveAt(randomIndex);
            }
            return selected;
        }

        private void ShowDonutSelectionUI(InGameOwner owner, DeckData deckData)
    {
        List<DonutInstanceData> selectionDonuts = GetRandomDonuts(deckData, 3);

        foreach(Transform child in donutSelectPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var donutData in selectionDonuts)
        {
            GameObject cardObject = Instantiate(donutSelectCardPrefab, donutSelectPanel);

            DonutSelectCardUI cardUI = cardObject.GetComponent<DonutSelectCardUI>();
            if(cardUI != null)
            {
                cardUI.Initialize(donutData, this);
            }
            else
            {
                Debug.LogError("프리펩에 없습니다.");
            }
        }

        donutSelectPanel.gameObject.SetActive(true);
    }
}
