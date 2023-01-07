using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int State = Animator.StringToHash("state");
    private Animator _animator;
    
    private const int MoveState = 1;
    private const int IdleState = 0;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayShootAnimation()
    {
        _animator.SetTrigger(Shoot);
    }
    
    /// <summary>
    /// The animation event for the shoot animation that resets the shoot trigger
    /// </summary>
    public void ResetShootTrigger()
    {
        _animator.ResetTrigger(Shoot);
    }

    public void PlayMoveAnimation()
    {
        _animator.SetInteger(State, MoveState);
    }
    
    public void PlayIdleAnimation()
    {
        _animator.SetInteger(State, IdleState);
    }
}
