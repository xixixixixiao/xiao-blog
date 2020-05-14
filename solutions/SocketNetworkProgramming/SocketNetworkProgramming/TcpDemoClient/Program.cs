using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpDemoClient
{
    /// <summary>
    /// 示例:
    /// 面向连接的网络编程初窥: 使用 TCP
    /// 
    /// 客户端.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            MultipleTimesSend();
        }

        /// <summary>
        /// 1. 使用 TCP
        /// </summary>
        static void UseTcp()
        {
            /**
             * 1. 新建一个客户端 Socket 对象, 用于连接服务端.
             */
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            /**
             * 2. 使用 `Connect` 方法连接服务端的 IP 终结点.
             */
            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 50000));

            /**
             * 3. 调用`Receive` 或 `Send` 方法和服务端进行通信.
             */
            byte[] data = new byte[1024];
            clientSocket.Receive(data);

            Console.WriteLine($"接收到服务端消息: {Encoding.UTF8.GetString(data)}");

            var message = "来着客户端消息: world";
            clientSocket.Send(Encoding.UTF8.GetBytes(message));

            Console.WriteLine($"发送消息到服务端: {message}");

            /**
             * 4. 使用结束之后断开连接, 并释放 Socket.
             */
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            Console.ReadKey();
        }

        /// <summary>
        /// 2. 多次向服务端发送数据.
        /// </summary>
        static void MultipleTimesSend()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 50000));

            byte[] data = new byte[1024];
            clientSocket.Receive(data);

            const int count = 10;

            // 多次向服务端发送数据.
            for (int i = 0; i < count; i++)
            {
                var message = "来着客户端消息: world";
                clientSocket.Send(Encoding.UTF8.GetBytes(message));
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            Console.ReadKey();
        }
    }
}

