using UnityEngine;

public class FloatingCityScript : MonoBehaviour
{
    public static float FloatingCityEnvHealth = 0;
    [SerializeField] public Transform floatCityArea;

    public void Awake()
    {
        floatCityArea.gameObject.SetActive(true);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "Hitbox")
        {
            FloatingCityEnvHealth = EnvironmentManager.Instance.GetEnvironmentProgress(EnemyEnvironment.FloatingCity);
            EnvironmentBar.instance.SetArrowPosition(FloatingCityEnvHealth);
            Debug.Log(FloatingCityEnvHealth);
        }
    }
}
