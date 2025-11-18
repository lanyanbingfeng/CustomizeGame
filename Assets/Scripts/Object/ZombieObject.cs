
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieObject : MonoBehaviour
{
    public int hp;
    public int atk;
    public int def;
    [Header("僵直时间")]
    public int stunnedTime;

    [Header("是否寻找玩家")]
    public bool isFindPlayer;
    public int findRange;
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Transform _pursuitTarget; //追击目标

    private bool _isAtk;
    private bool _isStunned;

    private bool _isDead;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (!isFindPlayer) return;
        StartCoroutine(FindPursuitTarget());
    }

    private void Atk()
    {
        _animator.SetTrigger("Atk");
        _isAtk = true;
    }

    private void AtkEvent()
    {
        
    }

    private void AtkEndAnimatorEvent()
    {
        _isAtk = false;
        _isStunned = false;
    }

    // 受伤僵直
    public void Stunned(bool lightOrHeavy)
    {
        _animator.SetTrigger(lightOrHeavy ? "LightAtkStunned" : "HeavyAtkStunned");
        _isStunned = true;
        _isAtk = false;
        Invoke(nameof(AtkEndAnimatorEvent), stunnedTime);
        if (hp > 0) return;
        hp = 0;
        Dead();
    }

    private void Dead()
    {
        _animator.SetTrigger("Dead");
        _isDead = true;
    }

    private IEnumerator FindPursuitTarget()
    {
        while (_pursuitTarget == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, findRange, 1 << LayerMask.NameToLayer("Player"));
            if (colliders.Length > 0)
            {
                _pursuitTarget = colliders[0].transform;
                StartCoroutine(FindPlayerPosition());
            }
            yield return new  WaitForSeconds(1f);
        }
    }

    private IEnumerator FindPlayerPosition()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(0.2f);
            if (_isStunned) continue;
            _navMeshAgent.SetDestination(_pursuitTarget.position);
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                // 到达玩家位置
                _animator.SetBool("Run",false);
                // 发动攻击
                if (!_isAtk && !_isStunned) Atk();
            }
            else
            {
                // 没有追上玩家
                _animator.SetBool("Run",true);
            }
        }
    }

    private void OnDestroy()
    {
        _isDead = true;
    }
}
