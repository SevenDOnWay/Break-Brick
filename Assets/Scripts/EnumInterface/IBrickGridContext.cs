public interface IBrickGridContext {
    void HealNeighbors( IGridPosition origin, int radius, int healAmount );
    void DamageRow( IGridPosition origin, int damage, DamageSource source );
    void DamageColumn( IGridPosition origin, int damage, DamageSource source );
    void DamageRadial( IGridPosition origin, int radius, int damage, DamageSource source );
    void DamageOrthogonalNeighbors( IGridPosition origin, int damage, DamageSource source, int depth );
}
