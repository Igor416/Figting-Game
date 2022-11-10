using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSightController : MonoBehaviour {
    const float viewRadius = 9f;
    const float attackRadius = 1.5f;
    const float angle = 110f;
    const float wideAngle = 300f;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool CanSeePlayer { get; set; }
    public bool CanAttackPlayer { get; set; }

    private HeroMovementController Player;
    private EnemyMovementController movementController;

    private IEnumerator LineOfSightRoutine() {
        while (true) {
            yield return new WaitForSeconds(0.2f);
            if (Player.Dead) {
                CanSeePlayer = false;
                CanAttackPlayer = false;
                movementController.EndAttack(Player);
                movementController.EndChase();
                break;
            }

            CanSeeCheck();
            if (CanSeePlayer) {
                CanAttackCheck();
            } else {
                CanAttackPlayer = false;
            }

            if (CanAttackPlayer && !movementController.Attacking) {
                movementController.Attack(Player);
            } else if (CanSeePlayer && !movementController.Chasing) {
                movementController.Chase(Player);
            }

            if (!CanAttackPlayer && movementController.Attacking) {
                movementController.EndAttack(Player);
            }

            if (!CanSeePlayer && movementController.Chasing) {
                movementController.EndChase();
            }
        }
    }

    void Start() {
        Player = GameObject.FindGameObjectWithTag("Hero").GetComponent<HeroMovementController>();
        movementController = GetComponent<EnemyMovementController>();
        StartCoroutine(LineOfSightRoutine());
    }

    private void CanSeeCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < (movementController.LookingAround ? wideAngle : angle) / 2) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                CanSeePlayer = !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
            } else {
                CanSeePlayer = false;
            }
        } else if (CanSeePlayer) {
            CanSeePlayer = false;
        }
    }

    private void CanAttackCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, attackRadius, targetMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < 180) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                CanAttackPlayer = !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
            } else {
                CanAttackPlayer = false;
            }
        } else if (CanSeePlayer) {
            CanAttackPlayer = false;
        }
    }
}