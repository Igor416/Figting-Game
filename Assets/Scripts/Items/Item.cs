using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour, IStorable {
    public RuntimeAnimatorController itemController;

    public Sprite Sprite;
    public TextMesh Label;
    public int MaxRemained;
    public string Name { get; set; }

    private int hitsRemained;
    public int HitsRemained {
        get {
            return hitsRemained;
        }
        set {
            hitsRemained = value;
            if (hitsRemained <= 0) {
                Inventory.OnItemExpired(this);
                Destroy(gameObject, 0.5f);
            }
        } 
    }

    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();

        Label = Instantiate(Label.gameObject, transform).GetComponent<TextMesh>();
        Label.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        Throw();
    }

    protected virtual void Start() {
        HitsRemained = Random.Range(MaxRemained / 2, MaxRemained);
        Name = name.Replace("(Clone)", "");
    }

    public override abstract string ToString();

    public void PickUp() {
        GetComponent<SphereCollider>().enabled = false;
        Label.text = "";

        animator.runtimeAnimatorController = null;
        SetInHand();
    }

    public void Throw() {
        GetComponent<SphereCollider>().enabled = true;

        animator.runtimeAnimatorController = null;
        SetInGround();
    }

    public void Select() {
        gameObject.SetActive(true);
    }

    public void UnSelect() {
        gameObject.SetActive(false);
    }

    protected virtual void SetInHand() {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Label.transform.localPosition = new Vector3(0, 0, 0);
        Label.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    protected void SetInGround() {
        transform.localPosition = new Vector3(Random.Range(-3, 4), 0, Random.Range(-3, 4));
        transform.localRotation = Quaternion.Euler(90, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);

        Label.transform.localPosition = new Vector3(0, 0, -1);
        Label.transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Hero")) {
            animator.runtimeAnimatorController = null;
            Label.text = ToString();
            Inventory.OnItemInRange(this);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Hero")) {
            animator.runtimeAnimatorController = itemController;
            Label.text = "";
            Inventory.OnItemOutOfRange(this);
        }
    }
}
