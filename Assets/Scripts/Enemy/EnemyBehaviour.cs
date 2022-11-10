using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    public float HP { get; set; } = 30;

    public ShieldCollider ShieldCollider;
    EnemyMovementController movementController;

    void Start() {
        movementController = GetComponent<EnemyMovementController>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("SwordCollider")) {
            if (!ShieldCollider.Blocked) {
                HP -= collision.collider.transform.parent.GetComponent<Sword>().Damage - 1.5f; // 1.5f is shield block
                if (HP <= 0) {
                    movementController.Dead = true;
                } else {
                    movementController.Damage = true;
                }
            }
        } else if (collision.collider.CompareTag("Respawn")) {
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnDestroy() {
        GameController.OnScored();
    }
}
