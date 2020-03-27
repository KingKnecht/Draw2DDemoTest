using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Policies.CanvasPolicy;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2DDemo.Commands;

namespace Draw2DDemo.ViewModels
{
    public abstract class ExampleViewModel : Screen, ICommandChangeMessage
    {
        private readonly IEventAggregator _eventAggregator;
        private double _worldMousePosX;
        private double _worldMousePosY;
        private int _renderedItemsCount;
        private ICommand _fitCommand;
        private ICommand _fillCommand;
        private ICommand _oneHundredPercentCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private ICommand _undoZoomCommand;
        private ICommand _redoZoomCommand;
        private int _selectionCount;
        private readonly SnapGridPolicy _snapGridPolicy = new SnapGridPolicy();
        private readonly SnapElementPolicy _snapElementPolicy = new SnapElementPolicy();

        private int _gridUnitX;
        private int _gridUnitY;
        private ICommand _enablePanModeCommand;

        public SelectionFeedbackPolicy SelectionFeedbackPolicy { get; set; } = new SelectionFeedbackPolicy();

        public RelayCommand LineCommand { get; set; }

        public RelayCommand PolylineCommand { get; set; }

        public RelayCommand ToggleGridSnapCommand { get; set; }
        public RelayCommand ToggleElementSnapCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        protected ExampleViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Canvas = new Canvas(0)
            {
                Width = 800,
                Height = 600,
                Grid = new Grid()
                {
                    UnitX = 100,
                    UnitY = 100
                }
            };

            GridUnitX = 100;
            GridUnitY = 100;

            Canvas.SelectionChanged += (sender, args) =>
            {
                SelectionCount = ((ICanvas)sender).Selection.All.Count();
                DeleteCommand.RaiseCanExecuteChanged();
            };

            CreateCommands();
            
            Canvas.InstallEditPolicy(new BoundingBoxSelectionPolicy());

            _snapGridPolicy.Enabled = false;
            _snapElementPolicy.Enabled = false;
            Canvas.InstallEditPolicy(_snapGridPolicy);
            Canvas.InstallEditPolicy(_snapElementPolicy);

          
            //AddAxes();
        }

        public int SelectionCount
        {
            get { return _selectionCount; }
            set
            {
                if (value == _selectionCount) return;
                _selectionCount = value;
                NotifyOfPropertyChange(() => SelectionCount);
            }
        }

        public int GridUnitX
        {
            get { return _gridUnitX; }
            set
            {
                if (value.Equals(_gridUnitX)) return;
                _gridUnitX = value;
                Canvas.Grid = new Grid() { UnitX = _gridUnitX, UnitY = GridUnitY };
                NotifyOfPropertyChange(() => GridUnitX);
            }
        }

        public int GridUnitY
        {
            get { return _gridUnitY; }
            set
            {
                if (value.Equals(_gridUnitY)) return;
                _gridUnitY = value;
                Canvas.Grid = new Grid() { UnitX = GridUnitX, UnitY = _gridUnitY };
                NotifyOfPropertyChange(() => GridUnitY);
            }
        }

        private void AddAxes()
        {
            var xLine = new Line(-Canvas.Width / 2, 0, Canvas.Width / 2, 0)
            {
                IsDragable = false,
                IsSelectable = false,
                CanBeSnapTarget = false
            };
            Canvas.AddFigure(xLine);
            xLine.SendToBack();

            var yLine = new Line(0, -Canvas.Height / 2, 0, Canvas.Height / 2)
            {
                IsDragable = false,
                IsSelectable = false,
                CanBeSnapTarget = false
            };
            Canvas.AddFigure(yLine);
            yLine.SendToBack();
        }

        public ICommand FitCommand
        {
            get { return _fitCommand; }
            set
            {
                if (value == null)
                    return;
                _fitCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand FillCommand
        {
            get { return _fillCommand; }
            set
            {
                if (value == null)
                    return;
                _fillCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand OneHundredPercentCommand
        {
            get { return _oneHundredPercentCommand; }
            set
            {
                if (value == null)
                    return;
                _oneHundredPercentCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand ZoomInCommand
        {
            get { return _zoomInCommand; }
            set
            {
                if (value == null)
                    return;
                _zoomInCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand ZoomOutCommand
        {
            get { return _zoomOutCommand; }
            set
            {
                if (value == null)
                    return;
                _zoomOutCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand UndoZoomCommand
        {
            get { return _undoZoomCommand; }
            set
            {
                if (value == null)
                    return;
                _undoZoomCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand RedoZoomCommand
        {
            get { return _redoZoomCommand; }
            set
            {
                if (value == null)
                    return;
                _redoZoomCommand = value;
                _eventAggregator.PublishOnUIThread(this);
            }
        }

        public ICommand EnablePanModeCommand
        {
            get { return _enablePanModeCommand; }
            set
            {
                if (Equals(value, _enablePanModeCommand)) return;
                _enablePanModeCommand = value;
                NotifyOfPropertyChange(() => EnablePanModeCommand);
            }
        }


        private void CreateCommands()
        {

            LineCommand = new RelayCommand(() =>
            {
                Canvas.InstallTool(new LineTool(), (tool) => LineCommand.RaiseCanExecuteChanged());
            }, () => Canvas.ActiveTool == null);


            PolylineCommand = new RelayCommand(() =>
            {
                Canvas.InstallTool(new PolylineTool(), (tool) => CommandManager.InvalidateRequerySuggested() /*PolylineCommand.RaiseCanExecuteChanged()*/);
            }, () => Canvas.ActiveTool == null);

            ToggleGridSnapCommand = new RelayCommand(EnableGridSnapCommandExecute);
            ToggleElementSnapCommand = new RelayCommand(EnableElementSnapCommandExecute);

            DeleteCommand = new RelayCommand(() =>
            {
                Canvas.RemoveSelected();
            }, () => Canvas.Selection.All.Any());

            EnablePanModeCommand = new RelayCommand(EnablePanModeCommandExecute);
        }

      
        private void EnablePanModeCommandExecute()
        {

            //if (Canvas.GetInstalledSnapPolicies().OfType<SnapGridPolicy>().Any())
            //{
            //    Canvas.UninstallEditPolicy(_snapGridPolicy);
            //}
            //else
            //{
            //    Canvas.InstallEditPolicy(_snapGridPolicy);
            //}
        }


        private void EnableGridSnapCommandExecute()
        {
            _snapGridPolicy.Enabled = !_snapGridPolicy.Enabled;
        }

        private void EnableElementSnapCommandExecute()
        {
            _snapElementPolicy.Enabled = !_snapElementPolicy.Enabled;
        }

        public double WorldMousePosX
        {
            get { return _worldMousePosX; }
            set
            {
                if (value.Equals(_worldMousePosX)) return;
                _worldMousePosX = Math.Round(value, 2);
                NotifyOfPropertyChange(() => WorldMousePosX);
            }
        }

        public double WorldMousePosY
        {
            get { return _worldMousePosY; }
            set
            {
                if (value.Equals(_worldMousePosY)) return;
                _worldMousePosY = Math.Round(value, 2);
                NotifyOfPropertyChange(() => WorldMousePosY);
            }
        }

        public int RenderedItemsCount
        {
            get { return _renderedItemsCount; }
            set
            {
                if (value == _renderedItemsCount) return;
                _renderedItemsCount = value;
                NotifyOfPropertyChange(() => RenderedItemsCount);
            }
        }

        public ICanvas Canvas { get; set; }

    }
}