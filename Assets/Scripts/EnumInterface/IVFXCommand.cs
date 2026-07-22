/// <summary>
/// A data-only visual-effect request. Presentation decides how to render it.
/// </summary>
public interface IVFXCommand {
    VFXType GetVFXType();
}
