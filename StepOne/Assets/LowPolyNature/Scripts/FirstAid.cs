using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : InventoryItemBase
{
    public int HealthPoints = 20;

    public override void OnUse()
    {
        GameManager.Instance.Player.Rehab(HealthPoints);

        GameManager.Instance.Player.Inventory.RemoveItem(this);

        Destroy(this.gameObject);
    }
}
