using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Traceroute
{
    public class Traceroute
    {
        private byte sequenceNumber = 0;
        private int ttl = 0;
        private static int maxHops = 30;
        private static int maxWaitingTime = 3000;
        private IPHostEntry hostDNS;
        private Boolean isNodeReached = false;

        public Traceroute(string hostStr)
        {
            try
            {
                hostDNS = Dns.GetHostEntry(hostStr);
                Console.WriteLine($"Tracing to: {hostDNS.HostName} ({hostDNS.AddressList[0]}). Max hops: {maxHops}");
            }
            catch (Exception e)
            {

                Console.WriteLine("Can't resolve entered DNS or IP");
            }
        }

        public void trace()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            IPEndPoint endPoint = new IPEndPoint(hostDNS.AddressList[0], 0);
            EndPoint remoteEndPoint = endPoint;
            DateTime time = new DateTime();
            
            byte[] package = new byte[72];
            MyICMP icmp = new MyICMP(package);
            int error;
            
            byte[] receivedPackage = new byte[256];
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, maxWaitingTime);
            while (ttl <= maxHops)
            {
                error = 0;
                icmp.sequenceNumber(package, sequenceNumber);
                icmp.checkSum(package);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, ttl);
                for (int i = 0; i < 3; i++)
                {
                    isNodeReached = false;
                    try
                    {
                        time = DateTime.Now;
                        socket.SendTo(package, endPoint);
                        socket.ReceiveFrom(receivedPackage, ref remoteEndPoint);
                        TimeSpan timeSpan = DateTime.Now - time;
                        Console.WriteLine($"{ttl} {remoteEndPoint.ToString()} {timeSpan.TotalMilliseconds} ms");
                        isNodeReached = true;
                    }
                    catch (SocketException e)
                    {
                        error++;
                        Console.Write("    *    ");
                        if (error == 3)
                        {
                            Console.WriteLine("    Unable to reach host");
                            break;
                        }
                    }
                    icmp.sequenceNumber(package, ++sequenceNumber);
                    icmp.checkSum(package);
                    if (isNodeReached)
                    {
                        i = 3;
                        continue;
                    }
                }
                ttl++;
            }
        }
    }
}