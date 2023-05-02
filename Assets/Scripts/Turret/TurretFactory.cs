using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract Factory Method pattern: The responsibility of creating objects its delegated to the factory, and the factory creates objects of a particular family.
//Provides a way to create objects that are related to each other without specifying their concrete classes.
//Provide more flexibility in creating related objects and more decoupling in comparision with an abstract class.
public abstract class TurretFactory
{
    public abstract Turret CreateTurret(Vector3 position, Transform target, float range, TurretData turretData);
}

public class TurretRegularFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, Transform target, float range, TurretData turretData)
    {
        GameObject turretGameObject = Object.Instantiate(turretData.prefab);
        turretGameObject.transform.position = position;
        var turret = turretGameObject.GetComponent<Turret>();
        turret.Initialize(position, target, range, turretData);
        return turret;
    }
}

public class TurretFreezerFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, Transform target, float range, TurretData turretData)
    {
        GameObject turretGameObject = Object.Instantiate(turretData.prefab);
        turretGameObject.transform.position = position;
        var turret = turretGameObject.GetComponent<Turret>();
        turret.Initialize(position, target, range, turretData);
        return turret;
    }
}