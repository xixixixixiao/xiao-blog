# [译文] 实体与值对象相同吗? (Is Entity the same as Value Object?)

> In this post, we’ll discuss an interesting question about whether the concepts of Entity and Value Object are the same.

在这篇文章中, 我们将讨论一个十分有意思的问题, 关于实体与值对象的原理是否相同.

> I wrote a lot about entities and value objects. Here’s the go-to post if you need to learn more about what they are and the differences between them: [Entity vs Value Object: the ultimate list of differences](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/). And to this date, I find interesting angles to look at these concepts from.

我写过许多关于实体和值对象的文章. 如果你想学习关于实体和值对象更多的信息, 在这里你可跳转到关于它俩之间的不同的文章: [实体 vs 值对象: 差异的终极清单](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/). 直到现在, 我发现了有趣的角度去观察他们的原理.

> This one comes from Panos Kousidis who asked a insightful question in the comments to [one of my posts about value objects](https://enterprisecraftsmanship.com/posts/value-object-better-implementation/):

这里是来自的 Panos Kousidis 在评论中提出极富洞察力的问题: [关于值对象的一篇文章](https://enterprisecraftsmanship.com/posts/value-object-better-implementation/)

> Can we consider an "Entity" as a "ValueObject" that compares only its Id for equality? Can this result in defining the base entity class as

我们是否能考虑将 `实体` 作为 `值对象` 仅比较它们的 `Id` 是否相等? 这是否可以将基于实体的类定义为:

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

> This is a deep question which doesn’t have a quick answer, so let’s break it down into two parts. Here’s what differentiates entities from value objects (again, taken from [this article](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/)):

这是个深刻的问题, 且没有简洁的答案, 将其分为两部分. 这里有关于实体和值对象有何不同的文章(还是这里, 跳转到 [这篇文章](https://enterprisecraftsmanship.com/posts/entity-vs-value-object-the-ultimate-list-of-differences/)):

> - Identity comparison

- 标识比较.

> - Immutability

- 不可变性.

> - History preservation

- 历史保留.

> Let’s review the treatment of entities as value objects with regards to each of these two items.

让我们回顾下, 将实体视为值对象的处理方法, 其中涉及到这两项中的每一项.

## 标识比较 (Identity comparison)

> **Identity comparison** defines how two instances of a class compare to each other.

**标识比较** 定义两个类的实例如何相互比较彼此.

> Entities are compared by their identifiers. Two objects are deemed equal if they have the same Id:

通过标识在实体间比较. 对于两个对象, 若他们拥有相同的标识, 则被认为是相等的.

![Identifier equality](./images/identifier-equality.png)
<center><i>Identifier equality (标识相等性)</i></center>

> Value objects are compared by their content. Two value objects are deemed the same if their contents match:

值对象则根据他们的内容去比较. 两个值对象若有相同的内容则被认为相等.

![Identifier equality](./images/structural-equality.png)
<center><i>Structural equality (结构相等性)</i></center>

> Note that although you usually compare value objects by **all** of their contents, it doesn’t have to always be the case. Some fields might not matter for identity comparison.

注意, 尽管你经常通过内容比较值对象, 他们也不总是相等的. 有些属性可能需要注意在比较标识的时候.

> An example is the `Error` class I brought up in a [recent article](https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/):

一个案例是这个我从[最近](https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/)的一篇文章选取的 `Error` 类:

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

> This class contains two fields:

此类包含两个字段 (原作者笔误, 应为`属性`):

> - `Code` — this is what defines an error,
> - `Message` — for additional debug information just for developers.

- `Code` —  此属性定义了错误代码,
- `Message` —  为开发者附加的调试信息.

> If you pass errors to external systems, those systems shouldn’t ever bind to error messages, only to their codes. This is why the `Error` class uses only the `Code` field for identity comparison: changes in debug messages don’t matter; two errors with the same code are treated as the same error even if their messages differ.

如果你往外部系统传递了一些错误, 这下系统不应该绑定错误信息, 仅仅给他们错误码. 这就是为什么 `Error` 类仅使用 `Code` 字段作为标识比较: 调试信息的变化不影响. 两个有相同错误码的错误被认为是相同的错误, 即使他们的调试消息不同.

> This is where Panos Kousidis' question comes from too. If you can exclude some fields from a value object’s identity comparison, can you also exclude all of them (except for the Id) and end up with the code like the following?

这也就是 Panos Kousidis 的问题根源. 如果你能从一个值对象中排除一些字段做标识比较, 你就还能排除除了 Id 之外所有字段直到它们的类似于下面的代码?

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

> You definitely can, I don’t see any reason why not. So, from the identity comparison perspective, the answer to the question "*Can you treat Entity and Value Object as the same concept?*" is **yes**.

## Immutability

> In terms of immutability, the difference between entities and value object is that value objects are immutable, whereas entities are almost always mutable. You don’t modify a value object; instead, you create a new one and replace the old instance with it.

> One could argue that immutability isn’t a defining property of a value object either. You could even apply the same line of reasoning as with **identity comparison** and say that what matters is immutability of the fields that form the value object’s identity, and that all other fields can be left mutable. In the example with the `Error` class, that would mean being able to change the `Message` field, but not `Code`.

> And it’s true that fields that form the object’s identity must not change. This requirement works similarly for entities and value objects:

> - The modification of an entity’s Id field would turn that entity into a different one. Thus, such a modification is prohibited.
> - Similarly, the modification of fields that form a value object’s identity would, too, turn that value object into a different one.

> But what about the remaining fields? If we can change an entity’s properties (except for the Id one), can’t we also change the fields of a value object, as long as they aren’t part of its identity?

> On the surface, it looks like we can, but this line of reasoning falls apart when you take into account the 3rd component that differentiates entities from value objects: history preservation.

## History preservation

> **History preservation** is whether or not an object has a history in your domain model.

> Entities have such a history (even though you might not store it explicitly). In other words, entities *live in a continuum*: they are created, modified, and deleted — all at different points in time. Value objects don’t have a history; they are a mere snapshot of some state.

> The modification of a value object implicitly extends its lifetime beyond just being a snapshot. Such a modification assumes the value object also has a history, which goes against the requirement of not preserving history in value objects.

> History preservation is what answers the question of "*Can you treat Entity and Value Object as the same concept?*". That answer is **no**.

## Summary

> The answer to the question of "Can we consider an entity as a value object that compares only its Id for equality?" boils down to three parts:

> - In terms of **identity comparison**, the answer is **yes**.
> - In terms of **immutability**, the answer is **yes**.
> - In terms of **history preservation**, the answer is **no**.

> Thus, the overall answer is also **no**.
