# .NET Core Socket 网络编程, 第一篇 - 支撑 .NET Core 网络编程的基础类

.NET Core 提供了功能丰富的类来支持网络编程. 在 .NET Core 的命名空间 `System.Net` 与 `System.Net.Sockets` 中包含了强大的类用于开发网络应用程序. 工欲善其事必先利其器, 在正式介绍 Socket 网络编程之前先了解一些网络编程相关的基础类的信息有助于理解后续代码.

## `IPAddress` 类

`IPAddress` 类提供 Internet 协议 (IP) 地址. 它表示了网络上的计算机的 IP 地址及其相关信息.

### IPAddress 常用字段/属性

| 字段/属性   | 说明                                              |
| :---------- | :------------------------------------------------ |
| `Any`       | 等效于以点分隔的四部分表示法中的 `0.0.0.0`.       |
| `Broadcast` | 等效于以点分隔的四部分表示法的 `255.255.255.255`. |
| `Loopback`  | 等效于以点分隔的四部分表示法中的 `127.0.0.1`.     |

### IPAddress 常用方法

| 方法            |                                     说明 |
| :-------------- | ---------------------------------------: |
| `Parse(String)` | 将 IP 地址字符串转换为 `IPAddress` 实例. |

## `IPEndPoint` 类

`IPEndPoint` 类将网络终结点表示为 IP 地址和端口号.

包含应用程序连接到主机上的服务所需的主机和本地或远程端口信息. 通过将主机的 IP 地址和端口号组合在一起, `IPEndPoint` 类形成服务的连接点.

### IPEndPoint 常用字段/属性

| 字段/属性       | 说明                        |
| :-------------- | :-------------------------- |
| `Address`       | 获取或设置终结点的 IP 地址. |
| `AddressFamily` | 获取网际协议 (IP) 地址族.   |
| `Port`          | 获取或设置终结点的端口号.   |

### IPEndPoint 常用方法

| 方法            | 说明                                                                  |
| :-------------- | :-------------------------------------------------------------------- |
| `Parse(String)` | 将表示为字符串的 IP 网络终结点 (地址和端口) 转换为 `IPEndPoint` 实例. |

## `IPHostEntry` 类

为 Internet 主机地址信息提供容器类. `IPHostEntry` 类将域名系统 (DNS) 主机名与一组别名和匹配的 IP 地址的数组相关联, 用作 Dns 类查询主机信息返回值.

## `Dns` 类

`Dns` 类提供简单的域名解析功能.

Dns 类是一个静态类，用于从 Internet 域名系统 (DNS) 检索有关特定主机的信息. DNS 查询中的主机信息将在 `IPHostEntry` 类的实例中返回. 如果指定的主机在 DNS 数据库中有多个条目, `IPHostEntry` 将包含多个 IP 地址和别名.

示例:

```cs
/** 获取域名 www.microsoft.com 的主机信息. */
IPHostEntry hostInfo = Dns.GetHostEntry("www.microsoft.com");
```

## `Ping` 及其相关类

Linux, mac OS 和 Windows 操作系统都提供了 `ping` 命令程序, 用于测试网络的连接状况. 与之对应, .NET Core 也提供了 `Ping`, `PingOptions` 和 `PingReply` 类用于检测远程计算机是否可访问.

- **`Ping` 类**, 检测远程计算机是否可访问.
- **`PingOptions` 类**, 用于控制如何传输 Ping 数据包.
- **`PingReply` 类**, 调用 `Ping` 类的 `Send` 方法之后返回目标主机的相关信息, 例如其状态和发送请求和接收回复所用的时间.

示例:

```cs
 Ping pingSender = new Ping();

 PingOptions options = new PingOptions
 {
     DontFragment = true
 };

 string target  = "127.0.0.1";
 string data    = "hello world";
 byte[] buffer  = Encoding.ASCII.GetBytes(data);
 int    timeout = 120;

 PingReply reply = pingSender.Send(target, timeout, buffer, options);
```

## Socket 详解

如前所述, Socket 是[支持 TCP/IP 网络通信的基本操作单元](dotnet-core-socket-network-programming-01-communication-mechanism.md). 在一个 Socket 的实例中, 不仅仅保存了本机的 IP 地址和端口, 还保存了目标计算机的 IP 地址和端口, 同时还有通信双方所使用的协议.

Socket 可以被视为 Stream 流一样数据管道, 而 Socket 这个管道存在于服务端与客户端的两台计算机之间, 在计算机之间的数据的发送与接收均在于这个管道中进行. 因此, 在应用程序创建好 Socket 对象之后, 就可以用 `Send`/`SendTo` 方法将数据发送到连接的 Socket 中, 或者使用 `Receive`/`ReceiveFrom` 方法从连接的 Socket 对象中接收数据.

### Socket 类的类型

Socket 有三种类型, 分别是 `Stream Socket` (流 Socket); `Dgram Socket` (数据报 Socket); `Raw Socket` (原始 Socket).

