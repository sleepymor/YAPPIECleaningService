using UnityEngine;

public class Enemy_Aoe : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1f;

    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox" && Time.time - lastDamageTime >= damageInterval)
        {
            lastDamageTime = Time.time;

            if (PlayerCombat.instance != null)
            {
                PlayerCombat.instance.TakeDamage(damage, transform);
            }
        }
    }
}
