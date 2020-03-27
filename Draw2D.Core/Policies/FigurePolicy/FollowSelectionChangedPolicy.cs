namespace Draw2D.Core.Policies.FigurePolicy
{
    public class FollowSelectionChangedPolicy : PolicyBase
    {
        private readonly Figure _emitter;

        public FollowSelectionChangedPolicy(Figure emitter)
        {
            _emitter = emitter;
        }

        public virtual void HandleSelectionChanged(Figure figure, Figure changedFigure, bool isSelected)
        {
            figure.IsVisible = changedFigure.IsSelected;
            if (figure.IsVisible)
            {
                figure.BringToFront();
            }
        }

        internal override void OnInstall(Figure hostFigure)
        {
            base.OnInstall(hostFigure);
            hostFigure.IsVisible = _emitter.IsSelected;
        }

      
    }
}