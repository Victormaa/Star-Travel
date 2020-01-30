using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InventoryItemBase
{
    public int FoodPoints = 20;

    public override void OnUse()
    {
        GameManager.Instance.Player.Eat(FoodPoints);

        GameManager.Instance.Player.Inventory.RemoveItem(this);

        Destroy(this.gameObject);
    }
}
