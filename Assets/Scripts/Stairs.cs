using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Trigger
{
    [SerializeField] Collider2D collisionCollider;
    SpriteRenderer spriteRenderer;
    [SerializeField] float startAlpha = 1;
    [SerializeField] float endAlpha = 0.1f;
    [SerializeField] float maxFadeDistance = 3;
    List<Collider2D> targets = new List<Collider2D>();

    private void Start()
    {
        spriteRenderer = collisionCollider.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if(targets.Count > 0)
        {
            float highestY = targets[0].transform.position.y;
            for(int i = 1; i < targets.Count; i++)
            {
                if (targets[i].transform.position.y > highestY)
                {
                    highestY = targets[i].transform.position.y;
                }
            }
            // Assuming transform position is center
            Color color = spriteRenderer.color;
            float t = 1 - Mathf.Clamp01((collisionCollider.transform.position.y - highestY) / maxFadeDistance);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            spriteRenderer.color = color;
        }
    }

    public override void TriggerEnter(Collider2D collider)
    {
        base.TriggerEnter(collider);
        if(collider.transform.position.y < collisionCollider.transform.position.y)
        {
            targets.Add(collider);
            Physics2D.IgnoreCollision(collisionCollider, collider, true);
        }
        else if(collider.TryGetComponent(out BaseMovement baseMovement))
        {
            baseMovement.stairs = this;
        }
    }

    public override void TriggerExit(Collider2D collider)
    {
        base.TriggerExit(collider);
        Physics2D.IgnoreCollision(collisionCollider, collider, false);
        targets.Remove(collider);
        if(targets.Count == 0)
        {
            Color color = spriteRenderer.color;
            color.a = startAlpha;
            spriteRenderer.color = color;
        }
    }

    public void Descend(Collider2D otherCollider)
    {
        Physics2D.IgnoreCollision(collisionCollider, otherCollider, true);
        if (!targets.Contains(otherCollider))
        {
            targets.Add(otherCollider);
        }
    }
}