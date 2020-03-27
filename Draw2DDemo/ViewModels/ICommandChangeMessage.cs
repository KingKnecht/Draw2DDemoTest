using System.Windows.Input;

namespace Draw2DDemo.ViewModels
{
    public interface ICommandChangeMessage
    {
        ICommand FitCommand { get; }
        ICommand FillCommand { get; }
        ICommand OneHundredPercentCommand { get; }
        ICommand ZoomInCommand { get; }
        ICommand ZoomOutCommand { get; }
        ICommand UndoZoomCommand { get; }
        ICommand RedoZoomCommand { get; }

    }
}