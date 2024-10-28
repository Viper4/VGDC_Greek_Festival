using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseMovement
{
    enum TargetMode
    {
        Closest,
        Farthest,
        HighestHealth,
        LowestHealth,
        Random,
    }
    [Header("Enemy")]
    [SerializeField] TargetMode targetMode = TargetMode.Closest;

    public float damage = 1f;
    public float attackCooldown = 1f;
    [HideInInspector] public float attackTimer = 0f;
    [HideInInspector] public List<Transform> targets = new List<Transform>();
    [HideInInspector] public Transform selectedTarget = null;

    private void Update()
    {
        MovementUpdate();
        if (targets.Count > 0)
        {
            if(selectedTarget == null)
                SelectTarget();

            RaycastHit2D targetHit = Physics2D.Linecast(transform.position, selectedTarget.position, collisionLayers);
            if(targetHit.transform == selectedTarget)
            {
                moveVelocity = selectedTarget.position - transform.position;
                moveVelocity.y = 0f;
                moveVelocity = moveVelocity.normalized * runSpeed;
            }
            else
            {
                moveVelocity = Vector2.zero;
            }
            ApplyVelocity(new Vector2(moveVelocity.x, rb.velocity.y));
        }
    }

    public void SelectTarget()
    {
        selectedTarget = targets[0];
        switch (targetMode)
        {
            case TargetMode.Closest:
                for (int i = 1; i < targets.Count; i++)
                {
                    if ((targets[i].position - transform.position).sqrMagnitude < (selectedTarget.position - transform.position).sqrMagnitude)
                    {
                        selectedTarget = targets[i];
                    }
                }
                break;
            case TargetMode.Farthest:
                for (int i = 1; i < targets.Count; i++)
                {
                    if ((targets[i].position - transform.position).sqrMagnitude > (selectedTarget.position - transform.position).sqrMagnitude)
                    {
                        selectedTarget = targets[i];
                    }
                }
                break;
            case TargetMode.HighestHealth:
                HealthSystem selectedHealthSystem = targets[0].GetComponent<HealthSystem>();
                for (int i = 1; i < targets.Count; i++)
                {
                    HealthSystem currentHealthSystem = targets[i].GetComponent<HealthSystem>();
                    if (currentHealthSystem.health > selectedHealthSystem.health)
                    {
                        selectedTarget = targets[i];
                        selectedHealthSystem = currentHealthSystem;
                    }
                }
                break;
            case TargetMode.LowestHealth:
                selectedHealthSystem = targets[0].GetComponent<HealthSystem>();
                for (int i = 1; i < targets.Count; i++)
                {
                    HealthSystem currentHealthSystem = targets[i].GetComponent<HealthSystem>();
                    if (currentHealthSystem.health > selectedHealthSystem.health)
                    {
                        selectedTarget = targets[i];
                        selectedHealthSystem = currentHealthSystem;
                    }
                }
                break;
            case TargetMode.Random:
                selectedTarget = targets[Random.Range(0, targets.Count)];
                break;
        }
    }

    public void OnTargetTriggerEnter(Collider2D targetCollider)
    {
        if(!targets.Contains(targetCollider.transform))
            targets.Add(targetCollider.transform);
    }

    public void OnTargetTriggerExit(Collider2D targetCollider)
    {
        targets.Remove(targetCollider.transform);
    }
}