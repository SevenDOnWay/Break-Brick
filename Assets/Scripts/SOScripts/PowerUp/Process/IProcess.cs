using UnityEngine;

public interface IProcess {

    ProcessType GetProssType();

    int OnHit(StatManager statManager, BrickScript brick);

    void OnApply();
}
