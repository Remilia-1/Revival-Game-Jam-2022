using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;



    private void Update()
    {
        UpdateVelocity();    
    }

    private void UpdateVelocity()
    {
        var velocity = characterController.velocity.magnitude;

        animator.SetFloat("Velocity", velocity);
    }
}
