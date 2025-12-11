using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDetailInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI donutName;
    [SerializeField] private Image donutIcon;
    [SerializeField] private TextMeshProUGUI donutDescription;
    [SerializeField] private TextMeshProUGUI donutDef;
    [SerializeField] private TextMeshProUGUI donutCri;
    [SerializeField] private TextMeshProUGUI donutMass;
    [SerializeField] private TextMeshProUGUI donutAtk;
    [SerializeField] private TextMeshProUGUI donutHp;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void SetRecipeDetail(string uid)
    {
        if (DataManager.Instance.TryGetRecipeData(uid, out RecipeData recipe) == false)
        {
            gameObject.SetActive(false);
            return;
        }
        if (DataManager.Instance.TryGetDonutData(recipe.result.itemId, out DonutData donut) == false)
        {
            gameObject.SetActive(false);
            return;
        }
        
        AddressableLoader.AssetLoadByPath<Sprite>(donut.resourcePath, x =>
        {
            if (donutIcon)
                donutIcon.sprite = x;
        });

        donutName.text = donut.donutName;
        donutDescription.text = donut.donutDescription;
        donutDef.text = $"DEF : {donut.def}";
        donutCri.text = $"CRI : {donut.crit.ToPercent()}";
        donutMass.text = $"Mass : {donut.mass}";
        donutAtk.text = donut.atk.ToString();
        donutHp.text = donut.hp.ToString();
    }
}
