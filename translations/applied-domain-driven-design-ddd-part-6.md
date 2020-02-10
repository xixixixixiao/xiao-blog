# 应用领域驱动设计, 第六章 - 应用服务 Applied Domain-Driven Design (DDD), Part 6 - Application Services

原文: [Applied Domain-Driven Design (DDD), Part 6 - Application Services](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-6.html)

> We have developed our domain, setup our infrastructure, now we need to expose our domain to Applications. This is where Application Service comes in.

我们已经开发完了领域, 建立起来了基础设施, 现在我们需要把领域暴露给应用层. 这就是`应用服务`的来源.

> Any kind of application should be able to consume your Application Service and use it, mobile, desktop or web. It's good to keep this in mind when you setup your Application Service and Distributed Interface Layer (Web Service).

无论何种类型的应用程序都应该可以使用`应用服务`, 包括手机, 桌面或者 Web 应用. 当开始建立`应用服务`和分布式接口层 (Web Service) 时, 最好把这点记在心里.

## 应用程序服务契约 (位于单独的项目中) Application Service Contract (lives in a separate project)

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
// 收来自 *应用服务* 的输入输出.
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

## 应用服务的实现 Application Service Implementation

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
    // 这仅仅只是一个示例, 不要像这样硬编码, 要使用参考数据或错误码.
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

## 使用范例 Example usage

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

## 总结 Summary

> - Application Service is a gateway in to your Domain Model Layer via Dto's (Data Transfer Objects)

- `应用服务`是 DTO (数据传输对象) 进入领域模型层的网关.

> - Application Service should not encapsulate any domain logic, it should be really thin 

- `应用服务`不应该封装任何领域逻辑. 它应该极度轻薄.

> - Application Service method should do only one thing and do it well with one region of the domain, don't mix it to "make it more performance efficient for the Application that's consuming it".

- `应用服务`的方法应该只做一件事, 配合好领域的一个区域. 不要错误地认为 "直接使用领域的应用程序的性能会更加高效".

> - To access Application Service you expose interface and Dto's for inputs and outputs (it's important not to expose your Domain Entity in a raw format, Dto is a proxy and it protects your domain)

- 为了访问`应用服务`, 你应该公开`应用服务`的接口和 DTO, 方便接收输入和输出. (不要公开暴露领域实体的原始格式很重要, DTO 作为领域实体的代理保护了领域)

> - Presenter (mobile app, desktop or web), should call different services to get data it needs and manipulate it to suit the UI. This might seem inefficient, or wasteful at first. You will realise that actually it's just as fast (if not faster), easier to test and maintain. 

- 表示层 (移动应用, 桌面应用或者 Web 应用) 应该调用不同的服务去获取它们需要的数据, 并对数据进行调整以适配 UI. 乍一看, 这好像有点低效浪费, 实际上你应该意识到它们一样快 (不亚于更快), 因为易于调试和维护.

## 技巧 Tips

> - Use AutoMapper to map your Domain Entity to Dto's, don't waste your time with manual mapping. It clutters your implementation code and maintenance becomes a nightmare. 

- 要使用 `AutoMapper` 去映射领域实体到 DTO, 不要浪费时间在手动映射上. 手动映射会把实现代码弄得乱七八糟, 然后维护就成了噩梦.

> - Don't think of screens when it comes to exposing Application Services, it's an API, think how a mobile app would access it, or how external potential customers would use it.

- 当涉及公开`应用服务`时, 不要考虑到界面之类的事情, `应用服务`是 API, 要考虑移动应用将会如何访问它, 或者潜在的外部客户端如何使用它.

> - Realise that you will end up writing Application Services that suit your UI. This is only natural as this is what you been doing for a while. It will take a few goes before you change your thinking.

- 我想你最终肯定会为了适配 UI 去编写`应用服务`. 这个做法其实很自然的, 因为你之前就一直这么做了好长时间, 所以你得试几次才能转变思路. (译者注: 就是不要为了适配 UI 去编写`应用服务`.)

> - Application Service can be consumed directly if you don't need distribution i.e. your MVC app will just reference Application Service directly, you can then just try and catch errors in your Controller.

- 如果不需要单独分发`应用服务`的话, 则可以直接使用它. 也就是说, MVC 应用可以直接引用`应用服务`, 然后就可以在控制器中使用 `try-catch` 去捕获错误了.

> - Application Service can be exposed via Web Service (Distributed Interface Layer). This further abstraction give you ability to "try and catch" errors so they can be exposed in a friendlier manner. Additionally it allows you to future proof your application e.g. versioning.

- `应用服务`可以通过 Web Service (分布式接口层) 的方式公开. 这种进一步地抽象让你可以用 `try-catch` 去捕获错误, 方便以更加友好的方式公开这些错误. 此外, 它还能在未来去验证应用程序, 比如接口版本之类的.

> **Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*
