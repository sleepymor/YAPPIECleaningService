using UnityEngine;
using UnityEngine.InputSystem;

public class MissionPanelToggle : MonoBehaviour
{
    public CanvasGroup panelGroup;
    private bool isVisible = false;

    private void OnEnable()
    {
        PlayerController.instance.controller.Movement.ToggleMission.performed += OnTogglePanel;
    }

    private void OnDisable()
    {
        PlayerController.instance.controller.Movement.ToggleMission.performed -= OnTogglePanel;
    }

    private void OnTogglePanel(InputAction.CallbackContext ctx)
    {
        isVisible = !isVisible;
        panelGroup.alpha = isVisible ? 1f : 0f;
        panelGroup.interactable = isVisible;
        panelGroup.blocksRaycasts = isVisible;
    }
}
