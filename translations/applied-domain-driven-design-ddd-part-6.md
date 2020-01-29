# Applied Domain-Driven Design (DDD), Part 6 - Application Services

We have developed our domain, setup our infrastructure, now we need to expose our domain to Applications. This is where Application Service comes in.

Any kind of application should be able to consume your Application Service and use it, mobile, desktop or web. It's good to keep this in mind when you setup your Application Service and Distributed Interface Layer (Web Service).

## Application Service Contract (lives in a separate project):

```cs
public interface ICartService
{
    CartDto Add(Guid customerId, CartProductDto cartProductDto);
    CartDto Remove(Guid customerId, CartProductDto cartProductDto);
    CartDto Get(Guid customerId);
    CheckOutResultDto CheckOut(Guid customerId);
}
 
// Dto's are Data Transfer Objects, 
// they are very important as they allow you to input and get the output from Application Services without exposing the actual Domain.
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
    public Nullable<Guid> PurchaseId { get; set; }
    public Nullable<CheckOutIssue> CheckOutIssue { get; set; }
}
 ```

## Application Service Implementation:

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

        Nullable<CheckOutIssue> checkOutIssue = customer.Cart.IsCheckOutReady();

        if (!checkOutIssue.HasValue)
        {
            Purchase purchase = customer.Cart.Checkout();
            checkOutResultDto = Mapper.Map<Purchase, CheckOutResultDto>(purchase);
            this.unitOfWork.Commit();
        }

        return checkOutResultDto;
    }

    // this is just an example, don't hard code strings like this, use reference data or error codes
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

## Example usage:

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

## Summary:

- Application Service is a gateway in to your Domain Model Layer via Dto's (Data Transfer Objects)
- Application Service should not encapsulate any domain logic, it should be really thin 
- Application Service method should do only one thing and do it well with one region of the domain, don't mix it to "make it more performance efficient for the Application that's consuming it".
- To access Application Service you expose interface and Dto's for inputs and outputs (it's important not to expose your Domain Entity in a raw format, Dto is a proxy and it protects your domain)
- Presenter (mobile app, desktop or web), should call different services to get data it needs and manipulate it to suit the UI. This might seem inefficient, or wasteful at first. You will realise that actually it's just as fast (if not faster), easier to test and maintain. 

## Tips:

- Use AutoMapper to map your Domain Entity to Dto's, don't waste your time with manual mapping. It clutters your implementation code and maintenance becomes a nightmare. 
- Don't think of screens when it comes to exposing Application Services, it's an API, think how a mobile app would access it, or how external potential customers would use it.
- Realise that you will end up writing Application Services that suit your UI. This is only natural as this is what you been doing for a while. It will take a few goes before you change your thinking.
- Application Service can be consumed directly if you don't need distribution i.e. your MVC app will just reference Application Service directly, you can then just try and catch errors in your Controller.
- Application Service can be exposed via Web Service (Distributed Interface Layer). This further abstraction give you ability to "try and catch" errors so they can be exposed in a friendlier manner. Additionally it allows you to future proof your application e.g. versioning.

## Useful links:

- [SOA.com](http://www.soa.com/solutions/soa_explained_7_steps) Service-oriented architecture explained in 7 steps

**Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*
