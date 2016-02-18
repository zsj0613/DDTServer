using CrossFighting.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CrossFighting
{
    class Program
    {
        static void Main(string[] args)
        {
            CrossFightServer.CreateInstance(new CrossFightServerConfig());
            CrossFightServer.Instance.Start();
            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
