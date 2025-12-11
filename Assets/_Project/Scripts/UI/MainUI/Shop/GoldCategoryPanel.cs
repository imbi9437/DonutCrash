using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldCategoryPanel : ShopCategoryPanel
{
    public override int PanelType => (int)ShopCategoryType.GoldExchange;

    private void OnEnable()
    {
        ShowMerchandiseButton(MerchandiseType.ExchangeGold);
    }
}
