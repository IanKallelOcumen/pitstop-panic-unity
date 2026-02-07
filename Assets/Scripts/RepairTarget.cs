using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Attach to a UI Image or empty GameObject over the car (e.g. tire, engine zone).
/// When the correct DraggableTool is dropped here, OnRepaired fires and you can swap to "fixed" car sprite.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class RepairTarget : MonoBehaviour
{
    [Header("Which tool fixes this part (must match DraggableTool.toolType)")]
    public string requiredToolType = "Wrench";

    [Header("State")]
    public bool isRepaired = false;
    [Tooltip("Optional: sprite to show when repaired (e.g. tire icon -> checkmark).")]
    public UnityEngine.UI.Image optionalRepairedImage;

    [Header("Events")]
    public UnityEvent OnRepaired = new UnityEvent();

    RepairGameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<RepairGameManager>();
    }

    public void TryRepairWith(DraggableTool tool)
    {
        if (isRepaired) return;
        if (tool.toolType != requiredToolType) return;

        isRepaired = true;
        if (optionalRepairedImage != null)
            optionalRepairedImage.enabled = true;
        OnRepaired?.Invoke();
        gameManager?.OnPartRepaired(this);
    }
}
