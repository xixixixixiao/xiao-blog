using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpDemoServer
{
    /// <summary>
    /// 示例:
    /// 面向连接的网络编程初窥: 使用 TCP
    /// 
    /// 服务端.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            SupportMultipleSend();
        }

        /// <summary>
        /// 1. 使用 TCP.
        /// </summary>
        static void UseTcp()
        {
            /**
             * 1. 新建一个服务端 Socket 对象, 用于绑定和监听客户端发起的连接.
             * 
             *    面向连接的 Socket 的类型为 Stream, 协议类型为 Tcp.
             */
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            /**
             * 2. 新建一个 IP 终结点, 其中包含了服务端的 IP 地址和端口. 然后服务端 Socket 对象使用 `Bind` 方法绑定它.
             * 
             *    演示程序, 绑定本机回环地址, 并监听 50000 端口.
             */
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 50000));

            /**
             * 3. 服务端 Socket 对象调用 `Listen` 方法, 服务端开启监听.
             * 
             *    开启监听, 指定最大连接数是 10.
             */
            serverSocket.Listen(10);

            Console.WriteLine("开启监听, 等待客户端连接.");

            /**
             *   4. 服务端 Socket 对象调用 `Accept` 方法, 等待客户端的连接. 此时, 将一直处于阻塞状态, 直到有新的客户端连接.
             *   5. 一旦有客户端连接到服务端, `Accept` 方法将创建一个新的 Socket 对象, 它包含了服务端/客户端的信息和协议.
             *   
             *      等待客户端连接.
             */
            Socket clientSocket = serverSocket.Accept();

            Console.WriteLine($"收到客户端 {clientSocket.RemoteEndPoint} 连接.");

            /**
             * 6. 使用 `Accept` 方法新建的 Socket 对象调用 `Receive` 或 `Send` 方法和客户端通信.
             * 
             *    发送数据.
             */
            string message = "来自服务端消息: hello";
            clientSocket.Send(Encoding.UTF8.GetBytes(message));

            Console.WriteLine($"发送消息到客户端: {message}");

            /**
             *    接收数据.
             */
            byte[] data = new byte[1024];
            clientSocket.Receive(data);

            Console.WriteLine($"接收到客户端消息: {Encoding.UTF8.GetString(data)}");

            /**
             * 使用结束之后断开连接, 并释放 Socket.
             */
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            Console.ReadKey();
        }

        /// <summary>
        /// 2. 使用循环反复接收客户端.
        /// </summary>
        static void UseTcpWithLoop()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 50000));
            serverSocket.Listen(10);

            // 使用循环反复接收客户端.
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                string message = "来自服务端消息: hello";
                clientSocket.Send(Encoding.UTF8.GetBytes(message));

                byte[] data = new byte[1024];
                clientSocket.Receive(data);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        /// <summary>
        /// 3. 支持客户端分批发送的数据.
        /// </summary>
        static void SupportMultipleSend()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 50000));
            serverSocket.Listen(10);

            // 使用循环反复接收客户端.
            while (true)
            {
                Console.WriteLine("等待客户端连接...");

                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine($"收到客户端 {clientSocket.RemoteEndPoint} 连接.");

                string message = "来自服务端消息: hello";
                clientSocket.Send(Encoding.UTF8.GetBytes(message));

                // 反复多次接收客户端发送的数据.
                while (true)
                {
                    byte[] data = new byte[1024];

                    var length = clientSocket.Receive(data);
                    var result = Encoding.UTF8.GetString(data, 0, length);

                    Console.WriteLine($"接收到客户端消息: {result}");

                    if (string.IsNullOrEmpty(result))
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 2. 使用带多线程版本的 TCP.
        /// </summary>
        static void UseTcpWithThread()
        {
            /**
             * 1. 新建一个服务端 Socket 对象, 用于绑定和监听客户端发起的连接.
             * 
             *    面向连接的 Socket 的类型为 Stream, 协议类型为 Tcp.
             */
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            /**
             * 2. 新建一个 IP 终结点, 其中包含了服务端的 IP 地址和端口. 然后服务端 Socket 对象使用 `Bind` 方法绑定它.
             * 
             *    演示程序, 绑定本机回环地址, 并监听 50000 端口.
             */
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 50000));

            /**
             * 3. 服务端 Socket 对象调用 `Listen` 方法, 服务端开启监听.
             * 
             *    开启监听, 指定最大连接数是 10.
             */
            serverSocket.Listen(10);

            Console.WriteLine("开启监听, 等待客户端连接.");

            while (true)
            {
                /**
                 *   4. 服务端 Socket 对象调用 `Accept` 方法, 等待客户端的连接. 此时, 将一直处于阻塞状态, 直到有新的客户端连接.
                 *   5. 一旦有客户端连接到服务端, `Accept` 方法将创建一个新的 Socket 对象, 它包含了服务端/客户端的信息和协议.
                 *   
                 *      等待客户端连接.
                 */
                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine($"收到客户端 {clientSocket.RemoteEndPoint} 连接.");

                Task.Run(() =>
                {
                    /**
                     * 6. 使用 `Accept` 方法新建的 Socket 对象调用 `Receive` 或 `Send` 方法和客户端通信.
                     * 
                     *    发送数据.
                     */
                    string message = "来自服务端消息: hello";
                    clientSocket.Send(Encoding.UTF8.GetBytes(message));

                    Console.WriteLine($"发送消息到客户端: {message}");

                    /**
                     *    接收数据.
                     */
                    byte[] data = new byte[1024];
                    clientSocket.Receive(data);

                    Console.WriteLine($"接收到客户端消息: {Encoding.UTF8.GetString(data)}");

                    /**
                     * 使用结束之后断开连接, 并释放 Socket.
                     */
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                });
            }

            // Console.ReadKey();
        }
    }
}

