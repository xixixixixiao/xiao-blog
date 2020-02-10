# 应用领域驱动设计, 第六章 - 应用程序服务 Applied Domain-Driven Design (DDD), Part 6 - Application Services

> We have developed our domain, setup our infrastructure, now we need to expose our domain to Applications. This is where Application Service comes in.

我们已经开发完了领域, 建立了基础设施层, 现在我们需要把领域暴露给应用程序层. 这就是应用程序服务层的来源.

> Any kind of application should be able to consume your Application Service and use it, mobile, desktop or web. It's good to keep this in mind when you setup your Application Service and Distributed Interface Layer (Web Service).

任何种类的应用应该消费和使用你的应用程序服务层, 手机应用, 桌面应用, Web 应用. 当你建立应用程序服务层和分布式接口层 (Web Service) 时, 最好记住这一点.

## 应用程序服务契约 (位于单独的项目中) Application Service Contract (lives in a separate project):

```cs
public interface ICartService
{
    CartDto Add(Guid customerId, CartProductDto cartProductDto);
    CartDto Remove(Guid customerId, CartProductDto cartProductDto);
    CartDto Get(Guid customerId);
    CheckOutResultDto CheckOut(Guid customerId);
}
 
// Dto's are Data Transfer Objects, 
// they are very important as they allow you to
// input and get the output from Application
// Services without exposing the actual Domain.
//
// DTO 就是数据传输对象 (Data Transfer Objects) 的缩写.
// DTO 十分重要, 因为它们允许你在不暴露真实领域的情况下接
// 受来自应用程序服务层的输入输出.
public class CartDto
{
    public Guid CustomerId { get; set; }
    public List<CartProductDto> Products { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class CartProductDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CheckOutResultDto
{
    public Guid? PurchaseId { get; set; }
    public CheckOutIssue? CheckOutIssue { get; set; }
}
 ```

## 应用程序服务的实现 Application Service Implementation:

```cs
public class CartService : ICartService
{
    readonly IRepository<Customer> repositoryCustomer;
    readonly IRepository<Product> repositoryProduct;
    readonly IUnitOfWork unitOfWork;
    readonly ITaxDomainService taxDomainService;

    public CartService(IRepository<Customer> repositoryCustomer, 
        IRepository<Product> repositoryProduct, IUnitOfWork unitOfWork, ITaxDomainService taxDomainService)
    {
        this.repositoryCustomer = repositoryCustomer;
        this.repositoryProduct = repositoryProduct;
        this.unitOfWork = unitOfWork;
        this.taxDomainService = taxDomainService;
    }

    public CartDto Add(Guid customerId, CartProductDto productDto)
    {
        CartDto cartDto = null;
        Customer customer = this.repositoryCustomer.FindById(customerId);
        Product product = this.repositoryProduct.FindById(productDto.ProductId);

        this.validateCustomer(customerId, customer);
        this.validateProduct(product.Id, product);

        decimal tax = this.taxDomainService.Calculate(customer, product);

        customer.Cart.Add(CartProduct.Create(customer.Cart, product, productDto.Quantity, tax));

        cartDto = Mapper.Map<Cart, CartDto>(customer.Cart);
        this.unitOfWork.Commit();
        return cartDto;
    }

    public CartDto Remove(Guid customerId, CartProductDto productDto)
    {
        CartDto cartDto = null;
        Customer customer = this.repositoryCustomer.FindById(customerId);
        Product product = this.repositoryProduct.FindById(productDto.ProductId);

        this.validateCustomer(customerId, customer);
        this.validateProduct(productDto.ProductId, product);

        customer.Cart.Remove(product);
        cartDto = Mapper.Map<Cart, CartDto>(customer.Cart);
        this.unitOfWork.Commit();
        return cartDto;
    }

    public CartDto Get(Guid customerId)
    {
        Customer customer = this.repositoryCustomer.FindById(customerId);
        this.validateCustomer(customerId, customer);
        return Mapper.Map<Cart, CartDto>(customer.Cart);

    }

    public CheckOutResultDto CheckOut(Guid customerId)
    {
        CheckOutResultDto checkOutResultDto = null;
        Customer customer = this.repositoryCustomer.FindById(customerId);
        this.validateCustomer(customerId, customer);

        CheckOutIssue? checkOutIssue = customer.Cart.IsCheckOutReady();

        if (!checkOutIssue.HasValue)
        {
            Purchase purchase = customer.Cart.Checkout();
            checkOutResultDto = Mapper.Map<Purchase, CheckOutResultDto>(purchase);
            this.unitOfWork.Commit();
        }

        return checkOutResultDto;
    }

    // this is just an example, don't hard code strings like this,
    // use reference data or error codes.
    //
    // 这仅仅只是一个示例, 不要像这样硬编码, 要使用引用数据或错误码.
    private void validateCustomer(Guid customerId, Customer customer)
    {
        if (customer == null)
            throw new Exception(String.Format("Customer was not found with this Id: {0}", customerId));
    }

    private void validateProduct(Guid productId, Product product)
    {
        if (product == null)
            throw new Exception(String.Format("Product was not found with this Id: {0}", productId));
    }
}
```

