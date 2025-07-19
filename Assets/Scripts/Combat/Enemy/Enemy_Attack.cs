using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] public Transform attackArea;

    private void Start()
    {
        attackArea.gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.transform.root.GetComponent<PlayerController>();
            Debug.Log("Attacking Player!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.transform.root.GetComponent<PlayerController>();
            Debug.Log("Cant attack player anymore!");
            //enemyHealth.TakeDamage(damageAmount);
        }
    }
}
