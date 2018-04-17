using OET_Math;
using OET_Types.Elements;

public enum eMaterial
{
    Concrete = 0,
    Steel = 1
}

namespace OET_Types.Elements
{
    public interface IElement
    {
        Node        StartNode   { get; set; }
        Node        EndNode     { get; set; }
        eMaterial   Material    { get; }
        float       Length      { get; }
        int         Id          { get; set; }
        IElement    Clone();
    }
}
