using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Core.Commands;
using Draw2D.Core.Factories.Handles;
using Draw2D.Core.Handles;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Policies;
using Draw2D.Core.Policies.CanvasPolicy;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;
using Draw2D.Core.Utlils.Linq;
using Draw2D.Core.Utlils.QuadTree;


namespace Draw2D.Core
{
    public class Canvas : ICanvas
    {
        private readonly List<Figure> _figures = new List<Figure>();
        private readonly List<Figure> _adornerfigures = new List<Figure>();

        private bool _leftMouseDown;
        private float _lastMouseDownPosX;
        private float _lastMouseDownPosY;
        private CommandBase _activeCommand;
        private bool _isDragging;
        private Geo.Rectangle _size;
        private float _lastMousePosX;
        private float _lastMousePosY;
        private Figure _rightMouseDownFigureHit;
        private IGrid _grid;
        private int _bulkEditCount;
        private float _width;
        private float _height;

        private readonly List<PolicyBase> _policies = new List<PolicyBase>();
        private SnapTargets _currentSnapTargets = SnapTargets.Center | SnapTargets.Vertices | SnapTargets.MidPoints;
        private ICoordinateSystem _coordinateSystem;
        private Rectangle _viewport = new Rectangle(0,0,1,1);

        public IEnumerable<PolicyBase> Policies
        {
            get { return _policies.Where(p => p.Enabled); }
        }


        public Geo.Rectangle Size
        {
            get { return _size; }
            set
            {
                if (value == _size)
                    return;

                _size = value;

                //Todo: really set all Regions?
                foreach (var policy in Figures.SelectMany(f => f.Policies.OfType<IRegionAwarePolicy>()))
                {
                    policy.Region = _size;
                }
            }
        }

        public IEnumerable<Figure> Figures
        {
            get
            {
                _figures.Sort(Figure.ZorderComparer); //hmmm. Collection for automatic ZOrder sorting would be nice.
                return _figures.Concat(_adornerfigures);
            }
        }

        public IEnumerable<Figure> AdornerFigures
        {
            get
            {
                _adornerfigures.Sort(Figure.ZorderComparer); //hmmm. Collection for automatic ZOrder sorting would be nice.
                return _adornerfigures;
            }
        }

        public event EventHandler<EventArgs> SceneChanged;
        public event EventHandler<FigureClickEventArgs> FigureRightClicked;
        public event EventHandler<ConnectionCreatedEventArgs> ConnectionCreated;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public CommandBase ActiveCommand
        {
            get
            {
                if (_activeCommand == null)
                {
                    _activeCommand = CommandBase.Empty;
                }
                return _activeCommand;
            }
            set
            {
                if (value == null)
                {
                    _activeCommand = CommandBase.Empty;
                }
                _activeCommand = value;
            }
        }

        public Canvas(int zOrder)
        {
            CoordinateSystem = new CartesianCoordinateSystem(0, 0);

            Selection = new Selection(this);

            ZOrder = zOrder;

            HandleFactory = new HandleFactory();

            //InstallEditPolicy(new BoundingBoxSelectionPolicy());
            //InstallEditPolicy(new SnapGridPolicy());
            //InstallEditPolicy(new ConnectionModeKeyboardPolicy());

            SnapCluster = new SnapCluster<Figure>(10, 10);

            QuadTree = new QuadTree<Figure>(new Geo.Rectangle(0,0, Width, Height));


        }

        public QuadTree<Figure> QuadTree { get; private set; }
        public int ZOrder { get; }

        public Selection Selection { get; set; }

        public float Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;

                _width = value;

                if (Width > 0 && Height > 0)
                    RebuildQuadTree();
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                if (_height == value)
                    return;

                _height = value;

