# Is Entity the same as Value Object

In this post, we’ll discuss an interesting question about whether the concepts of Entity and Value Object are the same.

I wrote a lot about entities and value objects. Here’s the go-to post if you need to learn more about what they are and the differences between them: [Entity vs Value Object: the ultimate list of differences](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/). And to this date, I find interesting angles to look at these concepts from.

This one comes from Panos Kousidis who asked a insightful question in the comments to [one of my posts about value objects](https://enterprisecraftsmanship.com/posts/value-object-better-implementation/):

> Can we consider an "Entity" as a "ValueObject" that compares only its Id for equality? Can this result in defining the base entity class as

```csharp
public abstract class Entity : ValueObject
{
   public int Id { get; protected set; }

   protected override IEnumerable<object> GetEqualityComponents()
   {
       yield return Id;
   }
}
```

This is a deep question which doesn’t have a quick answer, so let’s break it down into two parts. Here’s what differentiates entities from value objects (again, taken from [this article](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/)):

- Identity comparison
- Immutability
- History preservation

Let’s review the treatment of entities as value objects with regards to each of these two items.

## Identity comparison

**Identity comparison** defines how two instances of a class compare to each other.

Entities are compared by their identifiers. Two objectsi are deemed equal if they have the same Id:

![Identifier equality](./images/identifier-equality.png)
<center><i>Identifier equality</i></center>

Value objects are compared by their content. Two value objects are deemed the same if their contents match:

![Identifier equality](./images/structural-equality.png)
<center><i>Structural equality</i></center>

Note that although you usually compare value objects by **all** of their contents, it doesn’t have to always be the case. Some fields might not matter for identity comparison.

An example is the `Error` class I brought up in a [recent article](https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/):

```csharp
public sealed class Error : ValueObject
{
    public string Code { get; }
    public string Message { get; }

    internal Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code; // the only field used for comparison
    }
}
```

This class contains two fields:

- `Code` — this is what defines an error,
- `Message` — for additional debug information just for developers.

If you pass errors to external systems, those systems shouldn’t ever bind to error messages, only to their codes. This is why the `Error` class uses only the `Code` field for identity comparison: changes in debug messages don’t matter; two errors with the same code are treated as the same error even if their messages differ.

This is where Panos Kousidis' question comes from too. If you can exclude some fields from a value object’s identity comparison, can you also exclude all of them (except for the Id) and end up with the code like the following?

```csharp
public abstract class Entity : ValueObject
{
    public int Id { get; protected set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}

public class Customer : Entity
{
    public string Name { get; protected set; }
    public string Email { get; protected set; }
}
```

You definitely can, I don’t see any reason why not. So, from the identity comparison perspective, the answer to the question "*Can you treat Entity and Value Object as the same concept?*" is **yes**.

## Immutability

In terms of immutability, the difference between entities and value object is that value objects are immutable, whereas entities are almost always mutable. You don’t modify a value object; instead, you create a new one and replace the old instance with it.

One could argue that immutability isn’t a defining property of a value object either. You could even apply the same line of reasoning as with **identity comparison** and say that what matters is immutability of the fields that form the value object’s identity, and that all other fields can be left mutable. In the example with the `Error` class, that would mean being able to change the `Message` field, but not `Code`.

And it’s true that fields that form the object’s identity must not change. This requirement works similarly for entities and value objects:

- The modification of an entity’s Id field would turn that entity into a different one. Thus, such a modification is prohibited.
- Similarly, the modification of fields that form a value object’s identity would, too, turn that value object into a different one.

But what about the remaining fields? If we can change an entity’s properties (except for the Id one), can’t we also change the fields of a value object, as long as they aren’t part of its identity?

On the surface, it looks like we can, but this line of reasoning falls apart when you take into account the 3rd component that differentiates entities from value objects: history preservation.

## History preservation

**History preservation** is whether or not an object has a history in your domain model.

Entities have such a history (even though you might not store it explicitly). In other words, entities *live in a continuum*: they are created, modified, and deleted — all at different points in time. Value objects don’t have a history; they are a mere snapshot of some state.

The modification of a value object implicitly extends its lifetime beyond just being a snapshot. Such a modification assumes the value object also has a history, which goes against the requirement of not preserving history in value objects.

History preservation is what answers the question of "*Can you treat Entity and Value Object as the same concept?*". That answer is **no**.

## Summary

The answer to the question of "Can we consider an entity as a value object that compares only its Id for equality?" boils down to three parts:

- In terms of **identity comparison**, the answer is **yes**.
- In terms of **immutability**, the answer is **yes**.
- In terms of **history preservation**, the answer is **no**.

Thus, the overall answer is also **no**.
