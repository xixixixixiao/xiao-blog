# Applied Domain-Driven Design (DDD), Part 1 - Basics

When I started learning domain-driven design there was a lot of theory to take in, Eric Evans did a great job explaining it from theoretical point of view. As a software engineer I wanted to see some code and just to follow some examples, I found very little resource out there that showed applied domain-driven design in C#.

Over the coming weeks I will be posting series of articles on the subject, it will be my attempt to make domain-driven design simpler and easier follow. Articles that are published:

- [Applied Domain-Driven Design (DDD), Part 0 - Requirements and Modelling](http://www.zankavtaskin.com/2014/12/applied-domain-driven-design-ddd-part-0.html)
- [Applied Domain-Driven Design (DDD), Part 1 - Basics](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-1.html)
- [Applied Domain-Driven Design (DDD), Part 2 - Domain events](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-2.html)
- [Applied Domain-Driven Design (DDD), Part 3 - Specification Pattern](http://www.zankavtaskin.com/2013/10/applied-domain-driven-design-ddd-part-3.html)
- [Applied Domain-Driven Design (DDD), Part 4 - Infrastructure](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4_16.html)
- [Applied Domain-Driven Design (DDD), Part 5 - Domain Service](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4.html)
- [Applied Domain-Driven Design (DDD), Part 6 - Application Services](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4.html)
- [Applied Domain-Driven Design (DDD), Part 7 - Read Model](http://www.zankavtaskin.com/2016/06/applied-domain-driven-design-ddd-part-7.html)
- [Applied Domain-Driven Design (DDD), My Top 5 Best Practices](https://www.codeproject.com/Articles/1131462/Domain-Driven-Design-My-Top-Best-Practices)
- [Applied Domain-Driven Design (DDD), Event Logging & Sourcing For Auditing](http://www.zankavtaskin.com/2016/08/applied-domain-driven-design-ddd-event.html)

![Domain Driven Design Architecture](./images/applied-domain-driven-design-ddd-part-1/DDD_png_pure.png)

*Domain Driven Design Architecture (it's simpler then it looks)*

## 1. Before we get started let's see why DDD is so great

- Development becomes domain oriented not UI/Database oriented
- Domain layer captures all of the business logic, making your service layer very thin i.e. just a gateway in to your domain via DTO's
- Domain oriented development allows you to implement true service-oriented architecture i.e. making your services reusable as they are not UI/Presentation layer specific 
- Unit tests are easy to write as code scales horizontally and not vertically, making your methods thin and easily testable
- DDD is a set of Patterns and Principles, this gives developers a framework to work with, allowing everyone in the development team to head in the same direction

Through this series of articles I will be focusing on a simple and made up e-commerce domain.

## 2. So, let's see some code

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool Active { get; set; }
}

public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List Purchases { get; set; }
}

public class Purchase
{
    public Guid Id { get; set; }
    public List Products { get; set; }
    public DateTime Created { get; set; }
    public Customer Customer { get; set; }
}
```

Code above represents anemic classes. Some developers would stop here, and use these classes to pass data to service and then bind this data to the UI. Let's carry on and mature our models.

When a customer shops online they choose items first, they browse around, and then eventually they will make a purchase. So we need something that will hold the products, lets call it a Cart, this object will have no identity and it will be transient.

## 3. Cart is my value object

```csharp
public class Cart
{
    public List Products { get; set; }
}
```

Cart simply contains a list of products. Customer can go ahead and checkout these products when they are ready.

We can use what was said above as a business case "Customer can go ahead and checkout these products when they are ready".

## 4. Code would look like

```csharp
Cart cart = new Cart()
{
    Products = new Product[]
    {
        new Product(),
        new Product()
    }
};

Customer customer = new Customer()
{
    FirstName = "Josh",
    LastName = "Smith"
};

Purchase purchase = customer.Checkout(cart);
```

What's going on here? Customer checks-out the product and gets a purchase back. Normally in business context you get a receipt back, this provides basic information about the purchase, discounts, and acts as a guarantee that i can use to refer back to my purchase.

I could rename Purchase to Receipt, but wait, what's does purchase mean in the business context?

*"to acquire by the payment of money or its equivalent; buy."* - Dictionary.com

(Returning purchase object would make sense if customer actually made a purchase i.e. we pre-authenticated a customer and then simply passed payment authentication code to the checkout, but to keep this simple we are not going to do this)

Customer can now checkout, when they checkout they will get back a Purchase object (this will allow further data manipulation).

Code above needs to be re-factored, if we return back a purchase are we going to then add it to the collection of the customer i.e. Customer.Purchases.Add(...)? This seems strange, if we have a method Customer.Checkout(...) we should aim to capture relevant data right there and then. Customer should only expose business methods, if we have to expose something else in order to capture data then we are not doing it right.

## 5. Lets refine further

```csharp
public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List Purchases { get; set; }

    public Purchase Checkout(Cart cart)
    {
        Purchase purchase = new Purchase()
        {
             Customer = this,
             Products = cart.Products,
             Created = DateTime.Now
        };

        this.Purchases.Add(purchase);
        return purchase;
    }
}
```

Ok, so now when customer checks-out a cart, purchase will be added to the purchase collection and also returned  so it can be used further in our logic. This is great, but another software engineer can go in and compromise our domain. They can just add Orders directly to the customer without checking out i.e. Customer.Orders.Add(...).

## 6. Lets refine further

```csharp
public class Customer
{
    private List purchases = new List();

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public ReadOnlyCollection Purchases { get { return this.purchases.AsReadOnly(); } }

    public Purchase Checkout(Cart cart)
    {
        Purchase purchase = new Purchase()
        {
             Customer = this,
             Products = cart.Products,
             Created = DateTime.Now
        };
        this.purchases.Add(purchase);
        return purchase;
    }
}
```

Now orders can't be compromised, and code forces software engineers to checkout a cart. What about other properties? They are not protected. We know customer state can't just be changed, we have to go through a process. You need to ask your self, when personal information is changed on the customer, do we need to send an email out? Do we need to call some 3rd party API to sync up our records? Right now you might not have a requirement from your business analysts to do anything like this, so lets not worry about it, lets just protect these fields so they can't be changed.

```csharp
public class Customer
{
    private List purchases = new List();

    public Guid Id { get; protected set; }
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    public string Email { get; protected set; }
    public ReadOnlyCollection Purchases { get { return this.purchases.AsReadOnly(); } }
    public Purchase Checkout(Cart cart)
    {
        Purchase purchase = new Purchase()
        {
             Customer = this,
             Products = cart.Products,
             Created = DateTime.Now
        };
        this.purchases.Add(purchase);
        return purchase;
    }
}
```

That's great, now other software engineers in the team can't change personal information without adding a new method such as Customer.ChangeDetails(...).

**Taking in to account what was said above, thinking process, constant re-factoring and making the model match the actual business domain, this is what I've got so far:**

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
```

## 7. Example usage

```csharp
Cart cart = Cart.Create(new List() { new Product(), new Product() });
Customer customer = Customer.Create("josh", "smith", "josh.smith@microsoft.com");
Purchase purchase = customer.Checkout(cart);
```

## 8. Summary

- DDD is all about capturing business logic in the domain i.e. entities, aggregate roots, value objects and domain service.
- DDD is all about thought process and challenging where should what go and what is most logical.
- DDD is all about constant re-factoring and maturing your model as you get further requirements. More requirements your receive the better and stronger your domain will be. Therefore requirements are gold and something that software engineers should always strive to understand.

## 9. Useful links

- [Onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/), a very good example of what domain-driven design is all about
- [Aggregate root](http://martinfowler.com/bliki/DDD_Aggregate.html), great explanation of what aggregate root actually is
- [Explanation of Aggregate Root](http://lostechies.com/jimmybogard/2008/05/21/entities-value-objects-aggregates-and-roots/), Entity and Value objects
- [Supple Design, Intention revealing interfaces, etc](http://www.cs.colorado.edu/~kena/classes/6448/s05/lectures/lecture30.pdf). Pdf from University of Colorado

**Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*
