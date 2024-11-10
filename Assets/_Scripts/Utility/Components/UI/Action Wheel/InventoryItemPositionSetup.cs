using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemPositionSetup : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    [SerializeField] private float circleRadius = 200;
    [SerializeField] private float startingAngle = 0;
    [SerializeField] private bool antiHoraire;

    private void OnValidate()
    {
        if (items.Count == 0)
            return;

        float angle = 360 / items.Count;
        float currentAngle = startingAngle;

        foreach (var item in items)
        {
            if (item == null)
                continue;

            Vector3 position = Quaternion.Euler(0, 0, currentAngle) * new Vector3(0, circleRadius, 0);
            item.transform.localPosition = position;

            if (antiHoraire)
                currentAngle += angle;
            else
                currentAngle -= angle;
        }
    }
}
