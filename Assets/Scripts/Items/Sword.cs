using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item {
    public int Damage;
    public float Cooldown;

    protected override void Start() {
        base.Start();
        Name = Name.Replace(" w_ Colliders", "");
    }

    protected override void SetInHand() {
        base.SetInHand();
        transform.localPosition = new Vector3(-0.08f, 0.025f, 0.025f);
        transform.localRotation = Quaternion.Euler(90, 90, 90);
    }

    public override string ToString() {
        return $"Sword {Name}, Hits Remained {HitsRemained} / {MaxRemained}\nDamage: {Damage}, Cooldown: {Cooldown}s";
    }
}
