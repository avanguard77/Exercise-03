using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float movementSpeed;
    private int velocityHash;
    private bool switchMode = true;

    private void Start()
    {
        Player.Instance.WakingToRunning += InstanceOnWakingToRunning;
        Player.Instance.SwitchRunnnigToFlying += InstanceOnSwitchRunnnigToFlying;

        animator = GetComponent<Animator>();
    }

    private void InstanceOnSwitchRunnnigToFlying(object sender, EventArgs e)
    {
        switchMode = !switchMode;
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