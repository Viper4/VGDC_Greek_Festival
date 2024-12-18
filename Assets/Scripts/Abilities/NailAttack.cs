using UnityEngine;

public class NailAttack : Ability
{
    private AudioSource audioSource;
    [SerializeField] private Collider2D hitbox;

    private Collider2D[] hits = new Collider2D[10];

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Player.instance.input.Player.Nail.performed += OnInput;
    }

    private void OnDisable()
    {
        Player.instance.input.Player.Nail.performed -= OnInput;
    }

    public override void Use()
    {
        audioSource.PlayOneShot(audioSource.clip);
        ContactFilter2D Filter = new ContactFilter2D() { layerMask = hitLayers, useLayerMask = true };
        int numHits = hitbox.OverlapCollider(Filter, hits);
        for (int i = 0; i < numHits; i++)
        {
            if (hits[i].TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.AddHealth(-damage, 0, true);
                Player.instance.CheckKill(healthSystem);
            }
            if (hits[i].TryGetComponent(out BaseMovement baseMovement))
            {
                BaseMovement.KnockbackInfo knockbackInfo = knockback;
                knockbackInfo.velocity = hitbox.transform.right * knockbackInfo.velocity.x + hitbox.transform.up * knockbackInfo.velocity.y;
                baseMovement.ApplyKnockback(knockbackInfo, true);
            }
        }

        if (numHits > 0)
        {
            Vector2 direction = -(hitbox.transform.position - transform.position).normalized;
            direction.y = 0;
            owner.ApplyKnockback(new BaseMovement.KnockbackInfo(direction * recoilKnockback.velocity, recoilKnockback.duration, recoilKnockback.drag), true);
        }
    }
}
