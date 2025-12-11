using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiamondCategoryPanel : ShopCategoryPanel
{
    public override int PanelType => (int)ShopCategoryType.DiamondExchange;
    
    private void OnEnable()
    {
        ShowMerchandiseButton(MerchandiseType.ExchangeCash);
    }
}
