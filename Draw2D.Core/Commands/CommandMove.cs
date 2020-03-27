using System;

namespace Draw2D.Core.Commands
{
    public class CommandMove : CommandBase
    {
        private readonly Figure _figure;
        private int _oldX;
        private int _oldY;
        private int _newX;
        private int _newY;

        public CommandMove(Figure figure, int x, int y)
        {
            _figure = figure;
            _oldX = x;
            _oldY = y;
        }

        public virtual void SetPosition(int x, int y)
        {
            _newX = x;
            _newY = y;
        }

        public override bool CanExcecute()
        {
            return Math.Abs(_newX - _oldX) > 0.001 || Math.Abs(_newY - _oldY) > 0.001;
        }

        public override void Excecute()
        {
            _figure.SetPostion(_newX, _newY);
        }
    }
}