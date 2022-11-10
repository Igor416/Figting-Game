using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public Image Bar;
    public TextMesh Text;

    public bool IsEnabled() {
        return gameObject.activeSelf;
    }

    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void SetText(string text) {
        Text.text = text;
    }

    public void UpdateBar(float value) {
        Bar.fillAmount = value;
    }
}
