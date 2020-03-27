using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Caliburn.Micro;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2DDemo.Commands;

namespace Draw2DDemo.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell, IHandle<ICommandChangeMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private ExampleViewModel _exampleViewModel;
        private ICommand _createLineCommand;
        private ICommand _createPolylineCommand;
        private NavigationViewModel _navigationViewModel;
        private ICommand _fitCommand;
        private ICommand _fillCommand;
        private ICommand _oneHundredPercentCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private ICommand _undoZoomCommand;
        private ICommand _redoZoomCommand;
        private ICommand _toggleGridSnapCommand;
        private int _gridUnitX;
        private int _gridUnitY;
        private List<ExampleViewModel> _examples;
        private double _memoryUsage;
        private Timer _memoryUpdateTimer;
        private ICommand _toggleElementSnapCommand;
        private bool _isElementSnapChecked;
        private ICommand _deleteCommand;

        public ICommand FitCommand
        {
            get { return _fitCommand; }
            set
            {
                if (Equals(value, _fitCommand)) return;
                _fitCommand = value;
                NotifyOfPropertyChange(() => FitCommand);
            }
        }

        public ICommand FillCommand
        {
            get { return _fillCommand; }
            set
            {
                if (Equals(value, _fillCommand)) return;
                _fillCommand = value;
                NotifyOfPropertyChange(() => FillCommand);
            }
        }

        public ICommand OneHundredPercentCommand
        {
            get { return _oneHundredPercentCommand; }
            set
            {
                if (Equals(value, _oneHundredPercentCommand)) return;
                _oneHundredPercentCommand = value;
                NotifyOfPropertyChange(() => OneHundredPercentCommand);
            }
        }

        public ICommand ZoomInCommand
        {
            get { return _zoomInCommand; }
            set
            {
                if (Equals(value, _zoomInCommand)) return;
                _zoomInCommand = value;
                NotifyOfPropertyChange(() => ZoomInCommand);
            }
        }

        public ICommand ZoomOutCommand
        {
            get { return _zoomOutCommand; }
            set
            {
                if (Equals(value, _zoomOutCommand)) return;
                _zoomOutCommand = value;
                NotifyOfPropertyChange(() => ZoomOutCommand);
            }
        }

        public ICommand UndoZoomCommand
        {
            get { return _undoZoomCommand; }
            set
            {
                if (Equals(value, _undoZoomCommand)) return;
                _undoZoomCommand = value;
                NotifyOfPropertyChange(() => UndoZoomCommand);
            }
        }

        public ICommand RedoZoomCommand
        {
            get { return _redoZoomCommand; }
            set
            {
                if (Equals(value, _redoZoomCommand)) return;
                _redoZoomCommand = value;
                NotifyOfPropertyChange(() => RedoZoomCommand);
            }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
            set
            {
                if (Equals(value, _deleteCommand)) return;
                _deleteCommand = value;
                NotifyOfPropertyChange(() => DeleteCommand);
            }
        }

        public NavigationViewModel NavigationViewModel
        {
            get { return _navigationViewModel; }
            set
            {
                if (Equals(value, _navigationViewModel)) return;
                _navigationViewModel = value;
                NotifyOfPropertyChange(() => NavigationViewModel);
            }
        }

        public double MemoryUsage
        {
            get { return _memoryUsage; }
            set
            {
                if (value.Equals(_memoryUsage)) return;
                _memoryUsage = Math.Round(value, 2);
                NotifyOfPropertyChange(() => MemoryUsage);
            }
        }

        public ExampleViewModel ExampleViewModel
        {
            get { return _exampleViewModel; }
            set
            {
                if (Equals(value, _exampleViewModel)) return;
                _exampleViewModel = value;

                CreateLineCommand = _exampleViewModel.LineCommand;
                CreatePolylineCommand = _exampleViewModel.PolylineCommand;
                DeleteCommand = _exampleViewModel.DeleteCommand;
                GridUnitX = _exampleViewModel.GridUnitX;
                GridUnitY = _exampleViewModel.GridUnitY;

                ActivateItem(_exampleViewModel);

                FitCommand = ExampleViewModel.FitCommand;

                NotifyOfPropertyChange(() => ExampleViewModel);
            }
        }

        public ICommand CreateLineCommand
        {
            get { return _createLineCommand; }
            set
            {
                if (Equals(value, _createLineCommand)) return;
                _createLineCommand = value;
                NotifyOfPropertyChange(() => CreateLineCommand);
            }
        }

        public ICommand ToggleGridSnapCommand
        {
            get { return _toggleGridSnapCommand; }
            set
            {
                if (Equals(value, _toggleGridSnapCommand)) return;
                _toggleGridSnapCommand = value;

                NotifyOfPropertyChange(() => ToggleGridSnapCommand);
            }
        }

        public ICommand ToggleElementSnapCommand
        {
            get { return _toggleElementSnapCommand; }
            set
            {
                if (Equals(value, _toggleElementSnapCommand)) return;
                _toggleElementSnapCommand = value;
                NotifyOfPropertyChange(() => ToggleElementSnapCommand);
            }
        }

        public ICommand CreatePolylineCommand
        {
            get { return _createPolylineCommand; }
            set
            {
                if (Equals(value, _createPolylineCommand)) return;
                _createPolylineCommand = value;
                NotifyOfPropertyChange(() => CreatePolylineCommand);
            }
        }


        public int GridUnitX
        {
            get { return _gridUnitX; }
            set
            {
                if (value.Equals(_gridUnitX)) return;
                _gridUnitX = value;
                foreach (var example in _examples)
                {
                    example.GridUnitX = _gridUnitX;
                }

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
                foreach (var example in _examples)
                {
                    example.GridUnitY = _gridUnitY;
                }
                NotifyOfPropertyChange(() => GridUnitY);

            }
        }

        public bool IsElementSnapChecked
        {
            get { return _isElementSnapChecked; }
            set
            {
                if (value == _isElementSnapChecked) return;
                _isElementSnapChecked = value;
                NotifyOfPropertyChange(() => IsElementSnapChecked);
            }
        }

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _memoryUpdateTimer = new Timer(state =>
            {
                Process currentProcess = Process.GetCurrentProcess();
                long totalBytesOfMemoryUsed = currentProcess.WorkingSet64;
                MemoryUsage = totalBytesOfMemoryUsed / (1024.0 * 1024.0);
            }, null, 0, 1000);


        }

        protected override void OnActivate()
        {
            _examples = new List<ExampleViewModel>
            {
                new LinesExample(_eventAggregator),
                new PathExample(_eventAggregator),
                new SimpleExample(_eventAggregator),
                new BoundingBoxSelectionExample(_eventAggregator),
                new ConnectionRouterExample(_eventAggregator),
                new GlyphedTextExample(_eventAggregator),
                new PerformanceTest1(_eventAggregator),
                new RegionPolicyExample(_eventAggregator),
                new HandlesExample(_eventAggregator),
                new MasterSlaveExample(_eventAggregator),
                new TextExample(_eventAggregator),
                new ShapesExample(_eventAggregator),
                new DragDropPoliciesExample(_eventAggregator)
            };

            NavigationViewModel = new NavigationViewModel(this, _examples);
            ChangeExample(_examples.FirstOrDefault());

            IsElementSnapChecked = true;
            ToggleElementSnapCommand.Execute(null);
        }

        public void ChangeExample(ExampleViewModel selectedExample)
        {
            ExampleViewModel = selectedExample;

            //When the detail view is activate 2nd time we use this commands.
            //For the first activation they will be empty, because the view is not finished with binding.
            FitCommand = ExampleViewModel.FitCommand;
            FillCommand = ExampleViewModel.FillCommand;
            OneHundredPercentCommand = ExampleViewModel.OneHundredPercentCommand;
            ZoomInCommand = ExampleViewModel.ZoomInCommand;
            ZoomOutCommand = ExampleViewModel.ZoomOutCommand;
            UndoZoomCommand = ExampleViewModel.UndoZoomCommand;
            RedoZoomCommand = ExampleViewModel.RedoZoomCommand;

            ToggleGridSnapCommand = new RelayCommand(() =>
            {
                foreach (var example in _examples)
                {
                    example.ToggleGridSnapCommand.Execute(null);
                }
            });

            ToggleElementSnapCommand = new RelayCommand(() =>
            {
                foreach (var example in _examples)
                {
                    example.ToggleElementSnapCommand.Execute(null);
                }
            });
        }

        public void Handle(ICommandChangeMessage message)
        {
            //Be aware of command changes of the example views.
            FitCommand = message.FitCommand;
            FillCommand = message.FillCommand;
            OneHundredPercentCommand = message.OneHundredPercentCommand;
            ZoomInCommand = message.ZoomInCommand;
            ZoomOutCommand = message.ZoomOutCommand;
            UndoZoomCommand = message.UndoZoomCommand;
            RedoZoomCommand = message.RedoZoomCommand;
        }
    }

}