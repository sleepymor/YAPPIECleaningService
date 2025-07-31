using UnityEngine;

public class BeachScript : MonoBehaviour
{
    public static float BeachEnvHealth = 100;
    [SerializeField] public Transform beachArea;

    public void Awake()
    {
        beachArea.gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            BeachEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.Beach);
            EnvironmentBar.instance.SetArrowPosition(BeachEnvHealth);
            //Debug.Log(BeachEnvHealth);
        }
    }
}
