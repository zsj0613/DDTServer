using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cross
{
    class Program
    {
        static void Main(string[] args)
        {
            new CrossServer().Start();
            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
