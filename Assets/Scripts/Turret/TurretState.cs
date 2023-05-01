using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State pattern
public abstract class TurretState
{
    protected Turret turret;

    public TurretState(Turret turret)
    {
        this.turret = turret;
    }

    public abstract void UpdateState();
}

public class IdleState : TurretState
{
    public IdleState(Turret turret) : base(turret) { }

    public override void UpdateState()
    {
        if (turret.TargetInRange())
        {
            turret.ChangeState(new AttackState(turret));
        }
    }
}

public class AttackState : TurretState
{
    public AttackState(Turret turret) : base(turret) { }

    public override void UpdateState()
    {
        if (!turret.TargetInRange())
        {
            turret.ChangeState(new IdleState(turret));
        }
        else
        {
            //Aim
            turret.Shoot();
        }
    }
}