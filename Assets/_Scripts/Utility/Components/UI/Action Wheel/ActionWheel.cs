using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWheel : MonoBehaviour
{
    public GameObject Current { get; private set; }

    [Tooltip("Rect used as reference for mouse input. Usually Canvas.")]
    [SerializeField] private RectTransform referenceRectTransform;
    [SerializeField] private bool center = true;
    [SerializeField] private List<GameObject> options;
    [SerializeField] private float minimumDistance = 50f;

    [Space]
    [SerializeField] private ArticulationDriveAxis gizmoAxis;

    private Vector2 _resolution => referenceRectTransform.rect.size;

    private Vector2 _cursorPosition;
    private IInventorySlotSelection _oldSelection;
    private IInventorySlotSelection _currentSelection;

    private void Update()
    {
        HandleMouse();
        SelectItem();
    }

    private void HandleMouse()
    {
        _cursorPosition.x = Input.mousePosition.x / Screen.width;
        _cursorPosition.y = Input.mousePosition.y / Screen.height;
        
        _cursorPosition *= _resolution;

        if (center)
        {
            _cursorPosition -= _resolution / 2;
        }
    }

    private void SelectItem()
    {
        _oldSelection = _currentSelection;

        GameObject closestItem = null;
        float closestDistance = float.MaxValue;

        if (_cursorPosition.magnitude >= minimumDistance)
        {
            foreach (var item in options)
            {
                var distance = Vector3.Distance(item.transform.localPosition, _cursorPosition);

                var isCloser = distance < closestDistance;
                closestDistance = isCloser ? distance : closestDistance;
                closestItem = isCloser ? item : closestItem;
            }
        }

        Current = closestItem;

        _oldSelection?.OnDeselect(null);

        if (Current != null)
        {
            if (Current.TryGetComponent(out _currentSelection))
            {
                _currentSelection.OnSelect(null);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float scale = 0f;
        switch (gizmoAxis)
        {
            default:
            case ArticulationDriveAxis.X:
                scale = transform.lossyScale.x;
                break;
            case ArticulationDriveAxis.Y:
                scale = transform.lossyScale.y;
                break;
            case ArticulationDriveAxis.Z:
                scale = transform.lossyScale.z;
                break;
        }

        GizmosExtention.DrawWireCircle(transform.position, minimumDistance * scale);
    }
}
