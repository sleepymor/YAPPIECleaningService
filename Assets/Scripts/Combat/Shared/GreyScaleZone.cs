using UnityEngine;

public class GreyscaleZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ForceGrayscale greyscale = other.GetComponent<ForceGrayscale>();
        if (greyscale != null)
        {
            greyscale.EnableGreyscale();
            Debug.Log("Grayscaled");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ForceGrayscale greyscale = other.GetComponent<ForceGrayscale>();
        if (greyscale != null)
        {
            greyscale.DisableGreyscale();
        }
    }
}
