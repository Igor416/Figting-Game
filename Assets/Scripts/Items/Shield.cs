using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item {
    public ShieldCollider Collider;

    public int Block;
    public int Armour;
    public float ArmourRegen { get; set; }
    public bool Blocked {
        get {
            return Collider.Blocked;
        }
        set {
            Collider.Blocked = value;
        }
    }

    protected override void Start() {
        base.Start();
        Name = Name.Replace("Wooden_Shield_0", "#");
        ArmourRegen = 0.5f;
    }

    protected override void SetInHand() {
        base.SetInHand();
        transform.localPosition = new Vector3(0.05f, 0, 0.011f);
        transform.localRotation = Quaternion.Euler(250, 90, 0);
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    public override string ToString() {
        return $"Shield {Name}, Hits Remained {HitsRemained} / {MaxRemained}\nDamage Block: {Block}, Armour: {Armour}";
    }
}
