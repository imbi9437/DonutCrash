using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PremiumCategoryPanel : ShopCategoryPanel
{
    public override int PanelType => (int)ShopCategoryType.Premium;
    
    private void OnEnable()
    {
        ShowMerchandiseButton(MerchandiseType.BuyCash);
    }
}