## 使用范例 Example usage:

```cs
this.cartService.Add(
    this.customer.Id, 
    new CartProductDto()
    {
        ProductId = viewModel.ProductId,
        Quantity = 2
    }
);
 
CheckOutResultDto checkoutResult = this.cartService.CheckOut(this.customer.id);
```

## 总结 Summary:

> - Application Service is a gateway in to your Domain Model Layer via Dto's (Data Transfer Objects)

应用程序服务层是 DTO (数据传输对象) 进入领域模型层的网关.

> - Application Service should not encapsulate any domain logic, it should be really thin 

应用程序服务层不应该封装任何领域逻辑. 它应该极度轻薄.

> - Application Service method should do only one thing and do it well with one region of the domain, don't mix it to "make it more performance efficient for the Application that's consuming it".

应用程序服务层的方法应该只做一件事, 并且与领域的一个区域配合良好. 不要将其混淆为 "使用领域的应用程序服务的性能更加高效".

> - To access Application Service you expose interface and Dto's for inputs and outputs (it's important not to expose your Domain Entity in a raw format, Dto is a proxy and it protects your domain)

为了访问应用程序服务层, 你应该公开应用程序服务层的接口和 DTO, 方便接收输入和输出. (特别重要的是不要公开暴露领域实体的原始格式, DTO 是领域实体的代理并保护了领域)

> - Presenter (mobile app, desktop or web), should call different services to get data it needs and manipulate it to suit the UI. This might seem inefficient, or wasteful at first. You will realise that actually it's just as fast (if not faster), easier to test and maintain. 

表示层 (移动应用, 桌面应用或者 Web 应用) 应该调用不同的服务去获取它们需要的数据, 并对数据进行调整以适配 UI. 乍一看, 这好像有点低效或者浪费, 你应该意识到实际上它们一样快 (不亚于更快), 易于调试和维护.

## 技巧 Tips:

> - Use AutoMapper to map your Domain Entity to Dto's, don't waste your time with manual mapping. It clutters your implementation code and maintenance becomes a nightmare. 

要使用 `AutoMapper` 去映射领域实体到 DTO, 不要浪费时间在手动映射上. 手动映射会把实现代码弄得乱七八糟, 然后就成了维护的噩梦了.

> - Don't think of screens when it comes to exposing Application Services, it's an API, think how a mobile app would access it, or how external potential customers would use it.

当涉及公开应用程序服务层时, 不要考虑到界面之类的事情, 应用程序服务层是 API, 要考虑移动应用将会如何访问它, 或者外部潜在的客户端如何使用它.

> - Realise that you will end up writing Application Services that suit your UI. This is only natural as this is what you been doing for a while. It will take a few goes before you change your thinking.

我想你最终肯定会为了适配 UI 去编写应用程序服务层. 这个做法其实很自然的, 因为你之前就一直这么做了好长时间, 所以你得试几次才能转变思路.

> - Application Service can be consumed directly if you don't need distribution i.e. your MVC app will just reference Application Service directly, you can then just try and catch errors in your Controller.

如果不需要单独分发应用程序服务层的话, 则可以直接使用它. 也就是说, MVC 应用可以直接引用到应用程序服务层, 然后就可以在控制器中使用 `try-catch` 去捕获错误了.

> - Application Service can be exposed via Web Service (Distributed Interface Layer). This further abstraction give you ability to "try and catch" errors so they can be exposed in a friendlier manner. Additionally it allows you to future proof your application e.g. versioning.

应用程序服务层可以通过 Web Service (分布式接口层) 的方式公开. 这种进一步地抽象使你可以用 `try-catch` 去捕获这些错误, 方便以更加友好的方式公开这些错误. 此外, 它还能在未来去验证应用程序, 比如版本控制之类的.

> **Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*
