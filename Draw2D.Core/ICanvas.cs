using System;
using System.Collections.Generic;
using System.Windows.Input;
using Draw2D.Core.Factories.Handles;
using Draw2D.Core.Geo;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Policies;
using Draw2D.Core.Policies.RouterPolicy;

namespace Draw2D.Core
{
    public interface ICanvas
    {
        IEnumerable<Figure> Figures { get; }

        event EventHandler<EventArgs> SceneChanged;
        event EventHandler<FigureClickEventArgs> FigureRightClicked;
        event EventHandler<ConnectionCreatedEventArgs> ConnectionCreated;
        event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        void AddFigure(Figure figure);
        Selection Selection { get; }

        /// <summary>
        /// Width in 1/100 mm.
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// Height in 1/100 mm.
        /// </summary>
        float Height { get; set; }

        ICoordinateSystem CoordinateSystem { get; set; }
        HandleShapeFactory HandleShapeFactory { get; }
        ToolBase ActiveTool { get; }
        IGrid Grid { get; set; }
        double ViewportWidth { get; set; }
        double ViewportHeight { get; set; }
        double ContentOffsetX { get; set; }
        double ContentOffsetY { get; set; }
        void OnMouseLeftDown(double x, double y, bool isShiftKey, bool isCtrlKey);
        void OnMouseMove(double x, double y, bool isShiftKey, bool isCtrlKey);
        void OnMouseLeftUp(double x, double y, bool isShiftKey, bool isCtrlKey);

        void OnMouseRightDown(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseRightUp(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey);

        ICanvas InstallEditPolicy(PolicyBase policy);
        ICanvas UninstallEditPolicy(PolicyBase policy);
        void OnKeyDown(Key key);
        void OnKeyUp(Key key);
        void CreateConnection();
        void OnMouseLeftDoubleClick(double mouseX, double mouseY, bool isShiftKey, bool isCtrlKey);

        List<VectorFigure> GetRenderableFigures();
        void InstallTool(ToolBase tool, Action<ToolBase> onDone);

        IEnumerable<ISnapPolicy> GetInstalledSnapPolicies();
        void RemoveSelected();

        void StartBulkEdit();
        void EndBulkEdit();

    }

    public class SelectionChangedEventArgs :EventArgs
    {
        public SelectionChangedEventArgs()
        {
            
        }
    }

    public class ConnectionCreatedEventArgs : EventArgs
    {
        public Connection Connection { get;private set; }

        public ConnectionCreatedEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }

    public class FigureClickEventArgs : EventArgs
    {
        public Figure Sender { get;private set; }
        public float MousePosX { get; private set; }
        public float MousePosY { get; private set; }

        public FigureClickEventArgs(Figure sender, float mousePosX,float mousePosY)
        {
            Sender = sender;
            MousePosX = mousePosX;
            MousePosY = mousePosY;
        }
    }
}