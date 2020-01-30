using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(InventoryItemBase item)
    {
        Item = item;
    }

    public InventoryItemBase Item;
}

