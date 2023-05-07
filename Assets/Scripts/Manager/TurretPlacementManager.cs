using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurretPlacementManager : SingletonComponent<TurretPlacementManager>
{
    [SerializeField] private LayerMask placementLayerMask;

    private GameObject turretGhost;
    private Turret selectedTurretType;
    private TurretData selectedTurretData;
    private bool isPlacingTurret;

    private Dictionary<Type, TurretFactory> turretFactories = new Dictionary<Type, TurretFactory>();

    private void Start()
    {
        turretFactories.Add(typeof(TurretRegular), new TurretRegularFactory());
        turretFactories.Add(typeof(TurretFreezer), new TurretFreezerFactory());
    }

    public void SelectTurret(TurretData turretData)
    {
        if (!EconomyManager.Instance.CanAffordTurret(turretData.Cost))
            return;

        isPlacingTurret = true;

        // Create the ghost turret object
        turretGhost = Instantiate(turretData.Prefab);

        selectedTurretData = turretData;
        selectedTurretType = turretGhost.GetComponent<Turret>();
    }

    private void Update()
    {
        if (isPlacingTurret)
        {
            // Cast a ray from the top-down camera to determine the position of the ghost turret
            Ray ray = UIManager.Instance.topDownCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayerMask))
            {
                // Move the ghost turret to the position of the raycast hit
                turretGhost.transform.position = new Vector3(hit.point.x , 0f, hit.point.z);
            }

            if (Input.GetMouseButtonDown(0))
            {
                // Raycast to see if the turret can be placed at the clicked location
                if (Physics.Raycast(ray, out RaycastHit placementHit, Mathf.Infinity, placementLayerMask))
                {
                    //Purchase
                    EconomyManager.Instance.PurchaseTurret(selectedTurretData.Cost);

                    // Create a new turret at the clicked location
                    Vector3 turretPosition = new Vector3(placementHit.point.x, 0f, placementHit.point.z);
                    Creep target = null; // set the target to null, since there's no enemy at the moment
                    float range = selectedTurretData.Range;

                    // Get the correct factory for the selected turret type and create the turret
                    TurretFactory factory = turretFactories[selectedTurretType.GetType()];
                    Turret newTurret = factory.CreateTurret(turretPosition, target, range, selectedTurretData);

                    // Destroy the ghost turret object
                    Destroy(turretGhost);

                    // Reset the selected turret type, data and stop placing turrets
                    selectedTurretType = null;
                    selectedTurretData = null;
                    isPlacingTurret = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (turretGhost != null && selectedTurretData != null)
        {
            // Draw a wire disc with the range of the ghost turret
            Handles.color = new Color(1f, 0f, 0f, 1f);
            Handles.DrawWireDisc(turretGhost.transform.position, Vector3.up, selectedTurretData.Range);
        }
    }
}
    