                if (Height > 0 && Width > 0)
                    RebuildQuadTree();
            }
        }

        public ICoordinateSystem CoordinateSystem
        {
            get { return _coordinateSystem; }
            set
            {
                _coordinateSystem = value;
                _coordinateSystem.Canvas = this;
                RebuildQuadTree();
            }
        }

        public Figure BringToFront(Figure figure)
        {
            var maxZOrder = Figures.Max(f => f.ZOrder);
            if (figure.ZOrder <= maxZOrder)
            {
                figure.ZOrder = maxZOrder + 1;
            }
            return figure;
        }

        public Figure SendToBack(Figure figure)
        {
            if (figure.ZOrder != 0)
            {
                figure.ZOrder = 0;
            }

            return figure;
        }

        public ICanvas InstallEditPolicy(PolicyBase policy)
        {
            //If selection policy changes, unselect all figures first.
            if (policy is SelectionPolicy)
            {
                foreach (var figure in Selection.All)
                {
                    figure.Unselect();
                }

                //Remove old SelectionPolicy. There can be only one.
                _policies.Remove(policy);
            }


            //Todo: Zoom policy etc.

            //Install the given policy
            policy.OnInstall(this);
            _policies.Add(policy);

            return this;
        }

        public ICanvas UninstallEditPolicy(PolicyBase policy)
        {
            if (policy == null)
                return this;

            policy.OnUninstall(this);
            _policies.Remove(policy);

            return this;
        }


    
        public void OnMouseLeftDown(double x, double y, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(x, y);
            _leftMouseDown = true;
            _lastMouseDownPosX = worldPoint.X;
            _lastMouseDownPosY = worldPoint.Y;

            if (ActiveTool != null)
            {
                ActiveTool.OnMouseLeftDown(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
            }
            else
            {
                Policies.OfType<IMouseAware>()
                    .ToList()
                    .ForEach(p => p.OnMouseLeftDown(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey));
            }
        }

        public void OnMouseLeftUp(double x, double y, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(x, y);
            if (!_leftMouseDown)
                return; //We missed the down event.

            _leftMouseDown = false;

            if (_isDragging)
            {
                _isDragging = false;
                Policies.OfType<IDragAware>().ToList().ForEach(p => p.OnDragEnd(this, isShiftKey, isCtrlKey));
            }
            else
            {
                Policies.OfType<IMouseAware>().ToList().ForEach(p => p.OnMouseLeftUp(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey));
            }

            _lastMousePosX = 0;
            _lastMousePosY = 0;

        }

        public void OnMouseMove(double x, double y, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(x, y);
            if (ActiveTool != null)
            {
                ActiveTool.OnMouseMove(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
                return;
            }

            if (!_leftMouseDown)
            {
                Policies.OfType<IMouseAware>().ToList().ForEach(p => p.OnMouseMove(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey));
            }
            else
            {
                var dxSum = worldPoint.X - _lastMouseDownPosX;
                var dySum = worldPoint.Y - _lastMouseDownPosY;
                var dx = worldPoint.X - _lastMousePosX;
                var dy = worldPoint.Y - _lastMousePosY;

                if (!_isDragging)
                {
                    if (Math.Abs(dxSum) >= MinimalDragDistance || Math.Abs(dySum) >= MinimalDragDistance)
                    {
                        _isDragging = true;

                        Policies.OfType<IDragAware>()
                            .ToList()
                            .ForEach(
                                p =>
                                    p.OnDragStart(this, _lastMouseDownPosX, _lastMouseDownPosY, dxSum, dySum, isShiftKey,
                                        isCtrlKey));


                    }
                }
                else
                {

                    Policies.OfType<IDragAware>()
                        .ToList()
                        .ForEach(
                            p =>
                                p.OnMouseDrag(this, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey));

                }
            }
            _lastMousePosX = worldPoint.X;
            _lastMousePosY = worldPoint.Y;
        }

        public void OnMouseRightDown(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(mouseX, mouseY);
            if (ActiveTool != null)
            {
                ActiveTool.OnMouseRightDown(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
            }
            else
            {
                _rightMouseDownFigureHit = GetBestFigure(worldPoint.X, worldPoint.Y, new List<Type>(), new List<Type>());

                foreach (var policy in Policies.OfType<IMouseAware>())
                {
                    policy.OnMouseRightDown(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
                }
            }
        }

        public void OnMouseRightUp(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(mouseX, mouseY);
            if (_rightMouseDownFigureHit != null)
            {
                var currentFigureHit = GetBestFigure(worldPoint.X, worldPoint.Y, new List<Type>(), new List<Type>());
                if (currentFigureHit == _rightMouseDownFigureHit)
                {
                    //We have a RightClick
                    _rightMouseDownFigureHit = null;
                    OnFigureRightClicked(new FigureClickEventArgs(currentFigureHit, worldPoint.X, worldPoint.Y));
                }
            }
        }


        public void OnMouseLeftDoubleClick(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey)
        {
            var worldPoint = CoordinateSystem.ToWorldSpace(mouseX, mouseY);
            if (ActiveTool != null)
            {
                ActiveTool.OnMouseLeftDoubleClick(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
            }
            else
            {
                foreach (var policy in Policies.OfType<IMouseAware>())
                {
                    policy.OnMouseLeftDoubleClick(this, worldPoint.X, worldPoint.Y, isShiftKey, isCtrlKey);
                }
            }
        }

        public List<VectorFigure> GetRenderableFigures()
        {
            //return Figures.Where(f => f.IsVisible && _viewport.Contains(f)).OfType<VectorFigure>().ToList();
            return Figures.Where(f => f.IsVisible).OfType<VectorFigure>().ToList();
        }

        public void OnKeyDown(Key key)
        {
            foreach (var policy in Policies.OfType<KeyboardPolicy>())
            {
                policy.OnKeyDown(this, key);
            }
        }

        public void OnKeyUp(Key key)
        {
            foreach (var policy in Policies.OfType<KeyboardPolicy>())
            {
                policy.OnKeyUp(this, key);
            }
        }

        public void CreateConnection()
        {

        }

        public float MinimalDragDistance { get; set; } = 4;

        public void AddAdornerFigure(Figure figure)
        {
            if (figure.Canvas == this)
                return;

            figure.Canvas = this;

            _adornerfigures.Add(figure);

            OnSceneChanged();
        }

        public void AddFigure(Figure figure)
        {
            if (figure.Canvas == this)
                return;

            StartBulkEdit();

            figure.Canvas = this;

            if (figure.CanBeSnapTarget)
            {
                SnapCluster.Add(figure.GetSnapPoints(), figure);
            }

            QuadTree.Insert(figure);

            _figures.Add(figure);

            EndBulkEdit();
        }

        protected virtual void OnSceneChanged()
        {
            if (_bulkEditCount > 0)
                return;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NeedsRepaint(Figure figure)
        {
            OnSceneChanged();
        }


        public Figure GetBestFigure(float x, float y, List<Type> blacklist, List<Type> whitelist)
        {
            //Note: We expect that figures are ordered by ZOrder. That's why we can take the last element.
            var hitFigure = Figures.LastOrDefault(f => !blacklist.Contains(f.GetType()) && f.HitTest(x, y));

            var resizeHandle = hitFigure as IHandle;
            if (resizeHandle != null)
            {
                return resizeHandle.Owner;
            }

            return hitFigure;
        }

        public List<Figure> GetBestFigures(Geo.Rectangle searchBox, List<Type> blacklist, List<Type> whitelist)
        {
            var hits = QuadTree.Query(searchBox).ToList(); //Figures.Where(f => searchBox.Contains(f.BoundingBox)).ToList();
            var toFilterOut = new List<Figure>();

            foreach (var type in blacklist)
            {
                foreach (var figure in hits)
                {

                    if (figure.GetType().GetInterfaces().Contains(type))
                    {
                        toFilterOut.Add(figure);
                    }
                    else if (figure.GetType() == type)
                    {
                        toFilterOut.Add(figure);
                    }
                }
            }

            foreach (var filtered in toFilterOut)
            {
                hits.Remove(filtered);
            }

            return hits;
        }

        public void RemoveAdornerFigure(Figure figure)
        {
            if (figure == null)
                return;

            figure.Canvas = null;
            _adornerfigures.Remove(figure);

            OnSceneChanged();
        }

        public void RemoveFigure(Figure figure)
        {
            if (figure == null)
                return;

            StartBulkEdit();

            Selection.All.Remove(figure);

            figure.Canvas = null;
            _figures.Remove(figure);
            SnapCluster.Remove(figure);

            EndBulkEdit();
        }

        private void RebuildQuadTree()
        {
            QuadTree = new QuadTree<Figure>(new Geo.Rectangle(0,0, Width, Height));
            foreach (var oldFigure in _figures.Where(f => f.IsVisible))
            {
                QuadTree.Insert(oldFigure);
            }
        }

        public void ActivateConnectionRouter()
        {
            //ActiveTool = new PolylineTool(r =>
            //{
            //    ActiveTool = null;
            //    //OnConnectionCreated(new ConnectionCreatedEventArgs(r.));
            //});
            //ActiveTool = new LineTool(r => ActiveTool = null);
            ConnectionRouter = new OrthogonalConnectionRouter(r =>
            {
                ActiveTool = null;
                // OnConnectionCreated(r.C);
            });
        }

        public HandleShapeFactory HandleShapeFactory { get; private set; } = new HandleShapeFactory();
        public ToolBase ActiveTool { get; private set; }

        public void InstallTool(ToolBase tool, Action<ToolBase> onDone)
        {
            ActiveTool?.Cancel(this);

            ActiveTool = tool;

            tool.OnDone = (toolBase) =>
            {
                ActiveTool = null;
            };

        }

        public IEnumerable<ISnapPolicy> GetInstalledSnapPolicies()
        {
            return Policies.OfType<ISnapPolicy>();
        }

        public void RemoveSelected()
        {
            StartBulkEdit();
            var toBeDeleted = Selection.All.ToList();
            foreach (var figure in toBeDeleted)
            {
                RemoveFigure(figure);
            }

            Selection.Clear();

            EndBulkEdit();
        }

        public void StartBulkEdit()
        {
            _bulkEditCount++;
        }

        public void EndBulkEdit()
        {
            _bulkEditCount--;
            if (_bulkEditCount == 0)
            {

                RebuildQuadTree();

                OnSceneChanged();
            }
        }



        public ConnectionRouter ConnectionRouter { get; set; }

        public void OnSelectionChanged(Figure figure)
        {
            foreach (var policy in Policies.OfType<SelectFeedbackPolicy>())
            {
                policy.OnSelectionChanged(this, figure);
            }

            OnSelectionChanged();
        }


        public void OnFigureTranslated(Figure figure)
        {
            foreach (var policy in Policies.OfType<SelectFeedbackPolicy>())
            {
                policy.OnDimensionsChanged(this, figure);
            }
        }

        public IHandleFactory HandleFactory { get; set; }

        public IGrid Grid
        {
            get
            {
                if (_grid == null)
                {
                    _grid = EmptyGrid.Grid;
                }

                return _grid;
            }
            set
            {
                _grid = value;
                OnSceneChanged();
            }
        }

        public double ViewportWidth
        {
            get { return _viewport.Width; }
            set
            {
                _viewport = new Rectangle(-ContentOffsetX.ToFloat(), -ContentOffsetY.ToFloat(),
                                           value.ToFloat(),ViewportHeight.ToFloat());
                OnSceneChanged();
            }
        }

        public double ViewportHeight
        {
            get { return _viewport.Height; }
            set
            {
                _viewport = new Rectangle(-ContentOffsetX.ToFloat(),- ContentOffsetY.ToFloat(),
                                            ViewportWidth.ToFloat(), value.ToFloat());
                OnSceneChanged();
            }
        }

        public double ContentOffsetX
        {
            get { return _viewport.X; }
            set
            {
                _viewport = new Rectangle(-value.ToFloat(), -ContentOffsetY.ToFloat(), ViewportWidth.ToFloat(),
                    ViewportHeight.ToFloat());
                OnSceneChanged();
            }
        }

        public double ContentOffsetY
        {
            get { return _viewport.Y; }
            set
            {
                _viewport = new Rectangle( -ContentOffsetX.ToFloat(),- value.ToFloat(),
                    ViewportWidth.ToFloat(),ViewportHeight.ToFloat());
                OnSceneChanged();
            }
        }
        protected virtual void OnFigureRightClicked(FigureClickEventArgs e)
        {
            FigureRightClicked?.Invoke(this, e);
        }

        protected virtual void OnConnectionCreated(ConnectionCreatedEventArgs e)
        {
            ConnectionCreated?.Invoke(this, e);
        }

        public void TrySelect(Figure figure)
        {
            //Todo: Ask policy to select?
        }

        public IEnumerable<ISnapPolicy> GetSnapPolicies()
        {
            return Policies.OfType<ISnapPolicy>().OrderBy(p => p.Priority).Reverse();
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs());
        }

        public SnapCluster<Figure> SnapCluster { get; set; }

        public void OnFigureDrag(Figure figure)
        {
            SnapCluster.Remove(figure);
            if (figure.CanBeSnapTarget)
            {
                SnapCluster.Add(figure.GetSnapPoints(), figure);
            }

        }
    }


    public interface IRenderContext
    {
        void DrawLine(Pen pen, Point p1, Point p2);
        void DrawEllipse(Brush brush, Pen pen, Point center, float radiusX, float radiusY);

    }
}