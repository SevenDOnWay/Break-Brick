using UnityEngine;

public abstract class Process {

    public abstract ProcessType GetProssType();

    public abstract int OnHit(StatManager statManager, Vector2 pos);

    public virtual void OnApply() { }


}
