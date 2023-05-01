using UnityEngine;

//Factory Method pattern
//TurretFactory defines a factory method called CreateTurret.
public abstract class TurretFactory
{
    public abstract Turret CreateTurret(Vector3 position, Transform target, float range);
}
//BasicTurretFactory as a concrete class extends TurretFactory and overrides the CreateTurret method to return a new instance of BasicTurret.
public class BasicTurretFactory : TurretFactory
{
    public override Turret CreateTurret(Vector3 position, Transform target, float range)
    {
        return new BasicTurret(position, target, range);
    }
}

public abstract class Turret
{
    protected Transform target;
    protected float range;
    protected TurretState state;
    protected Vector3 currentPosition;

    public Turret(Vector3 position, Transform target, float range)
    {
        this.target = target;
        this.range = range;
        state = new IdleState(this);
        currentPosition = position;
    }

    public void Update()
    {
        state.UpdateState();
    }

    public void ChangeState(TurretState state)
    {
        this.state = state;
    }

    public abstract void Shoot();

    public bool TargetInRange()
    {
        float distanceToTarget = Vector3.Distance(currentPosition, target.position);
        return distanceToTarget <= range;
    }
}