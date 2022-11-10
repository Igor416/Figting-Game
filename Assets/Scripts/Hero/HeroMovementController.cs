using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovementController : MonoBehaviour {
    const float moveSpeed = 2;
    const float runSpeed = 3.5f;
    const float sprintSpeed = 6f;

    public Transform Ground;
    public RuntimeAnimatorController[] animatorControllers;

    private Vector3 lastPosition;

    private bool dead;
    private bool moving;
    private bool jumping;
    private bool sprinting;
    private bool running;
    private float currentSpeed;
    private bool performingAttack;
    private bool damage;

    public bool Dead {
        get {
            return dead;
        }
        set {
            dead = value;
            if (value) {
                lastPosition = transform.position;

                animatorService.SetAnimator(Animators.Battle);
                animatorService.Play("Death");
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Rigidbody>().useGravity = false;
                GameController.OnDied();
            }
        }
    }

    public bool Falling { get; set; }
    public bool Damage {
        get {
            return damage;
        }
        set {
            if (value) {
                performingAttack = false;
                animatorService.SetAnimator(Animators.Battle);
                animatorService.SetSpeed(1.5f);
                animatorService.Play("Get Hit");
            }
            damage = value;
        }
    }
    public bool Battling { get; set; }
    public bool Blocking { get; set; }

    private Inventory inventory;
    private AnimatorService animatorService;
    private enum Animators { Fall = 0, Idle, Jump, Run, Sprint, Strafe, Talk, Walk, Battle }

    void Start() {
        inventory = GetComponent<Inventory>();
        animatorService = new AnimatorService(animatorControllers, GetComponent<Animator>());
        animatorService.SetAnimator(Animators.Idle);
    }

    void FixedUpdate() {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (!Dead) {
            if (Battling) {
                animatorService.SetAnimator(Animators.Battle);
                if (!performingAttack) {
                    GetMovement(out float _);

                    if (Input.GetMouseButton(1)) {
                        Damage = false;
                        animatorService.Play("Cover");
                        Blocking = true;
                    } else {
                        Blocking = false;
                        if (Input.GetMouseButton(0)) {
                            Damage = false;
                            performingAttack = true;
                            string animationId = Input.GetMouseButton(2) ? "2" : "1";
                            animatorService.Play("Attack_0" + animationId);
                            inventory.GetActiveSword().HitsRemained--;

                        } else if (moving) {
                            Damage = false;
                            animatorService.Play("Walk");
                        } else if (!Damage) {
                            animatorService.Play("Idle");
                        }
                    }   
                }
            } else {
                Blocking = false;
                if (Input.GetKey(KeyCode.Space) && !moving && !Input.GetKey(KeyCode.LeftShift)) {
                    Damage = false;
                    jumping = true;
                    animatorService.SetAnimator(Animators.Jump);
                } else if (!jumping) {
                    (float x, float z) = GetMovement(out float speed);
                    Animators animatorName;
                    if (moving || speed != currentSpeed) {
                        Damage = false;
                        animatorName = running ? (sprinting ? Animators.Sprint : Animators.Run) : Animators.Walk;
                        currentSpeed = speed;
                        if (z == -1 && sprinting) {
                            currentSpeed = runSpeed;
                            animatorName = Animators.Run;
                        }

                        if (animatorService.SetAnimator(animatorName) || lastPosition.x != x || lastPosition.z != z) {
                            string name = $"BasicMotions@{animatorName}01 - {GetDirection(z, false)}{GetDirection(x, true)}";
                            animatorService.Play(name);
                        }
                        lastPosition = new Vector3(x, 0, z);
                    }
                    else if (!Damage) {
                        animatorService.SetAnimator(Animators.Idle);
                    }
                }
                EndAttack();
            }
        }
        else {
            EndAttack();
            if (!Falling) {
                transform.position = new Vector3(lastPosition.x, 0, lastPosition.z);
            } else {
                transform.position = new Vector3(lastPosition.x, transform.position.y, lastPosition.z);
            }
        }
    }

    void EndAttack() {
        if (performingAttack) {
            performingAttack = false;
        }
    }

    private void EndJump() {
        jumping = false;
    }

    private void EndBlocking() {
        Blocking = false;
    }

    private void EndTakingDamage() {
        Damage = false;
    }

    private (float x, float z) GetMovement(out float speed) {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        if (new Vector3(xDirection, 0, zDirection).normalized.magnitude >= 0.1f) {
            running = Input.GetKey(KeyCode.LeftShift);
            sprinting = Input.GetKey(KeyCode.Space);
            speed = Battling ? moveSpeed : (running ? (sprinting ? sprintSpeed : runSpeed) : moveSpeed);
            Vector3 lookAtPos = Input.mousePosition;
            lookAtPos.z = Camera.main.transform.position.y - transform.position.y;
            lookAtPos = Camera.main.ScreenToWorldPoint(lookAtPos);

            //transform.forward = lookAtPos - transform.position;
            xDirection = Round(xDirection) * Mathf.Sign(xDirection);
            zDirection = Round(zDirection) * Mathf.Sign(zDirection);
            transform.position += Time.smoothDeltaTime * speed * new Vector3(xDirection, 0, zDirection).normalized;

            //lastRotationY = transform.eulerAngles.y;

            moving = true;
        } else {
            speed = currentSpeed;
            moving = false;
            running = false;
            sprinting = false;
            xDirection = 0;
            zDirection = 0;
        }

        return (xDirection, zDirection);
    }

    private static int Round(float value) {
        if (value < -1) {
            return -1;
        } else if (value == 0) {
            return 0;
        } else {
            return 1;
        }
    }

    private static string GetDirection(float value, bool isHorizontal) {
        if (value == -1) {
            return isHorizontal ? "Left" : "Backwards";
        } else if (value == 0) {
            return string.Empty;
        } else {
            return isHorizontal ? "Right" : "Forwards";
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Respawn")) {
            GetComponent<CapsuleCollider>().enabled = false;
            Falling = true;
            dead = true;
            lastPosition = transform.position;
            animatorService.SetAnimator(Animators.Fall);
        } else if (collision.collider.CompareTag("Ground") && Ground != collision.collider.transform) {
            Ground = collision.collider.transform;
        }
    }
}
