# Applied Domain-Driven Design (DDD) - Event Logging & Sourcing For Auditing

原文: [Applied Domain-Driven Design (DDD) - Event Logging & Sourcing For Auditing](http://www.zankavtaskin.com/2016/08/applied-domain-driven-design-ddd-event.html)

In this article I am going to explore the use of Event Logging and Sourcing as a solution for domain auditing.  This article is not going to explore how to use Event Sourcing to obtain the current model state.

## What is Event Logging?

In my previous article I've [explored domain events](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-2.html). In that article they were synchronous unpersisted events. Aggregate root or Service would just raise an event and a handler would handle it. In this article we are going to change that, we are going to persist these domain events.

## What is Event Sourcing? 

"Append-only store to record the full series of events that describe actions taken on data in a domain, rather than storing just the current state, so that the store can be used to materialize the domain objects. This pattern can simplify tasks in complex domains by avoiding the requirement to synchronize the data model and the business domain; improve performance, scalability, and responsiveness; provide consistency for transactional data; and maintain full audit trails and history that may enable compensating actions." - [Event Sourcing Pattern Microsoft](https://msdn.microsoft.com/en-gb/library/dn589792.aspx)

## Requirements domain Event Logging and Sourcing can fulfil:

- As a technical support member of staff I would like to be able to view audit log so that I can find out what my customers did i.e. did they get themselves in to a mess or is it that our software is buggy?
- As a system admin I would like to be able to view the audit log so that I can find out what my users are doing i.e. someone is not sure why something was changed, software admin needs to double check what happened. 
- As a security analyst I would like to view audit log so that I can find out who has committed fraud. 
- As a business expert I would like to find out how long it has taken someone to go through a process so that we can optimise it. 
- As a security analyst I would like audit log to be immutable so that no one can tamper with it 
- As a software engineer I would like to see what user has done so that I can re-produce their steps and debug the application. 
- As a software engineer I would like persisted domain events to be forwarded to the queue as we can't have 2 phase commit in the Cloud.

## Why not just use CQRS with Event Sourcing? 

As it was mentioned by [Udi, CQRS is a pattern that should be used where data changes are competitive or collaborative](http://udidahan.com/2011/04/22/when-to-avoid-cqrs/). A lot of systems don't fall in to this category, even if they do, you would only use CQRS potentially with Event Sourcing (CQRS != Event Sourcing) for a [part of the application and not everywhere](https://www.infoq.com/news/2016/04/event-sourcing-anti-pattern). This means you can't have automatic audit for your entire system by using CQRS with Event Sourcing.

Event Sourcing is all about storing events and then sourcing them to derive the current model state.
If you don't need "undo" and "replay" functionality, and if you don't need to meet super high scalability non-functional requirements (which most likely you don't) why over-engineer?

This proposed solution is just logging events to get some of the benefits that Event Sourcing provides without the deriving the current model state. However, it will still be sourcing the events to obtain the audit log.

## Why is this a good solution for auditing? 

Your domain is rich and full of domain events ([domain event is something that has happened, it's an immutable fact and you are just broadcasting it](http://cqrs.nu/Faq/commands-and-events)). It's also written using ubiquitous language. Because it describes what has happened and what was changed it's a great candidate to meet your [auditing, troubleshooting, debugging](http://martinfowler.com/eaaDev/EventSourcing.html) and 2 phase commit Cloud requirements.  


### Pros:
- It's fairly easy to create audit read model from domain events  
- Domain events provide business context of what has happened and what has changed  
- Reference data (Mr, Dr, etc) is stored in the same place so you can provide full audit read model 
- Events can be written away to append only store 
- Only useful event data is stored 

### Cons:
- Every request (command) must result in domain event and you need to flatten it, it's more development work
- Requires testing 
- Duplication of data. One dataset for current state. Second dataset for events. There might be mismatch due to bugs and changes. 

## What about "proof of correctness"?

Udi, has already [discussed this here](http://udidahan.com/2011/04/22/when-to-avoid-cqrs/) (scroll down to the "proof of correctness").

I recommend that you keep your storage transaction logs, it doesn't give you proof of correctness however it gives you extra protection. If someone bypasses your application and tampers with your data in the database at least it will be logged and you will be able to do something about it.

## Domain event logging implementation example 

I am going to take my [previous article and build upon it](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-2.html). I've introduced in the past this interface:

```cs
public interface IDomainEvent {}
```

IDomainEvent interface was used like this:

```cs
public class CustomerCheckedOut : IDomainEvent
{
    public Purchase Purchase { get; set; }
}
```
We are going to change IDomainEvent to DomainEvent:
```cs
public abstract class DomainEvent 
{
    public string Type { get { return this.GetType().Name; } }
 
    public DateTime Created { get; private set; }
 
    public Dictionary<string, Object> Args { get; private set; }
 
    public DomainEvent()
    {
        this.Created = DateTime.Now;
        this.Args = new Dictionary<string, Object>();
    }
 
    public abstract void Flatten();
}
```
This new DomainEvent will:

1. Give you a timestamp for when domain event was created 
2. Get the domain event name 
3. Force events to flatten its payloads 
4. Stores important arguments against the event 

Here is example implementation:

```cs
public class CustomerCheckedOut : DomainEvent
{
    public Purchase Purchase { get; set; }
 
    public override void Flatten()
    {
        this.Args.Add("CustomerId", this.Purchase.Customer.Id);
        this.Args.Add("PurchaseId", this.Purchase.Id);
        this.Args.Add("TotalCost", this.Purchase.TotalCost);
        this.Args.Add("TotalTax", this.Purchase.TotalTax);
        this.Args.Add("NumberOfProducts", this.Purchase.Products.Count);
    }
}
```
Flatten method is used to capture important arguments against the event. How you flatten really depends on your requirements. For example if you want to store information for audit purposes, then above flatten might be good enough. If you want to store events so that you can "undo" or "replay" you might want to store more information.

Why have Flatten method at all? Why not serialise and store the entire "Purchase" object? This object might have many value objects hanging of it, it might also have an access to another aggregate root. You will end up storing a lot of redundant data, it will be harder to keep track of versions (if your object shape changes, which it will) and it will be harder to query. This is why Flatten method is important, it strips away all of the noise.

We don't want to handle all event flattening and persisting manually. To simplify and automate the event handling process I've introduced generic event handler:

```cs
public class DomainEventHandle<TDomainEvent> : Handles<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    IDomainEventRepository domainEventRepository;
 
    public DomainEventHandle(IDomainEventRepository domainEventRepository)
    {
        this.domainEventRepository = domainEventRepository;
    }
 
    public void Handle(TDomainEvent args)
    {
        args.Flatten();
        this.domainEventRepository.Add(args);
    }
}
```
## Extending this to meet additional security and operational requirements

You can take this few steps further and create a correlation id for the entire web request. This way you will be able to correlate IIS W3C logs, event logs and database logs. Find out how you can achieve this here. 

*Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment. 
