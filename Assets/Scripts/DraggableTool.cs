using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to a UI Image (tool icon). Lets the player drag it; use with RepairTarget for drop zones.
/// Assign toolType to match RepairTarget's requiredToolType (e.g. "Tire" -> Jack/Wrench).
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class DraggableTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tool identity - must match RepairTarget.requiredToolType")]
    public string toolType = "Wrench";

    [Header("Optional")]
    [Tooltip("If set, this tool is hidden when not the current objective (e.g. only show correct tools per level).")]
    public bool hideWhenNotRequired = false;

    RectTransform rectTransform;
    Canvas canvas;
    CanvasGroup canvasGroup;
    Vector2 startAnchoredPos;
    Transform startParent;
    int startSiblingIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (!TryGetComponent(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        startSiblingIndex = transform.GetSiblingIndex();
        startAnchoredPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 local);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, null, out Vector2 local);
        rectTransform.localPosition = new Vector3(local.x, local.y, rectTransform.localPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if dropped on a RepairTarget
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            var target = eventData.pointerCurrentRaycast.gameObject.GetComponent<RepairTarget>();
            if (target != null)
            {
                target.TryRepairWith(this);
                ReturnToStart();
                return;
            }
        }

        ReturnToStart();
    }

    void ReturnToStart()
    {
        transform.SetParent(startParent, false);
        transform.SetSiblingIndex(startSiblingIndex);
        rectTransform.anchoredPosition = startAnchoredPos;
    }

    public void SetVisible(bool visible)
    {
        if (TryGetComponent<Image>(out var img))
            img.enabled = visible;
        if (TryGetComponent<CanvasGroup>(out var cg))
            cg.blocksRaycasts = visible;
    }
}
