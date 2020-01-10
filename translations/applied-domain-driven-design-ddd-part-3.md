# 应用领域驱动设计, 第三章 - 规范模式 Applied Domain-Driven Design (DDD), Part 3 - Specification Pattern

原文: [Applied Domain-Driven Design (DDD), Part 3 - Specification Pattern](http://www.zankavtaskin.com/2013/10/applied-domain-driven-design-ddd-part-3.html)

> Specification pattern is great, David Fancher wrote a great piece on it, i suggest you read it before you continue.

规范模式十分强大, David Fancher 了一篇非常好的关于规范模式的文章, 我建议你在阅读本文之前去读一下那篇文章.

> In short, specification pattern allows you to chain business queries.

简而言之, 规范模式就是允许你可以链式业务查询.

## 示例 (Example)

```csharp
ISpecification<Customer> spec =
    new CustomerRegisteredInTheLastDays(30).And(new CustomerPurchasedNumOfProducts(2));
```

## 此系列上文的实体 (Entity from previous posts in this series)

```csharp
public class Customer : IDomainEntity
{
    private List<Purchase> purchases = new List<Purchase>();

    public Guid Id { get; protected set; }
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    public string Email { get; protected set; }
    public string Password { get; protected set; }
    public DateTime Created { get; protected set; }
    public bool Active { get; protected set; }

    public ReadOnlyCollection<Purchase> Purchases { get { return this.purchases.AsReadOnly(); } }

    public static Customer Create(string firstname, string lastname, string email)
    {
        Customer customer = new Customer()
        {
            FirstName = firstname,
            LastName = lastname,
            Email = email,
            Active = true,
            Created = DateTime.Today
        };

        DomainEvents.Raise<CustomerCreated>(new CustomerCreated() { Customer = customer });
        return customer;
    }

    public Purchase Checkout(Cart cart)
    {
        Purchase purchase = Purchase.Create(this, cart.Products);
        this.purchases.Add(purchase);
        DomainEvents.Raise<CustomerCheckedOut>(new CustomerCheckedOut() { Purchase = purchase });
        return purchase;
    }
}
```

## 规范示例 (Specification Examples)

```csharp
public class CustomerRegisteredInTheLastDays : SpecificationBase<Customer>
{
    readonly int nDays;

    public CustomerRegisteredInTheLastDays(int nDays)
    {
        this.nDays = nDays;
    }

    public override Expression<Func<Customer,bool>>  SpecExpression
    {
        get
        {
            return customer => customer.Created >= DateTime.Today.AddDays(-nDays)
                && customer.Active;
        }
    }
}

public class CustomerPurchasedNumOfProducts : SpecificationBase<Customer>
{
    readonly int nPurchases;

    public CustomerPurchasedNumOfProducts(int nPurchases)
    {
        this.nPurchases = nPurchases;
    }

    public override Expression<Func<Customer,bool>>  SpecExpression
    {
        get
        {
            return customer => customer.Purchases.Count == this.nPurchases
                && customer.Active;
        }
    }
}
```

## 抽象仓储查询示例 (Abstract Repository Query Example)

```csharp
IRepository<Customer> customerRepository = new Repository<Customer>();

ISpecification<Customer> spec =
    new CustomerRegisteredInTheLastDays(30).And(new CustomerPurchasedNumOfProducts(2));

IEnumerable<Customer> customers = customerRepository.Find(spec);
```

## 抽象仓储示例 (Abstract Repository Example)

```csharp
public interface IRepository<TEntity>
    where TEntity : IDomainEntity
{
    TEntity FindById(Guid id);
    TEntity FindOne(ISpecification<TEntity> spec);
    IEnumerable<TEntity> Find(ISpecification<TEntity> spec);
    void Add(TEntity entity);
    void Remove(TEntity entity);
}
```

## 总结 (Summary)

> - Specification allows you to query data in a abstract way i.e. you can query memory collections or an RDBMS. This ensures persistence/infrastructure ignorance.

- 规范允许你以一种抽象的方式查询数据, 也就是说, 你可以查询内存集合或者查询关系型数据库. 规范确保了持久层 / 基础设施层的未知.

> - Specification encapsulates a business rule in one spec.

- 规范将业务规则封装在一个规范中.

> - Specification pattern allows you to chain your business rules up.

- 规范模式允许你可以把业务规则链接起来.

> - Specification makes your domain layer DRY i.e. you don't need to write same LINQ all over again.

- 规范使你的领域层干燥, 也就是你不需要再写同样的 LINQ 语句了.

> - Specifications are easy to unit test.

- 规范让单元测试更加容易.

> - Specifications are stored in the domain layer, this provides full visibility.

- 规范存储在领域层, 这提供了完整的可见性.

> - Specifications are super elegant.

- 规范十分优雅. (让你写的代码特别讲究)

## 建议 (Tips)

> - Break complex business logic rules down in your specification as NHibernate might struggle to interpret them in to a SQL query. This is a generally good tip as you don't want messy SQL hitting your database.

- 在你的规范模式中, 需要分解复杂的业务逻辑规则, 因为类似于 NHibernate 之类的 ORM 框架可能难以将其解释为 SQL 查询. 这通常是比较好建议, 因为你不希望将凌乱的 SQL 语句发送到数据库.

> - Query data around the entity properties, don't try and change the properties on the entity i.e. instead of writing customer.Created.AddDays(30) >= DateTime.Today write customer.Created >= DateTime.Today.AddDays(-30). The former will try and compile it as a SQL and will fail as it's too complex, the latter will convert the value to a parameter.

- 查询实体属性的书是, 不要尝试修改实体的属性, 也就是说用 `customer.Created >= DateTime.Today.AddDays(-30)` 代替 `customer.Created.AddDays(30) >= DateTime.Today`. 后者可能因为太复杂导致尝试编译为 SQL 语句而失败, 前者则会把值转化为参数.

> - As specifications are logical queries they should not change state of the caller or the calling objects. i.e. don't call state changing methods, such as customer.Checkout(....) && customer.Active == true. This tip goes hand in hand with the tip above.

- 由于规范是逻辑查询, 因此他们不应该修改调用者或调用对象的状态. 比如, 不要调用类似于这种 `customer.Checkout(....) && customer.Active == true` 修改状态的方法. 这个建议与上面的建议是相辅相成的.

## 一些有用的链接 (Useful links)

- [规范, 表达式树, 以及 NHibernate](http://davefancher.com/2012/07/03/specifications-expression-trees-and-nhibernate/)  一篇很好的文章, 其中包含了有关如何在 NHibernate 中使用规范的出色示例.
- [规范模式](http://en.wikipedia.org/wiki/Specification_pattern), 布尔规范模式的基础说明.

> *Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*
