using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Detecting");
        if (other.gameObject.GetComponent<Enemy_Health>())
        {
            Enemy_Health enemyHealth = other.gameObject.GetComponent<Enemy_Health>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
