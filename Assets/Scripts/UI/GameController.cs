using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void GameEvent();

public class GameController : MonoBehaviour {
    public TextMeshProUGUI ScoreLabel;
    public TextMeshProUGUI GameOverScoreLabel;

    Animator animator;

    int score = 0;

    public static event GameEvent Died;
    public static void OnDied() {
        Died?.Invoke();
    }

    public static event GameEvent Scored;
    public static void OnScored() {
        Scored?.Invoke();
    }

    void Start() {
        animator = GetComponent<Animator>();

        Died = ShowGameOver;
        Scored = () => {
            score++;
            ScoreLabel.text = "Score: " + score;
        };
    }

    private void ShowGameOver() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        GameOverScoreLabel.text = ScoreLabel.text;
        animator.Play("GameOver");
    }
}
