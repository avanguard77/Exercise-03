using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float movementSpeed;
    private void Start()
    {
        Player.Instance.WakingToRunning += InstanceOnWakingToRunning;

        Player.Instance.Jumping += InstanceOnJumping;
        Player.Instance.HangingToLanding += InstanceOnHangingToLanding;

        animator = GetComponent<Animator>();
    }
    private void InstanceOnHangingToLanding(object sender, EventArgs e)
    {
        animator.SetBool("IsHanging", true);
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