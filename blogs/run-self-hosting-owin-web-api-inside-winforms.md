# 实现 WebApi 自托管服务宿主于 WinForms 及其交互

在 Windows 平台 Web 服务一般托管于 IIS. 在开发中, 会遇到 WinForms 或 WPF 服务端程序需要提供对外 API 作为服务. 在本文详细介绍 WebApi 自托管于 WinForms 中, WPF 或 Console 程序实现类似.

## 0. 完整示例演示

![示例演示](./images/self-hosting-demo-gif.gif)

## 1. 新建解决方案以及 WinForms 工程

### 1.1 新建解决方案及工程

如下图所示:

![解决方案结构](./images/self-hosting-solution.png)

### 1.2 拖拽控件

绘制必要控件, 布局如下:

![窗体布局](./images/self-hosting-main-form.png)

备注: 绘制一个 NumericUpDown 和两个 Button 控件.

## 2. 开发 HTTP 服务类

```csharp
/// <summary>
/// HTTP service.
/// </summary>
public class HttpService
{
    /// <summary>
    /// HTTP self hosting.
    /// </summary>
    private HttpSelfHostServer _server;

    #region HTTP Service

    /// <summary>
    /// start HTTP server.
    /// </summary>
    /// <param name="port">the port of the http server</param>
    public void StartHttpServer(string port)
    {
        var config = new HttpSelfHostConfiguration($"http://0.0.0.0:{port}");

        config.MapHttpAttributeRoutes();
        config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}");

        this._server = new HttpSelfHostServer(config);
        this._server.OpenAsync().Wait();
    }

    /// <summary>
    /// Close HTTP server.
    /// </summary>
    public void CloseHttpServer()
    {
        this._server.CloseAsync().Wait();
        this._server.Dispose();
    }

    #endregion
}
```

WebApi 自托管服务主要由 `HttpSelfHostServer` 实现.

```csharp
config.MapHttpAttributeRoutes();
```

可以在 `Controller` 中使用路由特性.

## 3. 调用 HTTP 服务

在 MainForm 窗体程序中引用 HTTP 服务:

```csharp

public class MainForm:Form
{
    /// <summary>
    /// Http service.
    /// </summary>
    private readonly HttpService _http;

    public MainForm()
    {
        /**
         * initialize http service.
         */
        _http = new HttpService();
    }
}

```

### 3.1 编写开启 HTTP 服务代码

```csharp
/// <summary>
/// start the http server.
/// </summary>
private void StartButton_Click(object sender, EventArgs e)
{
    /**
     * start.
     */
    try
    {
        var port = this.PortNum.Value;

        _http.StartHttpServer($"{port}");
    }
    catch (Exception exception)
    {
        MessageBox.Show($"{exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

### 3.2 编写关闭 HTTP 服务代码

```csharp
/// <summary>
/// close the http server.
/// </summary>
private void CloseButton_Click(object sender, EventArgs e)
{
    /**
     * close.
     */
    try
    {
        _http.CloseHttpServer();

    }
    catch (Exception exception)
    {
        MessageBox.Show($"{exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

## 4. 开发控制器

```csharp
/// <summary>
/// Home controller.
/// </summary>
[RoutePrefix("api/home")]
public class HomeController : ApiController
{
    /// <summary>
    /// Print the greetings
    /// </summary>
    /// <param name="name">visitor</param>
    /// <returns>greetings</returns>
    [Route("echo")]
    [HttpGet]
    public IHttpActionResult Echo(string name)
    {
        return Json(new {Name = name, Message = $"Hello, {name}"});
    }
}
```

## 5. 合在一起

![解决方案完整结构](./images/self-hosting-solution-complete.png)

[下载完整示例代码 (GitHub)](https://github.com/xixixixixiao/xiao-blog/tree/master/solutions/SelfHostingDemo)