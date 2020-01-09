# 应用领域驱动设计, 第二章 - 领域事件 Applied Domain-Driven Design (DDD), Part 2 - Domain Events

原文: [Applied Domain-Driven Design (DDD), Part 2 - Domain Events](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-2.html)

> In my last post we have addressed DDD thought process and constant refining/re-factoring. In this post we are going to talk about domain events. There are many articles on this out there (see references at the bottom), so i will be brief.

在我的上一篇文章中, 我们讨论了领域驱动设计的思考的过程和不断地完善/重构. 在这篇文章中, 我们将讨论领域事件. 参考本文末, 有许多关于领域事件的文章, 所以本文我将做简单的介绍.

> When something has happened in the domain, domain event can be raised. It can be from a trivial property change to an overall object state change. This is a fantastic way to describe an actual event in your domain, i.e. customer has checked out, customer was created, etc.

当领域中发生 *某些事情* 时, 领域事件可以被引发. 它可以是从小到琐碎的属性的改变, 大到整体对象的状态改变. 在领域中, 这是一种描述真实事件发生的最佳方式, 即, *客户已经结账*, *客户被创建*等等.

## 让我们扩展之前的电商示例 (Let's extend our previous e-commerce example)

```csharp
public class Product
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public int Quantity { get; protected set; }
    public DateTime Created { get; protected set; }
    public DateTime Modified { get; protected set; }
    public bool Active { get; protected set; }
}

public class Cart
{
    private List products;

    public ReadOnlyCollection Products
    {
        get { return products.AsReadOnly(); }
    }

    public static Cart Create(List products)
    {
        Cart cart = new Cart();
        cart.products = products;
        return cart;
    }
}

public class Purchase
{
    private List products = new List();

    public Guid Id { get; protected set; }
    public ReadOnlyCollection Products
    {
        get { return products.AsReadOnly(); }
    }
    public DateTime Created { get; protected set; }
    public Customer Customer { get; protected set; }

    public static Purchase Create(Customer customer, ReadOnlyCollection products)
    {
        Purchase purchase = new Purchase()
        {
            Id = Guid.NewGuid(),
            Created = DateTime.Now,
            products = products.ToList(),
            Customer = customer
        };
        return purchase;
    }
}

public class Customer
{
    private List purchases = new List()

    public Guid Id { get; protected set; }
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    public string Email { get; protected set; }
    public ReadOnlyCollection Purchases { get { return this.purchases.AsReadOnly(); } }

    public Purchase Checkout(Cart cart)
    {
        Purchase purchase = Purchase.Create(this, cart.Products);
        this.purchases.Add(purchase);
        DomainEvents.Raise(new CustomerCheckedOut() { Purchase = purchase });
        return purchase;
    }

    public static Customer Create(string firstName, string lastName, string email)
    {
        Customer customer = new Customer()
        {
             FirstName = firstName,
             LastName = lastName,
             Email = email
        };
        return customer;
    }
}

public class CustomerCheckedOut : IDomainEvent
{
    public Purchase Purchase { get; set; }
}

public class CustomerCheckedOutHandle : Handles CustomerCheckedout
{
    public CustomerCheckedOutHandle()
    {

    }

    public void Handle(CustomerCheckedOut args)
    {
        //send notifications, emails, update third party systems, etc
    }
}
```

> For example in our case, when customer calls Customer.Checkout(...) we raise "CustomerCheckedOut" event. When event is handled it should not change the purchase object state, it should facilitate additional behavior only. For example sending out an email, updating financial monthly balance sheet, calling third party API, etc.

例如, 在我们的这个例子中, 当客户调用了 `Customer.Checkout(...)` 时, 会引发 `CustomerCheckedOut` 事件. 当事件被处理时, 不应该改变交易对象的状态, 它仅仅只应该引导附加的行为. 比如发送邮件, 更新财务月度资金表, 调用第三方 API, 等等.

## 你可以通过下面的方式使用领域事件 (This is how you could use it)

```csharp
public class CustomerCheckedOut : IDomainEvent
{
    public Purchase Purchase { get; set; }
}

public class CustomerCheckedOutHandle : Handles CustomerCheckedOut
{
    readonly IEmailSender emailSender;

    public CustomerCheckedOutHandle(IEmailSender emailSender)
    {
        this.emailSender = emailSender;
    }

    public void Handle(CustomerCheckedOut args)
    {
        this.emailSender.Send(args.Purchase.Customer, EmailTemplate.PurchaseMade);
        //send notifications, update third party systems, etc
        // 发送通知, 更新第三方系统, 等等.
    }
}

public interface IEmailSender
{
    void Send(Customer customer, EmailTemplate template);
}

public enum EmailTemplate
{
    PurchaseMade,
    CustomerCreated
}
```

> It can be confusing, what do you put in to Customer.Checkout(...) method and what do you put in to the handler?

这可能会令开发者迷惑, 这到底往 `Customer.Checkout(…)` 方法里放入了什么, 以及会向处理程序传递了什么?

> In my opinion Customer.Checkout(...) method should do what it needs to do with the access to the properties/fields that it has access to. So creating a purchase object and adding it to the collection, incrementing purchase count on the customer, updating last purchase date on the customer object, you get the idea.

在我看来, `Customer.Checkout(...)` 方法应该是对需要访问的属性/字段进行访问, 所以创建一个交易对象并添加到集合中, 增加客户对象的交易的计数, 更新客户对象的上次交易的日期, 就可以了.

> What does handler have access to? All of the infrastructure layer interfaces. This makes it a great place to send out emails and notifications, synchronize with third party services, create audit records, etc.

事件处理程序可以访问些哪些东西? 所有的基础设施层的接口. 这让它可以发送邮件和通知, 同步第三方服务, 创建审计记录, 等等.

## 总结 (Summary)

> - Domain event handlers should not change the state of the object that caused domain event to be triggered.

领域事件处理程序不应该改变导致领域事件触发的对象的状态.

> - Domain events should be raised inside the domain.

领域事件应该由领域内部引发.

> - Domain events handlers should be looked at as a side effect / chain reaction facilitator.

领域事件处理程序应该被视为副作用或者连锁反应的引导者.

## 一些有用的链接 (Useful links)

1. [领域事件 - 救赎](http://www.udidahan.com/2009/06/14/domain-events-salvation/)
2. [强化领域 - 领域事件](http://lostechies.com/jimmybogard/2010/04/08/strengthening-your-domain-domain-events/)

> *Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*
