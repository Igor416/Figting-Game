using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollider : MonoBehaviour {
    public float RechargeTime { get; set; } = 50;
    public bool Blocked { get; set; }

    private bool blocking;
    [SerializeField] private bool possesedByPlayer = false;
    private string weaponTag = "SwordCollider";

    void Start() {
        weaponTag = possesedByPlayer ? "EnemyWeapon" : "SwordCollider";
        blocking = true;
    }

    private IEnumerator Recharge() {
        yield return new WaitForSeconds(RechargeTime);
        blocking = true;
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag(weaponTag)) {
            if (blocking) {
                Blocked = true;
                blocking = false;
                StartCoroutine(Recharge());
            } else {
                Blocked = false;
            }
        }
    }
}
