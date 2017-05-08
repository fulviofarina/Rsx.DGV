namespace Rsx.DGV
{
    public interface IDGVControl
    {
        IDGVControlCreate ICreate { get; }
        IDGVControlInvoke IInvoke { get; }
        IDGVControlMethods IMethods { get; }
    }
}