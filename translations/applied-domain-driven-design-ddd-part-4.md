# 应用领域驱动设计, 第四章 - 基础设施层 Applied Domain-Driven Design (DDD), Part 4 - Infrastructure

原文: [Applied Domain-Driven Design (DDD), Part 4 - Infrastructure](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4_16.html)

> If you come from database centric development (where database is the heart of the application) then this is going to be hard for you. In domain-driven design database and general data sources are not important, your application is persistence ignorant.

如果你之前都是以数据库为中心的开发模式 (数据库是应用程序的核心), 那么这篇文章将对你来说有点不太友好. 在领域驱动设计中, 数据库和通用的数据源并不那么重要, 程序对持久化一无所知.

> Put your infrastructure interfaces in to Domain Model Layer. Your domain will use them to get data, it doesn't need to care how, it just knows there is an interface exposed and it will use it. This simplifies things and allows you to focus on your actual Domain rather worrying about what database you will be using, where data is coming from, etc.

将基础设施层的接口放入在领域模型层中, 领域将使用这些接口读取数据, 领域并不需要关心如何实现, 领域仅需要知道这有暴露的可以使用的接口. 这样简化了很多事情.

## 基础设施层契约 Infrastructure Contracts

```cs
public interface IEmailDispatcher
{
    void Dispatch(MailMessage mailMessage);
}

public interface INewsletterSubscriber
{
    void Subscribe(Customer customer);
}
```

```cs
// this lives in the core library and you can inherit from it and extend it
// e.g. ICustomerRepository : IRepository<Customer> then you can add some
// custom methods to your new interface. This can be useful if you want to 
// uselize some of rich features of the ORM that you are using (should be a very rare case)
//
// 这个位于核心类库, 你可以继承自它并扩展; 例如: ICustomerRepository : IRepository<Customer>
// 然后就能向新接口中添加一些自定义的方法. 这个可以十分有用, 如果你想使用一些当前正在使用的 ORM
// 的丰富功能 (这是十分罕见的).
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

## 实现基础设施 (位于基础设施层) Infrastructure Implementation (lives in the Infrastructure Layer)

```cs
public class NHibernateRepository<TEntity> : IRepository<TEntity>
        where TEntity : IDomainEntity
    {
        public TEntity FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne(ISpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(ISpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public class SmtpEmailDispatcher : IEmailDispatcher
    {
        public void Dispatch(MailMessage mailMessage)
        {
            throw new NotImplementedException();
        }
    }

    public class WSNewsletterSubscriber : INewsletterSubscriber
    {
        public void Subscribe(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
```

## 使用范例 Example usage

```cs
public class CustomerCreatedHandle : Handles<CustomerCreated>
{
    readonly INewsletterSubscriber newsletterSubscriber;
    readonly IEmailDispatcher emailDispatcher;

    public CustomerCreatedHandle(INewsletterSubscriber newsletterSubscriber, IEmailDispatcher emailDispatcher)
    {
        this.newsletterSubscriber = newsletterSubscriber;
        this.emailDispatcher = emailDispatcher;
    }

    public void Handle(CustomerCreated args)
    {
        // example #1 calling an interface email dispatcher this can have
        // differnet kind of implementation depending on context, e.g
        // smtp = SmtpEmailDispatcher (current),
        // exchange = ExchangeEmailDispatcher,
        // msmq = MsmqEmailDispatcher, etc... let infrastructure worry about it

        // 范例 #1: 调用 email 分发接口. 这在上下文中的实现依赖可以有不同的种类.
        // smtp = SmtpEmailDispatcher (current),
        // exchange = ExchangeEmailDispatcher,
        // msmq = MsmqEmailDispatcher, 等等... 让基础设施层去关心它.
        this.emailDispatcher.Dispatch(new MailMessage());

        // example #2 calling an interface newsletter subscriber
        // this can differnet kind of implementation e.g
        // web service = WSNewsletterSubscriber (current), msmq = MsmqNewsletterSubscriber,
        // Sql = SqlNewsletterSubscriber, etc... let infrastructure worry about it.

        // 范例 #2: 调用通信订阅者接口, 这有不同的实现方式, 例如:
        // web service = WSNewsletterSubscriber (current),
        // msmq = MsmqNewsletterSubscriber,
        // Sql = SqlNewsletterSubscriber, 等等... 让基础设施层去关心它.
        this.newsletterSubscriber.Subscribe(args.Customer);
    }
}
```

## 总结 Summary

> - Infrastructure contains implementation classes that actually talks to the infrastructure IO, Sql, Msmq, etc.

- 基础设施层包含了实际与 IO, 数据库, 消息队列等通信的实现类.

> - Domain is the heart of the application not the Infrastructure (this can be hard to grasp if you come from DBA background).

- 领域是应用程序的核心而非基础设施层 (如果有 DBA 背景的开发者这将有点难以领悟).

> - Infrastructure is not important in Domain-design design, it facilitates the application development doesn't lead it.

- 基础设施层在领域驱动设计中并不重要, 它促进了应用程序的开发, 而非引领程序的开发.

> - Infrastructure should not contain any domain logic, all domain logic should be in the domain. (i guarantee that when you first start out, you will put logic in there without knowing it)

- 基础设施层不应该包含任何领域逻辑, 所有的领域逻辑应该位于领域中. (我保证, 当你第一次使用基础设施层的时候, 你会在不经意间把逻辑放在基础设施层里)

## 技巧 Tips

> When it comes to repositories try and just use a generic repository and stay away from custom implementations as much as possible i.e. IRepository<Customer> = good, ICustomerRepository = bad (it's never that simple, but a good general rule to work with).

当涉及到数据仓储时, 尝试仅用通用的数据仓储, 并尽可能地保持远离自定义的实现. 即: `IRepository<Customer>` 是良好的设计, 而 `ICustomerRepository` 则是糟糕的设计. (这永远不会这么简单, 但一个良好且通用的规则需要处理.)

> When you first start out with infrastructure implementations, force your self not to put any if statements in to it. This will help your mind adjust to leaving all logic out of the Infrastructure Layer.

当你开始实现基础设施层时, 关注你自己不要将任何 `if` 语句放在里面, 这会有助于你的思维逻辑调整, 将所有的业务逻辑排除在基础设施层之外.

> Take your time and try and understand what persistence ignorance really means, also try and research polyglot persistence this will expand your understanding.

花点时间去尝试理解忽略持久化的真正含义, 再尝试去搜索一下多语种持久化会扩展你的理解.

## 有用的链接 Useful links

> - [Onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/) (i really don't like the name) showcases well how you need to start thinking about infrastructure layer - Jeffrey Palermo

- [洋葱架构](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/) (说实话我真不喜欢这个名字) 展示了你应该如何开始思考基础设施层 - Jeffrey Palermo

> - [Polyglot Persistence](http://www.martinfowler.com/bliki/PolyglotPersistence.html) - Martin Fowler

- [多语种持久性](http://www.martinfowler.com/bliki/PolyglotPersistence.html) - Martin Fowler

> *Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*
