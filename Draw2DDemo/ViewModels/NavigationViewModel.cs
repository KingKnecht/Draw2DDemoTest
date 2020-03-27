using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Draw2DDemo.ViewModels
{
    public class NavigationViewModel
    {
        private readonly IShell _shell;
        private ExampleViewModel _selectedItem;
        public ObservableCollection<ExampleViewModel> Examples { get; set; } = new ObservableCollection<ExampleViewModel>();

        public NavigationViewModel(IShell shell, IEnumerable<ExampleViewModel> examples)
        {
            _shell = shell;
            foreach (var exampleCanvasViewModel in examples)
            {
                Examples.Add(exampleCanvasViewModel);
            }
        }

        public ExampleViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                _shell.ChangeExample(_selectedItem);
            }
        }
    }
}