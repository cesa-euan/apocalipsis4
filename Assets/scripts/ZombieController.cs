using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public float lookRadius = 10f; // Radio de detección del jugador
    public Animator animator;
    public int maxHealth = 100;
    private int currentHealth;
    Transform target;
    NavMeshAgent agent;
    bool isDead = false;
    bool isAttacking = false;
    public float attackCooldown = 2f; // Tiempo entre ataques
    private float nextAttackTime = 0f;
    public float hitReactionTime = 0.5f; // Tiempo de reacción al impacto
    private bool isReactingToHit = false; // Bandera para evitar otros comportamientos mientras reacciona

    // Variables para patrullaje
    public Transform[] patrolPoints; // Puntos de patrullaje
    private int currentPatrolIndex = 0;
    public float waitTimeAtPatrolPoint = 2f; // Tiempo que el zombi espera en cada punto
    private float patrolWaitTimer = 0f;

    void Start()
    {
        target = FPSController.instance.transform; // El objetivo es el transform del jugador
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        // Si hay puntos de patrullaje, envía al zombi al primer punto
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (!isDead && !isReactingToHit) // Si el zombi está vivo y no está reaccionando al hit
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius) // Si el jugador está dentro del radio de detección
            {
                agent.SetDestination(target.position); // Seguir al jugador
                animator.SetBool("isWalking", true); // Activa la animación de caminar

                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();

                    // Si el zombi está dentro del radio de ataque, realiza un ataque
                    if (!isAttacking && Time.time >= nextAttackTime)
                    {
                        Attack();
                    }
                }
            }
            else
            {
                Patrol(); // Si el jugador está fuera del rango, patrulla
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return; // Si no hay puntos de patrullaje, no hacer nada

        // Si el zombi está cerca del punto de patrullaje actual
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= waitTimeAtPatrolPoint)
            {
                // Mover al siguiente punto de patrullaje
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolWaitTimer = 0f; // Reiniciar el temporizador de espera
            }
        }
        else
        {
            animator.SetBool("isWalking", true); // Si está caminando entre puntos, activa la animación de caminar
        }
    }

    void Attack()
    {
        // Evita que el zombi siga moviéndose cuando ataca
        agent.isStopped = true;
        animator.SetTrigger("Attack"); // Activa la animación de ataque
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown; // Reinicia el cooldown del ataque
    }

    public void EndAttack()
    {
        isAttacking = false;
        agent.isStopped = false; // Reanuda el movimiento del zombi
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Si el zombi no ha muerto, reproduce la animación de impacto
        if (currentHealth > 0)
        {
            StartCoroutine(ReactToHit()); // Detiene el movimiento y reproduce la animación de impacto
        }
        else
        {
            Die(); // Llama al método Die si la salud es 0 o menor
        }
    }

    IEnumerator ReactToHit()
    {
        isReactingToHit = true; // Indica que el zombi está reaccionando al golpe
        agent.isStopped = true; // Detiene el movimiento del zombi
        animator.SetTrigger("Hit"); // Activa la animación de impacto

        yield return new WaitForSeconds(hitReactionTime); // Espera a que termine la animación de impacto

        agent.isStopped = false; // Reanuda el movimiento después del impacto
        isReactingToHit = false; // Permite que el zombi vuelva a moverse normalmente
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true; // Detén al agente de navegación
        animator.SetTrigger("Die"); // Activa la animación de morir
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
