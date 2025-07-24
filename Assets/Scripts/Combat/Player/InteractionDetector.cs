using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private InteractableInterface interactableInterface = null;
    [SerializeField] private GameObject interactionCanvas;

    private float holdTime = 0f;
    private float requiredHoldTime = 0.5f;
    private bool hasInteracted = false;

    private void Start()
    {
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);
    }

    private void Update()
    {
        if (interactableInterface != null && PlayerController.instance != null)
        {
            var interractAction = PlayerController.instance.controller.Movement.Interract;

            if (interractAction.IsPressed())
            {
                holdTime += Time.deltaTime;

                if (holdTime >= requiredHoldTime && !hasInteracted)
                {
                    interactableInterface.Interract();
                    hasInteracted = true;
                }
            }
            else
            {
                // Reset if released early or done
                holdTime = 0f;
                hasInteracted = false;
            }
        }
        else
        {
            holdTime = 0f;
            hasInteracted = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableInterface interactable) && interactable.CanInterract())
        {
            interactableInterface = interactable;
            if (interactionCanvas != null)
                interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableInterface interactable) && interactable == interactableInterface)
        {
            interactableInterface = null;
            holdTime = 0f;
            hasInteracted = false;

            if (interactionCanvas != null)
                interactionCanvas.SetActive(false);
        }
    }
}
