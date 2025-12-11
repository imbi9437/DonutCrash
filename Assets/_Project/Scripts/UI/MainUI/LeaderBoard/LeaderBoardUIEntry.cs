using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUIEntry : MonoBehaviour
{
    [SerializeField] private Color otherColor;
    [SerializeField] private Color ownColor;
    
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private TextMeshProUGUI score;

    public void Setup(LeaderboardEntry entry)
    {
        background.color = entry.uid != DataManager.Instance.UserUid ? otherColor : ownColor;
        nickName.text = entry.nickName;
        score.text = entry.score.ToString();
    }
}
