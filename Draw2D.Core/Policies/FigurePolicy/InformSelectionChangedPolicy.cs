namespace Draw2D.Core.Policies.FigurePolicy
{
    public class InformSelectionChangedPolicy : PolicyBase
    {
        private readonly Figure[] _clients;

        public InformSelectionChangedPolicy(params Figure[] clients)
        {
            _clients = clients;
        }

        public virtual void InformClients(Figure changedFigure, bool isSelected)
        {
            foreach (var client in _clients)
            {
                client.InformSelectionChanged(changedFigure, isSelected);
            }
        }
    }
}