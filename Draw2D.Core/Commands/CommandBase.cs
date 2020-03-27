using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core.Commands
{
    public abstract class CommandBase
    {
        public virtual bool CanExcecute()
        {
            return true;
        }

        public virtual void Excecute()
        {
            
        }

        public virtual void Cancel()
        {

        }
        
        public static readonly CommandBase Empty = new EmptyCommand();
    }

    public class EmptyCommand : CommandBase
    {}
}
