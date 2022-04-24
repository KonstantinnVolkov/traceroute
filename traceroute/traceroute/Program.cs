using System;
using System.Net;
using System.Net.Sockets;

namespace Traceroute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ready");
            String hostStr = Console.ReadLine();
            Traceroute traceroute = new Traceroute(hostStr);
            traceroute.trace();
        }
    }
}