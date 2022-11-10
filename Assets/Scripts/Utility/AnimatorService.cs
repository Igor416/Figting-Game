using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimatorService {
    readonly RuntimeAnimatorController[] animatorControllers;
    readonly Animator animator;

    public AnimatorService(RuntimeAnimatorController[] animatorControllers, Animator animator) {
        this.animatorControllers = animatorControllers;
        this.animator = animator;
    }

    private RuntimeAnimatorController GetAnimator<T>(T animator) {
        return animatorControllers[Convert.ToInt32(animator)];
    }

    public bool SetAnimator<T>(T animator) {
        this.animator.speed = 1;
        if (this.animator.runtimeAnimatorController != GetAnimator(animator)) {
            this.animator.runtimeAnimatorController = GetAnimator(animator);
            return true;
        }
        return false;
    }

    public void SetSpeed(float speed) {
        animator.speed = speed;
    }

    public void Play(string animationName) {
        animator.Play(animationName);
    }
}
