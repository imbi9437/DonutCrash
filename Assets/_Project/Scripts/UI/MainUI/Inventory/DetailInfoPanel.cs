using _Project.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;

public class DetailInfoPanel : MonoBehaviour
{
    [SerializeField]private GameObject donutInfoPanel;
    
    [SerializeField]private Image donutImage; //도넛 이미지 
    [SerializeField]private TMP_Text donutNameText; //도넛 이름 
    [SerializeField]private TMP_Text donutEffectText; //도넛 특수효과 설명text
    [SerializeField]private TMP_Text donutLvText; //도넛 레벨
    [SerializeField]private TMP_Text donutAtkText; //도넛 공격력 
    [SerializeField]private TMP_Text donutDefText;
    [SerializeField]private TMP_Text donutHpText;
    [SerializeField]private TMP_Text donutMassText;
    [SerializeField]private TMP_Text donutCritText;
    
    [SerializeField]private GameObject bakerInfoPanel;
    
    [SerializeField]private Image bakerImage;
    [SerializeField]private TMP_Text bakerNameText;
    [SerializeField]private TMP_Text bakerEffectText;
    [SerializeField]private TMP_Text bakerLvText;

    private DonutInstanceData currentDonut; //현재 선택된 도넛 
    private BakerInstanceData currebtBaker; //현재 선택된 마녀 
    private string ingredientuid; //재료 uid
    private int stack; //재료 스택 
    
    private DonutData donutData;
    private BakerData bakerData;
    private IngredientData ingredientData;
    
    private void Awake()
    {
        PanelStart();
        EventHub.Instance?.RegisterEvent<US.RequestDonutInstanceData>(DonutDataConnect);
        EventHub.Instance?.RegisterEvent<US.RequestBakerInstanceData>(BakerDataConnect);
        EventHub.Instance?.RegisterEvent<US.RequestIngredientData>(IngredientConnect);

        EventHub.Instance?.RegisterEvent<US.RequestPanelClear>(PanelStartRapper);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<US.RequestDonutInstanceData>(DonutDataConnect);
        EventHub.Instance?.UnRegisterEvent<US.RequestBakerInstanceData>(BakerDataConnect);
        EventHub.Instance?.UnRegisterEvent<US.RequestIngredientData>(IngredientConnect);
        EventHub.Instance?.UnRegisterEvent<US.RequestPanelClear>(PanelStartRapper);
    }

    private void PanelStartRapper(US.RequestPanelClear evt) => PanelStart();

    private void DonutDataConnect(US.RequestDonutInstanceData evt)
    {
        if (currentDonut == evt.donutinstance)
        {
            return;
        }
        currentDonut = evt.donutinstance;
        DataManager.Instance?.TryGetDonutData(currentDonut.origin, out donutData);
        DonutInfoUIUpdate();
        currebtBaker = null;
        ingredientuid = null;
    }

    private void BakerDataConnect(US.RequestBakerInstanceData evt)
    {
        if (currebtBaker == evt.bakerinstance)
        {
            return;
        }
        currebtBaker = evt.bakerinstance;
        DataManager.Instance?.TryGetBakerData(currebtBaker.origin, out bakerData);
        BakerInfoUIUpdate();
        currentDonut = null;
        ingredientuid = null;
    }

    private void IngredientConnect(US.RequestIngredientData evt)
    {
        if (ingredientuid == evt.ingredient)
        {
            return;
        }
        ingredientuid = evt.ingredient;
        stack = evt.ingredientstack;
        DataManager.Instance?.TryGetIngredientData(ingredientuid, out ingredientData);
        IngredientUIUpdate();
        currebtBaker = null;
        currentDonut = null;
    }

    private void DonutInfoUIUpdate()
    {
        bakerInfoPanel.SetActive(false);
        donutInfoPanel.SetActive(true);

        if (donutData == null || currentDonut == null) return;
        
        currentDonut.CalcDonutStatus();

        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x => donutImage.sprite = x).Forget();
        
        Debug.Log($"====={donutNameText}");
        Debug.Log(donutData);
        
        donutNameText.text = donutData.donutName;
        donutAtkText.text = $"  {currentDonut.atk}";
        donutDefText.text = $"DEF : {currentDonut.def}";
        donutHpText.text = $"  {currentDonut.hp}";
        donutCritText.text = $"CRI : {currentDonut.crit.ToPercent()}%";
        donutLvText.text = $"LV : {currentDonut.level}";
        donutMassText.text = $"MASS : {currentDonut.mass}";

        donutEffectText.text = donutData.donutDescription; //설명 
    }

    private void BakerInfoUIUpdate()
    {
        bakerInfoPanel.SetActive(true);
        donutInfoPanel.SetActive(false);
        bakerNameText.text = bakerData.bakerName;
        bakerLvText.text = $"LV :{currebtBaker.level}";
        bakerEffectText.text = bakerData.bakerDescription;
        AddressableLoader.AssetLoadByPath<Sprite>(bakerData.resourcePath, x => bakerImage.sprite = x).Forget();
    }

    private void IngredientUIUpdate()
    {
        bakerInfoPanel.SetActive(true);
        donutInfoPanel.SetActive(false);
        bakerLvText.text = "";
        bakerNameText.text = ingredientData.ingredientName;
        bakerEffectText.text = ingredientData.ingredientDescription;
        
        AddressableLoader.AssetLoadByPath<Sprite>(ingredientData.resourcePath, x => bakerImage.sprite = x).Forget();
    }

    private void PanelStart()
    {
        bakerInfoPanel.SetActive(false);
        donutInfoPanel.SetActive(false);
    }
}
