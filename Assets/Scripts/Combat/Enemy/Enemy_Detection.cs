using UnityEngine;

public class Enemy_Detection : MonoBehaviour
{
    [SerializeField] public Transform detectionArea;

    private void Start()
    {
        detectionArea.gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.transform.root.GetComponent<PlayerController>();
            Debug.Log("Player Detected!");
            //enemyHealth.TakeDamage(damageAmount);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.transform.root.GetComponent<PlayerController>();
            Debug.Log("Player Exited!");
            //enemyHealth.TakeDamage(damageAmount);
        }
    }
}
