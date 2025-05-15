using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float movementSpeed;
    private int velocityHash;

    private void Start()
    {
        Player.Instance.WakingToRunning += InstanceOnWakingToRunning;
        Player.Instance.Jumping += InstanceOnJumping;
        Player.Instance.HangingToLanding += InstanceOnHangingToLanding;
        Player.Instance.JumpingToHanging+= InstanceOnJumpingToHanging;
        
        animator = GetComponent<Animator>();
    }

    private void InstanceOnJumpingToHanging(object sender, EventArgs e)
    {
        animator.SetBool("Islanding", true);
    }

    private void InstanceOnHangingToLanding(object sender, EventArgs e)
    {
        animator.SetBool("IsJumping", false);
    }

    private void InstanceOnJumping(object sender, EventArgs e)
    {
        animator.Play("Jumping Up");
    }

    private void InstanceOnWakingToRunning(object sender, Player.WakingToRunningEventArges e)
    {
        movementSpeed = e.playerSpeed;
    }

    private void Update()
    {
        animator.SetFloat("Speed", movementSpeed);
    }
}