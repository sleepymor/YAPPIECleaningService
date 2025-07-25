using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = PlayerConfig.c.MeleeDamage;
        if (other.gameObject.GetComponent<Enemy_Health>())
        {
            Enemy_Health enemyHealth = other.gameObject.GetComponent<Enemy_Health>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
