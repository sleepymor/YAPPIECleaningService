using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = PlayerConfig.c.MeleeDamage;
        if (other.CompareTag("Enemy"))
        {
            Enemy_Health enemyHealth = other.gameObject.GetComponent<Enemy_Health>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
