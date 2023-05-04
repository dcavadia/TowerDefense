using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretSelectionButton : MonoBehaviour, IPointerClickHandler
{
    public TurretData turretData;

    public void OnPointerClick(PointerEventData eventData)
    {
        TurretPlacementManager.Instance.SelectTurret(turretData);
    }
}
