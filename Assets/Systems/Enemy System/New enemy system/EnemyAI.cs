using UnityEngine;

public abstract class EnemyAI : MonoBehaviour 
{
    public float maxHealth = 100f;
    public float currentHealth;

    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float detectionRange = 10f;

    [SerializeField]
    private EnemyAI target;
    public EnemyStates currentState;
    [SerializeField]
    private Transform playerTransform;
    


    
}

public enum EnemyStates
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Flee
}