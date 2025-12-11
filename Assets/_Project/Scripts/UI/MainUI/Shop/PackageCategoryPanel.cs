using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackageCategoryPanel : ShopCategoryPanel
{
    public override int PanelType => (int)ShopCategoryType.Package;
    
    
    private void OnEnable()
    {
        ShowMerchandiseButton(MerchandiseType.Package);
    }
}
