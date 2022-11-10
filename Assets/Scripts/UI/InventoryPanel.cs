using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour {
    public Transform Hand;

    public RectTransform cursor;
    public Button[] ItemButtons;

    Item[] items;

    private int activeSlot = -1;
    public int ActiveSlot {
        get {
            return activeSlot;
        }
        set {
            if (value == activeSlot) {
                activeSlot = -1;
                cursor.gameObject.SetActive(false);
                if (value >= 0) {
                    if (!(items[value] is null)) {
                        items[value].UnSelect();
                    }
                }

                Inventory.OnSlotSelected(name);
                return;
            }
            activeSlot = value;
            if (!cursor.gameObject.activeSelf) {
                cursor.gameObject.SetActive(true);
            }
            Inventory.OnSlotSelected(name);

            cursor.localPosition = new Vector2(ItemButtons[value].transform.localPosition.x, -33);

            for (int i = 0; i < maxItems; i++) {
                if (!(items[i] is null)) {
                    if (i == value) {
                        items[i].Select();
                    } else {
                        items[i].UnSelect();
                    }
                }
            }
        }
    }
    int maxItems;

    void Awake() {
       maxItems = ItemButtons.Length;
       items = new Item[maxItems];
    }

    public Item GetSelected() {
        if (activeSlot == -1) {
            return null;
        }
        return items[activeSlot];
    }

    public bool IsEmpty() {
        foreach (var item in items) {
            if (!(item is null)) {
                return false;
            }
        }
        return true;
    }

    public void Add(Item item, bool isPrefab = false) {
        Transform parent;
        if (isPrefab) {
            parent = Hand;
            item = Instantiate(item, Hand);
        } else {
            parent = item.transform.parent;
            item.transform.SetParent(Hand);
        }
        item.PickUp();

        for (int i = 0; i < maxItems; i++) {
            if (items[i] is null) {
                UpdateSlot(i, item);
                ActiveSlot = i;
                return;
            }
        }

        if (activeSlot == -1) {
            ActiveSlot = 0;
        }
        Remove(activeSlot, parent);
        UpdateSlot(activeSlot, item);
    }

    public void Remove(int i, Transform newParent) {
        Item item = items[i];
        item.transform.SetParent(newParent);
        item.Throw();
        UpdateSlot(i, null);
    }

    private void UpdateSlot(int i, Item newItem) {
        items[i] = newItem;

        var color = new Color(255, 255, 255);
        if (newItem is null) {
            color.a = 0;
            ItemButtons[i].image.sprite = null;
        } else {
            color.a = 1;
            ItemButtons[i].image.sprite = newItem.Sprite;
        }

        ItemButtons[i].image.color = color;
    }
}
