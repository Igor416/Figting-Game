using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviour : MonoBehaviour {
    public float HP { get; set; }
    public float HPRegen { get; set; }
    public Shield Shield { get; set; }

    public static event ItemEvent ShieldObtained;
    public static event ItemEvent ShieldLost;

    public static void OnShieldObtained(Shield shield) {
        ShieldObtained?.Invoke(shield);
    }
    public static void OnShieldLost(Shield shield) {
        ShieldLost?.Invoke(shield);
    }

    private float maxHP;
    private float maxArmour;
    private BarController HealthBar;
    private BarController ArmourBar;

    private HeroMovementController movementController;

    private IEnumerator RegenerateHP() {
        while (true) {
            if (HP + HPRegen <= maxHP) {
                HP += HPRegen;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator RegenerateArmour() {
        while (true) {
            if (!(Shield is null)) {
                if (Shield.Armour + 1 <= maxArmour) {
                    Shield.Armour += 1;
                }
                yield return new WaitForSeconds(Shield.ArmourRegen);
            } else {
                yield return new WaitForSeconds(1);
            }
        }
    }

    void Awake() {
        HealthBar = GameObject.Find("Health").GetComponent<BarController>();
        ArmourBar = GameObject.Find("Armour").GetComponent<BarController>();

        movementController = GetComponent<HeroMovementController>();
    }

    void Start() {
        maxHP = 100;
        maxArmour = 10;
        HP = maxHP;
        HPRegen = 0.5f;

        HealthBar.SetText("+" + HPRegen);
        ArmourBar.Disable();

        ShieldObtained = (Item shield) => {
            Shield = shield as Shield;
            ArmourBar.Enable();
            ArmourBar.SetText("" + this.Shield.Armour);
        };
        ShieldLost = (Item shield) => {
            Shield = null;
            ArmourBar.Disable();
        };

        StartCoroutine(RegenerateHP());
        StartCoroutine(RegenerateArmour());
    }

    void Update() {
        HealthBar.UpdateBar(HP / maxHP);

        if (ArmourBar.IsEnabled()) {
            ArmourBar.UpdateBar(Shield.Armour / maxArmour);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("EnemyWeapon")) {
            if (Shield is null || !movementController.Blocking || Shield.Blocked) {
                HP -= 6;
                if (!(Shield is null)) {
                    Shield.Blocked = false;
                    HP += Shield.Block;
                    Shield.HitsRemained--;
                }

                if (HP <= 0) {
                    movementController.Damage = false;
                    movementController.Dead = true;
                } else {
                    movementController.Damage = true;
                }
            }
        }
    }
}
