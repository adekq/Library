using Apache.Ignite.Core;
using System;

namespace IgniteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Ignition.Start();            
        }
    }
}
