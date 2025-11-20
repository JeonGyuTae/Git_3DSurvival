using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);

    void TakeDamage(int damage, Vector3 hitPosition);
}
