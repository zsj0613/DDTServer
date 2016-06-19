using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Server.Manager
{
    public interface IRunMgr
    {
        bool CenterStatus { get; }
        bool FightStatus { get; }
        bool GameStatus { get; }
    }
}
