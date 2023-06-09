using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract Factory Method pattern: The responsibility of creating objects its delegated to the factory, and the factory creates objects of a particular family.
//Provides a way to create objects that are related to each other without specifying their concrete classes.
//Provide more flexibility in creating related objects and more decoupling in comparision with an abstract class.
public abstract class TurretFactory
{
    protected Turret CreateTurretInternal(Vector3 position, TurretData turretData)
    {
        GameObject turretGameObject = Object.Instantiate(turretData.Prefab);
        turretGameObject.transform.position = position;
        var turret = turretGameObject.GetComponent<Turret>();
        turret.Initialize(turretData);
        return turret;
    }

    public abstract Turret CreateTurret(Vector3 position, TurretData turretData);
}

public class TurretRegularFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, TurretData turretData)
    {
        return CreateTurretInternal(position, turretData);
    }
}

public class TurretFreezerFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, TurretData turretData)
    {
        return CreateTurretInternal(position, turretData);
    }
}

public class TurretFireFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, TurretData turretData)
    {
        GameObject turretGameObject = Object.Instantiate(turretData.Prefab);
        turretGameObject.transform.position = position;
        var turret = turretGameObject.GetComponent<Turret>();
        turret.Initialize(turretData);
        //Use some sfx?
        return turret;
    }
}