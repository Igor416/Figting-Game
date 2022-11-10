using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryEvent(string panel);
public delegate void ItemEvent(Item item);

public class Inventory : MonoBehaviour {
    public Transform RightHand;
    public Transform LeftHand;

    InventoryPanel swords;
    InventoryPanel shields;

    public static event InventoryEvent SlotSelected;
    public static void OnSlotSelected(string panel) {
        SlotSelected?.Invoke(panel);
    }

    public static event ItemEvent ItemInRange;
    public static event ItemEvent ItemOutOfRange;

    public static void OnItemInRange(Item item) {
        ItemInRange?.Invoke(item);
    }
    public static void OnItemOutOfRange(Item item) {
        ItemOutOfRange?.Invoke(item);
    }

    public static event ItemEvent ItemExpired;
    public static void OnItemExpired(Item item) {
        ItemExpired?.Invoke(item);
    }


    bool canPickUp;
    Item itemToPickUp;

    private HeroMovementController movementController;

    void Awake() {
        swords = GameObject.Find("Swords Inventory").GetComponent<InventoryPanel>();
        shields = GameObject.Find("Shields Inventory").GetComponent<InventoryPanel>();

        swords.Hand = RightHand;
        shields.Hand = LeftHand;

        movementController = GetComponent<HeroMovementController>();

        SlotSelected = ChangeIdle;

        ItemInRange += (Item item) => {
            itemToPickUp = item;
            canPickUp = true;
        };
        ItemOutOfRange = (Item item) => {
            itemToPickUp = null;
            canPickUp = false;
        };
        ItemExpired = (Item item) => {
            if (item is Sword) {
                Throw(swords);
            } else {
                Throw(shields);
            }
        };
    }

    void Update() {
        for (int i = 1; i <= 3; i++) {
            if (Input.GetKeyDown("" + i)) {
                swords.ActiveSlot = i - 1;
            }
        }
        for (int i = 4; i <= 5; i++) {
            if (Input.GetKeyDown("" + i)) {
                shields.ActiveSlot = i - 4;
            }
        }
        if (canPickUp) {
            if (Input.GetKeyDown(KeyCode.F)) {
                Add(itemToPickUp);
                OnItemOutOfRange(itemToPickUp);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            Throw(swords);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            Throw(shields);
        }
    }

    public void Add(Item item, bool isPrefab = false) {
        if (item.CompareTag("Sword")) {
            swords.Add(item, isPrefab);
        } else {
            if (shields.IsEmpty()) {
                HeroBehaviour.OnShieldObtained(item as Shield);
            }
            shields.Add(item, isPrefab);
        }
    }

    public void Add(Item[] items, bool isPrefab = false) {
        foreach (var item in items) {
            Add(item, isPrefab);
        }

        SetBattling(true);
    }

    public void Add(GameObject[] items, bool isPrefab = false) {
        foreach (var item in items) {
            Add(item.GetComponent<Item>(), isPrefab);
        }

        SetBattling(true);
    }

    public void ChangeIdle(string panel) {
        panel = panel.Replace(" Inventory", "");
        if (panel == "Swords") {
            if (swords.ActiveSlot != -1) {
                SetBattling(true);
                return;
            }
            if (shields.ActiveSlot != -1) {
                shields.ActiveSlot = shields.ActiveSlot;
            }
        } else {
            if (shields.ActiveSlot != -1) {
                SetBattling(true);
                return;
            }
            if (swords.ActiveSlot != -1) {
                swords.ActiveSlot = swords.ActiveSlot;
            }
        }
        SetBattling(false);
    }

    public Sword GetActiveSword() {
        return (Sword)swords.GetSelected();
    }

    private void Throw(InventoryPanel panel) {
        if (panel.ActiveSlot != -1) {
            Item shield = panel.GetSelected();
            if (shields.IsEmpty()) {
                HeroBehaviour.OnShieldLost(shield as Shield);
            }

            panel.Remove(panel.ActiveSlot, movementController.Ground);
            panel.ActiveSlot = panel.ActiveSlot;
            movementController.Battling = false;
        }
    }

    private void SetBattling(bool status) {
        movementController.Battling = status;
    }
}
