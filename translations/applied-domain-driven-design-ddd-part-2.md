# Applied Domain-Driven Design (DDD), Part 2 - Domain Events

In my last post we have addressed DDD thought process and constant refining/re-factoring. In this post we are going to talk about domain events. There are many articles on this out there (see references at the bottom), so i will be brief.

When something has happened in the domain, domain event can be raised. It can be from a trivial property change to an overall object state change. This is a fantastic way to describe an actual event in your domain, i.e. customer has checked out, customer was created, etc.

## Let's extend our previous e-commerce example

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

For example in our case, when customer calls Customer.Checkout(...) we raise "CustomerCheckedOut" event. When event is handled it should not change the purchase object state, it should facilitate additional behavior only. For example sending out an email, updating financial monthly balance sheet, calling third party API, etc.

## This is how you could use it

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

It can be confusing, what do you put in to Customer.Checkout(...) method and what do you put in to the handler?

In my opinion Customer.Checkout(...) method should do what it needs to do with the access to the properties/fields that it has access to. So creating a purchase object and adding it to the collection, incrementing purchase count on the customer, updating last purchase date on the customer object, you get the idea.

What does handler have access to? All of the infrastructure layer interfaces. This makes it a great place to send out emails and notifications, synchronize with third party services, create audit records, etc.

## Summary

Domain event handlers should not change the state of the object that caused domain event to be triggered.
Domain events should be raised inside the domain.
Domain events handlers should be looked at as a side effect / chain reaction facilitator.

## Useful links

1. [Domain Events - Salvation](http://www.udidahan.com/2009/06/14/domain-events-salvation/)
2. [Strengthening your domain - domain events](http://lostechies.com/jimmybogard/2010/04/08/strengthening-your-domain-domain-events/)

*Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.