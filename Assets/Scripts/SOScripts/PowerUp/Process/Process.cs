using UnityEngine;

public abstract class Process {

    public abstract ProcessType GetProssType();

    public abstract int OnHit(StatManager statManager);

    public virtual void OnApply() { }


}
