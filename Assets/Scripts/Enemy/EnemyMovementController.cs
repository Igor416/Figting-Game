using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour {
    private const float chaseSpeed = 3.2f;
    private float moveSpeed;

    public RuntimeAnimatorController[] animatorControllers;

    private Transform target;

    private float currentAngle;
    private Vector3 currentDirection;
    private Vector3 attackPosition;

    public bool Dead { get; set; }
    public bool Damage { get; set; }
    public bool LookingAround { get; set; }
    public bool Chasing { get; set; }
    public bool Attacking { get; set; }

    private AnimatorService animatorService;
    private enum Animators { Attacks = 0, Damages, Deaths, Idle, Walk }

    private IEnumerator LookAround() {
        yield return new WaitForSeconds(Random.Range(0, 30));
        while (true) {
            yield return new WaitForSeconds(Random.Range(7, 9.5f));
            LookingAround = true;
            animatorService.SetAnimator(Animators.Idle);
            animatorService.Play("Skeleton@Idle01_Action01");
            while (Chasing || Attacking) {
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private IEnumerator Patrol() {
        animatorService.SetAnimator(Animators.Walk);
        int i = Random.Range(0, 4);
        while (true) {
            for (; i < 4; i++) {
                currentAngle = i * 90;
                currentDirection = GetDirection();
                transform.rotation = Quaternion.Euler(0, currentAngle, 0);
                while (Attacking || Chasing) {
                    yield return new WaitForSeconds(0.02f);
                }
                yield return new WaitForSeconds(Random.Range(4, 5.5f));
            }
            i = 0;
        }
    }

    void Start() {
        animatorService = new AnimatorService(animatorControllers, GetComponent<Animator>());

        moveSpeed = Random.Range(0.3f, 0.8f);
        StartCoroutine(LookAround());
        StartCoroutine(Patrol());
        Chasing = false;
        Attacking = false;
    }

    void FixedUpdate() {
        if (Dead) {
            animatorService.SetAnimator(Animators.Deaths);
            Destroy(gameObject, 0.5f);
        } else if (Damage) {
            animatorService.SetAnimator(Animators.Damages);
        } else if (!Damage) {
            if (Attacking || Chasing) {
                if (Attacking) {
                    animatorService.SetAnimator(Animators.Attacks);
                    transform.position = attackPosition;
                } else {
                    animatorService.SetAnimator(Animators.Walk);
                    transform.position = Vector3.MoveTowards(transform.position, target.position, Time.smoothDeltaTime * chaseSpeed);
                }
                Vector3 lookAtPos = target.position;
                transform.forward = lookAtPos - transform.position;
                currentAngle = transform.eulerAngles.y;
            } else if (!LookingAround) {
                animatorService.SetAnimator(Animators.Walk);
                transform.position += Time.deltaTime * moveSpeed * currentDirection;
            }
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        }
    }

    private void Walk() {
        LookingAround = false;
        animatorService.SetAnimator(Animators.Walk);
    }

    private void EndTakingDamage() {
        Damage = false;
    }

    public void Chase(HeroMovementController target) {
        Chasing = true;
        Attacking = false;
        this.target = target.transform;
    }

    public void EndChase() {
        Chasing = false;
        Attacking = false;
        this.target = null;
    }

    public void Attack(HeroMovementController target) {
        attackPosition = transform.position;
        Attacking = true;
        this.target = target.transform;
    }

    public void EndAttack(HeroMovementController target) {
        Chasing = true;
        Attacking = false;
        animatorService.SetAnimator(Animators.Walk);
        this.target = target.transform;
    }

    private Vector3 GetDirection() {
        switch (currentAngle) {
            case 0:
                return new Vector3(0, 0, 1);
            case 90:
                return new Vector3(1, 0, 0);
            case 180:
                return new Vector3(0, 0, -1);
            case 270:
                return new Vector3(-1, 0, 0);
            default:
                return new Vector3();
        }
    }
}
