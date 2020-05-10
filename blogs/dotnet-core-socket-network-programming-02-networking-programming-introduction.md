# .NET Core Socket 网络编程, 第二篇 - 支撑 .NET Core 网络编程的基础类

.NET Core 提供了功能丰富的类来支持网络编程. 在 .NET Core 的命名空间 `System.Net` 与 `System.Net.Sockets` 中包含了强大的类用于开发网络应用程序. 工欲善其事必先利其器, 在正式介绍 Socket 网络编程之前先了解一些网络编程相关的基础类的信息有助于理解后续代码.

## `IPAddress` 类

`IPAddress` 类提供 Internet 协议 (IP) 地址. 它表示了网络上的计算机的 IP 地址及其相关信息.

### 常用字段/属性

| 字段/属性    | 说明                                           |
| :---------- | :--------------------------------------------- |
| `Any`       | 等效于以点分隔的四部分表示法中的 `0.0.0.0`.       |
| `Broadcast` | 等效于以点分隔的四部分表示法的 `255.255.255.255`. |
| `Loopback`  | 等效于以点分隔的四部分表示法中的 `127.0.0.1`.     |

### 常用方法

| 方法            | 说明                                    |
| :-------------- | -------------------------------------: |
| `Parse(String)` | 将 IP 地址字符串转换为 `IPAddress` 实例. |

## `IPEndPoint` 类

`IPEndPoint` 类将网络终结点表示为 IP 地址和端口号.

包含应用程序连接到主机上的服务所需的主机和本地或远程端口信息. 通过将主机的 IP 地址和端口号组合在一起, `IPEndPoint` 类形成服务的连接点.

### 常用字段/属性

| 字段/属性        | 说明                      |
| :-------------- | :------------------------ |
| `Address`       | 获取或设置终结点的 IP 地址. |
| `AddressFamily` | 获取网际协议 (IP) 地址族.   |
| `Port`          | 获取或设置终结点的端口号.   |

### 常用方法

| 方法            | 说明                                                              |
| :-------------- | :---------------------------------------------------------------- |
| `Parse(String)` | 将表示为字符串的 IP 网络终结点（地址和端口）转换为 `IPEndPoint` 实例. |

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
