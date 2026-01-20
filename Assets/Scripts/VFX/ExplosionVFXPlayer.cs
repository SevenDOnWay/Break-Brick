using UnityEngine;

public class ExplosionVFXPlayer : IVFXPlayerBase, IVFXPlayer{

    public override VFXType GetVFXType() => VFXType.Explosion;


    public override void Execute( IVFXCommand cmd ) {
        base.Execute(cmd);  
    }

    
}