- **Stream Socket** 用于实现 TCP 通信, 它面向连接, 传输的数据是可靠无差错且无重复, 发送和接收数据的顺序是相同的.

- **Dgram Socket** 用于实现 UDP 通信, 它 **无** 连接的, 以独立的 Dgram 形式发送数据 (数据包尺寸不能大于 32KB), 无容错检查, 也不保证收发顺序.

- **Raw Socket** 用于实现 IP 数据包通信, 直接访问最底层. 常用于侦听与分析数据包.

在 .NET Core 中, 以上三种类型的 Socket 均可使用命名空间 `System.Net.Sockets` 中的 `Socket` 类实现. `Socket` 类构造函数如下:

```cs
public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType);
```

- `addressFamily` 参数指定 `Socket` 类使用的寻址方案:
  
  - `AddressFamily.InterNetwork` 代表了 IP 版本 4 (IPv4) 的地址;
  - `AddressFamily.InterNetworkV6` 代表了 IP 版本 6 (IPv6) 的地址.

- `socketType` 参数指定 `Socket` 类的类型.
  
- `protocolType` 参数指定 `Socket` 使用的协议.

`socketType` 参数与 `protocolType` 参数的枚举值不是独立的, 必须搭配使用. 某些地址簇限制了可与它们一起使用的协议, 如果地址簇, Socket 类型和协议类型的组合导致无效 Socket, 则此构造函数将引发一个 `SocketException`. 以下表格列出了这两个参数的组合:

| `socketType`        | `protocolType`      | 说明                        |
| :------------------ | :------------------ | :-------------------------- |
| `SocketType.Stream` | `ProtocolType.Tcp`  | 面向连接, 可靠的通信.       |
| `SocketType.Dgram`  | `ProtocolType.Udp`  | 无连接的, 不可靠的通信.       |
| `SocketType.Raw`    | `ProtocolType.Icmp` | 使用 Internet 控制消息协议. |
| `SocketType.Raw`    | `ProtocolType.Igmp` | 使用 Internet 组管理协议.   |

示例:

```cs
/** 基于 TCP 的 IPv4 的面向连接的 Socket. */
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
```

上述代码创建了一个支持可靠, 双向, 面向连接的字节流. 数据既不重复数据, 也不保留边界. 此类型的 Socket 与单个对方主机通信, 并且在通信开始之前需要建立远程主机连接.

```cs
/** 基于 UDP 的 IPv4 的无连接的 Socket. */
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
```

上述代码创建了一个无连接的, 支持数据报的 Socket. 消息可能会丢失或重复并可能在到达时不按顺序排列, 可以与多个对方主机进行通信.

### Socket 类的常用属性

| 属性             | 说明                                                                        |
| :--------------- | :-------------------------------------------------------------------------- |
| `AddressFamily`  | 获取 Socket 的地址簇                                                        |
| `Available`      | 获取已经从网络接收且可供读取的数据量                                        |
| `Connected`      | 获取一个值, 该值指示 Socket 是在上次 Send 还是 Receive 操作时连接到远程主机 |
| `LocalEndPoint`  | 获取本地终结点                                                              |
| `ProtocolType`   | 获取 Socket 的协议类型                                                      |
| `RemoteEndPoint` | 获取远程终结点                                                              |
| `SocketType`     | 获取 Socket 的类型                                                          |

### Socket 类的常用方法

1. `void Connect(EndPoint remoteEP)`

   与指定的远程计算机终结点之间建立网络连接. 如果使用的是无连接协议 (如 UDP), 则在发送和接收数据之前无需调用 `Connect` 方法.

2. `void Bind(EndPoint localEP)`

   使 Socket 与一个本地终结点相关联, 必须先调用 `Bind` 方法, 然后才能调用 `Listen` 方法.

3. `void Listen(int backlog)`

   `Listen` 方法用于等待客户端发起连接的请求. `backlog` 参数指定可以排队等待接受的最大连接数量. 必须先调用 `Bind` 方法, 然后才能调用 `Listen` 方法, 否则将引发 `SocketException` 异常.

4. `Socket Accept()`

   创建新的 Socket 已处理连接请求. 当执行到 `Accept` 方法时, 线程会处于阻塞状态, 直到有新的客户端请求连接为止, 然后创建并返回新的 `Socket`, 其中包含了客户端的信息.

5. `int Send(byte[] buffer)`

   `Send` 将数据同步发送到 `Connect` 或 `Accept` 方法中指定的远程主机, 并返回成功发送的字节数. `Send` 可用于面向连接的协议和无连接的协议.

6. `int Receive(byte[] buffer)`

   将数据读取到 `buffer` 参数并返回成功读取的字节数. 当 `Receive` 没有可读数据时, 将处于阻塞状态.

7. `void Shutdown(SocketShutdown how)`

   关闭连接, 禁止发送和接收数据.

8. `void Close()`

   关闭远程计算机连接, 并释放与 Socket关联的所有托管资源和非托管资源.
