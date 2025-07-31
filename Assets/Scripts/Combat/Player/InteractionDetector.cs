using UnityEngine;
using UnityEngine.UI;

public class InteractionDetector : MonoBehaviour
{
    private InteractableInterface interactableInterface = null;

    [SerializeField] private GameObject interactionCanvas;
    [SerializeField] private Slider interactionSlider;

    private float holdTime = 0f;
    private float requiredHoldTime = 1f;
    private bool hasInteracted = false;

    [SerializeField] private float cooldownDuration = 1f;
    private float cooldownTimer = 0f;

    private void Start()
    {
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        if (interactionSlider != null)
            interactionSlider.value = 0f;
    }

    private void Update()
    {
        // Reduce cooldown if active
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            if (interactionSlider != null)
                interactionSlider.value = 1f - (cooldownTimer / cooldownDuration);

            return;
        }

        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (interactableInterface == null || PlayerController.instance == null)
        {
            ResetInteraction();
            return;
        }

        var interactAction = PlayerController.instance.controller.Movement.Interract;

        if (interactAction.IsPressed())
        {
            holdTime += Time.deltaTime;

            if (interactionSlider != null)
                interactionSlider.value = holdTime / requiredHoldTime;

            if (holdTime >= requiredHoldTime && !hasInteracted)
            {
                interactableInterface.Interract();
                hasInteracted = true;

                // Start cooldown
                cooldownTimer = cooldownDuration;

                // Reset UI
                ResetInteraction();
                if (interactionCanvas != null)
                    interactionCanvas.SetActive(false);
            }
        }
        else
        {
            ResetInteraction();
        }
    }

    private void ResetInteraction()
    {
        holdTime = 0f;
        hasInteracted = false;

        if (interactionSlider != null)
            interactionSlider.value = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableInterface interactable) && interactable.CanInterract())
        {
            interactableInterface = interactable;

            // Only show interaction UI if not on cooldown
            if (cooldownTimer <= 0f && interactionCanvas != null)
                interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableInterface interactable) && interactable == interactableInterface)
        {
            interactableInterface = null;
            ResetInteraction();

            if (interactionCanvas != null)
                interactionCanvas.SetActive(false);
        }
    }
}
