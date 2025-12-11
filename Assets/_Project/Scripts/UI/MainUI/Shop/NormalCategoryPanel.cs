using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalCategoryPanel : ShopCategoryPanel
{
    public override int PanelType => (int)ShopCategoryType.Normal;
    
    private void OnEnable()
    {
        ShowMerchandiseButton(MerchandiseType.BuyGold);
    }
}
