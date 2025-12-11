using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//노드 패널에서 도넛의 Uid와 아이콘만 주고받을 스크립트.
public class DonutNode : MonoBehaviour
{
    public TextMeshProUGUI nodeNameText;
    public Button nodeBtn;
    public Image icon;

    public RecipeNodeData nodeData;
    public List<DonutNode> nextNodes;

    public GameObject isLockIcon;

    public bool isSet = false;

    public int i = 0;
    void Awake()
    {
        icon = transform.Find("Frame Out/Slot/Icon").GetComponent<Image>();
        nodeNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        nodeBtn = GetComponent<Button>();
    }
}


