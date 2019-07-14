using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OET_UI.Command
{
    interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
    }
}
