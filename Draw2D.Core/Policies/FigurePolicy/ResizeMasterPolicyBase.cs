namespace Draw2D.Core.Policies.FigurePolicy
{
    public abstract class ResizeMasterPolicyBase : DragDropEditPolicy
    {
        protected readonly Figure _slave;

        protected ResizeMasterPolicyBase(Figure slave)
        {
            _slave = slave;
        }
    }
}