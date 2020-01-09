# Applied Domain-Driven Design (DDD), Part 4 - Infrastructure

If you come from database centric development (where database is the heart of the application) then this is going to be hard for you. In domain-driven design database and general data sources are not important, your application is persistence ignorant.

Put your infrastructure interfaces in to Domain Model Layer. Your domain will use them to get data, it doesn't need to care how, it just knows there is an interface exposed and it will use it. This simplifies things and allows you to focus on your actual Domain rather worrying about what database you will be using, where data is coming from, etc.

## Infrastructure Contracts

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
//this lives in the core library and you can inherit from it and extend it e.g. ICustomerRepository : IRepository<Customer> then you can add some custom methods to your new interface. This can be useful if you want to uselize some of rich features of the ORM that you are using (should be a very rare case)
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

## Infrastructure Implementation (lives in the Infrastructure Layer)

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

## Example usage

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
        //example #1 calling an interface email dispatcher this can have differnet kind of implementation depending on context, e.g
        // smtp = SmtpEmailDispatcher (current), exchange = ExchangeEmailDispatcher, msmq = MsmqEmailDispatcher, etc... let infrastructure worry about it
        this.emailDispatcher.Dispatch(new MailMessage());

        //example #2 calling an interface newsletter subscriber  this can differnet kind of implementation e.g
        // web service = WSNewsletterSubscriber (current), msmq = MsmqNewsletterSubscriber, Sql = SqlNewsletterSubscriber, etc... let infrastructure worry about it

        this.newsletterSubscriber.Subscribe(args.Customer);
    }
}
```

## Summary

- Infrastructure contains implementation classes that actually talks to the infrastructure IO, Sql, Msmq, etc.
- Domain is the heart of the application not the Infrastructure (this can be hard to grasp if you come from DBA background).
- Infrastructure is not important in Domain-design design, it facilitates the application development doesn't lead it.
- Infrastructure should not contain any domain logic, all domain logic should be in the domain. (i guarantee that when you first start out, you will put logic in there without knowing it)


## Tips

When it comes to repositories try and just use a generic repository and stay away from custom implementations as much as possible i.e. IRepository<Customer> = good, ICustomerRepository = bad (it's never that simple, but a good general rule to work with).
When you first start out with infrastructure implementations, force your self not to put any if statements in to it. This will help your mind adjust to leaving all logic out of the Infrastructure Layer.
Take your time and try and understand what persistence ignorance really means, also try and research polyglot persistence this will expand your understanding.

## Useful links

Onion architecture (i really don't like the name) showcases well how you need to start thinking about infrastructure layer - Jeffrey Palermo
Polyglot Persistence - Martin Fowler

**Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*