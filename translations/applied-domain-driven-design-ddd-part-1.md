# 应用领域驱动设计, 第一章 - 基础 Applied Domain-Driven Design (DDD), Part 1 - Basics

原文: [Applied Domain-Driven Design (DDD), Part 1 - Basics](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-1.html)

> When I started learning domain-driven design there was a lot of theory to take in, Eric Evans did a great job explaining it from theoretical point of view. As a software engineer I wanted to see some code and just to follow some examples, I found very little resource out there that showed applied domain-driven design in C#.

当我开始学习领域驱动设计时, 有大量的理论需要吸收理解, Eric Evans 从理论的视角做了非常好的解释. 作为一个软件开发者我想查看一些代码和效仿一些示例, 但是, 我却发现很少有资源用 C# 去展示领域驱动设计的应用.

> Over the coming weeks I will be posting series of articles on the subject, it will be my attempt to make domain-driven design simpler and easier follow. Articles that are published:

接下来的几周¹我将会发表关于这个主题 (领域驱动设计) 的系列文章, 我将尝试让领域驱动设计更简单和更容易理解. 这系列的文章在这里:

- [应用领域驱动设计, 第〇章 - 需求和建模](applied-domain-driven-design-ddd-part-0.md)
- [应用领域驱动设计, 第一章 - 基础](applied-domain-driven-design-ddd-part-1.md)
- [应用领域驱动设计, 第二章 - 领域事件](http://www.zankavtaskin.com/2013/09/applied-domain-driven-design-ddd-part-2.html)
- [应用领域驱动设计, 第三章 - 规范模式](http://www.zankavtaskin.com/2013/10/applied-domain-driven-design-ddd-part-3.html)
- [应用领域驱动设计, 第四章 - 基础设施](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4_16.html)
- [应用领域驱动设计, 第五章 - 领域服务](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4.html)
- [应用领域驱动设计, 第六章 - 应用服务](http://www.zankavtaskin.com/2013/11/applied-domain-driven-design-ddd-part-4.html)
- [应用领域驱动设计, 第七章 - 读模型 (Read Model)](http://www.zankavtaskin.com/2016/06/applied-domain-driven-design-ddd-part-7.html)
- [应用领域驱动设计, 番外篇 - 五个最佳实践](https://www.codeproject.com/Articles/1131462/Domain-Driven-Design-My-Top-Best-Practices)
- [应用领域驱动设计, 番外篇 - 事件日志 & 审计资源](http://www.zankavtaskin.com/2016/08/applied-domain-driven-design-ddd-event.html)

![领域驱动设计架构 (Domain Driven Design Architecture)](./images/applied-domain-driven-design-ddd-part-1/DDD_png_pure.png)

**领域驱动设计架构 (这看起来比较简单) [Domain Driven Design Architecture (it's simpler then it looks)]*

## 1. 在开始之前, 让我们先看看领域驱动设计为何如此出色 (Before we get started let's see why DDD is so great)

> - **Development** becomes domain oriented not UI/Database oriented

- **开发** 转向面对领域而非面对 UI 或数据库开发.

> - **Domain layer** captures all of the business logic, making your service layer very thin i.e. just a gateway in to your domain via DTO's

- **领域层** 捕获了所有的业务逻辑, 让服务层十分精简轻薄; 服务层只是领域的网关, 仅通过数据传输对象 (DTO).

> - **Domain oriented development** allows you to implement true service-oriented architecture i.e. making your services reusable as they are not UI/Presentation layer specific

- **面向领域的开发** 让你实现真正的面向服务架构, 即让你的服务可重用化; 因为服务不特定于 UI 或表示层.

> - **Unit tests** are easy to write as code scales horizontally and not vertically, making your methods thin and easily testable

- **单元测试** 更加容易编写. 因为代码是横向扩展², 而非纵向扩展³; 从而让你的方法轻薄且易于测试.

> - **DDD** is a set of Patterns and Principles, this gives developers a framework to work with, allowing everyone in the development team to head in the same direction

- **领域驱动设计** 是一组模式和原则, 它为开发者提供了一套可用的框架, 使团队中的每位开发者都朝着共同的方向前进.

> Through this series of articles I will be focusing on a simple and made up e-commerce domain.

通过这个系列的文章, 我会重点介绍一个简单且虚构的电子商城领域.

## 2. 那, 让我们来瞧一些代码 (So, let's see some code)

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

> Code above represents anemic classes. Some developers would stop here, and use these classes to pass data to service and then bind this data to the UI. Let's carry on and mature our models.

以上的代码表示了几个贫血类. 有些开发者到这一步就停下来了, 然后利用这些类传输数据给服务和绑定数据给 UI. 现在, 让我们的模型继续完善下去.

> When a customer shops online they choose items first, they browse around, and then eventually they will make a purchase. So we need something that will hold the products, lets call it a Cart, this object will have no identity and it will be transient.

当一个客户在线购物时, 他们会先选择商品, 然后再到处浏览这件商品, 最后他们才将购买. 因此, 我们需要某个东西去容纳商品, 把这个东西叫做购物车, 这个对象是没有 Id 且仅临时的.

## 3. 购物车是我的值对象 (Cart is my value object)

```csharp
public class Cart
{
    public List Products { get; set; }
}
```

> Cart simply contains a list of products. Customer can go ahead and checkout these products when they are ready.

购物车仅包含了一个产品的列表. 当客户已经挑选好产品时, 之后就可以付款了.

> We can use what was said above as a business case "Customer can go ahead and checkout these products when they are ready".

我们可将上述的情形: "当客户已经挑选好产品时, 之后就可以付款了" 作为一个业务.

## 4. 代码如下所示 (Code would look like)

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

> What's going on here? Customer checks-out the product and gets a purchase back. Normally in business context you get a receipt back, this provides basic information about the purchase, discounts, and acts as a guarantee that i can use to refer back to my purchase.

观察现在的变化; 客户对购物车里的商品进行付款, 然后获得了此交易. 在业务上下文中, 通常会收到回执; 此回执提供了有关订购, 折扣等基础信息; 还能作为我的购物的凭证的参考.

> I could rename Purchase to Receipt, but wait, what's does purchase mean in the business context?

我可以把 `交易` 重命名为 `收据`, 但是, 稍等下, `交易` 在业务上下文的含义是什么?

> *"to acquire by the payment of money or its equivalent; buy."* - Dictionary.com

*"以付款或等值的方式获得; 购买."* - Dictionary.com

> (Returning purchase object would make sense if customer actually made a purchase i.e. we pre-authenticated a customer and then simply passed payment authentication code to the checkout, but to keep this simple we are not going to do this)

(如果客户确实进行了交易, 返回交易对象是有意义的. 也就是, 我们预先认证客户, 然后简单地把付款码传递给结账; 但是为了保持简单, 我们不打算这样做.)

> Customer can now checkout, when they checkout they will get back a Purchase object (this will allow further data manipulation).

客户现在可以付款, 当他们付款时, 客户会获取一个交易对象 (这个将允许进一步的数据操作).

> Code above needs to be re-factored, if we return back a purchase are we going to then add it to the collection of the customer i.e. Customer.Purchases.Add(...)? This seems strange, if we have a method Customer.Checkout(...) we should aim to capture relevant data right there and then. Customer should only expose business methods, if we have to expose something else in order to capture data then we are not doing it right.

上述代码需要进行重构, 如果我们打算返回一个`交易`对象, 然后将它添加到`客户`的`交易集合`中, 即 `Customer.Purchaese.Add(...)`. 这显得太奇怪了, 如果我们有个方法 `Customer.Chcekout(...)`, 则我们的目标应该是立即捕获相关数据. `客户` 只应该暴露业务方法, 如果我们只为了捕获数据而不得不公开其它内容, 那么我们做得就不对了.

## 5. 让我们进一步改善 (Lets refine further)

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

> Ok, so now when customer checks-out a cart, purchase will be added to the purchase collection and also returned  so it can be used further in our logic. This is great, but another software engineer can go in and compromise our domain. They can just add Orders directly to the customer without checking out i.e. Customer.Orders.Add(...).

好了, 现在当客户对购物车里的商品付款时, 交易将会添加到交易集合中然后并返回此次交易, 以便于在我们业务逻辑中进一步使用. 这挺好的, 但是, 另外一个开发者能进入并破坏掉我们的领域. 他们可以直接向 `客户` 添加订单, 而不需要付款, 即 `Customer.Orders.Add(...)`.

## 6. 让我们更进一步改善 (Lets refine further)

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

> Now orders can't be compromised, and code forces software engineers to checkout a cart. What about other properties? They are not protected. We know customer state can't just be changed, we have to go through a process. You need to ask your self, when personal information is changed on the customer, do we need to send an email out? Do we need to call some 3rd party API to sync up our records? Right now you might not have a requirement from your business analysts to do anything like this, so lets not worry about it, lets just protect these fields so they can't be changed.

现在, 订单不能被破坏了; 代码强制开发者对 *购物车* 做 *付款* 操作.那么其它属性呢, 他们没有受到保护. 我们知道客户的状态不能仅被更改, 必须通过一个过程. 你需要问下你自己, 当改变客户的个人信息时, 我们是否需要给他发一封电子邮件? 是否需要调用第三方的 API 同步记录? 此刻, 业务分析师可能还没有对你作此类需求, 所以不必关心它, 仅仅保护这些字段使之不能被更改即可.

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

> That's great, now other software engineers in the team can't change personal information without adding a new method such as Customer.ChangeDetails(...).

这很好, 当没有添加新的方法, 比如 `Customer.ChangeDetails(...)`, 现在这个团队中的其它的开发者也不能修改个人信息了.

> **Taking in to account what was said above, thinking process, constant re-factoring and making the model match the actual business domain, this is what I've got so far:**

**综合以上所描述的, 思考过程, 不断重构; 使模型与真实业务领域相匹配, 这就是我目前所得到的代码:**

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

## 7. 用法示例 (Example usage)

```csharp
Cart cart = Cart.Create(new List() { new Product(), new Product() });
Customer customer = Customer.Create("josh", "smith", "josh.smith@microsoft.com");
Purchase purchase = customer.Checkout(cart);
```

## 8. 总结 (Summary)

> - DDD is all about capturing business logic in the domain i.e. entities, aggregate roots, value objects and domain service.

- 领域驱动设计就是要捕获领域中的业务逻辑; 即实体, 聚合根, 值对象和领域服务.

> - DDD is all about thought process and challenging where should what go and what is most logical.

- 领域驱动设计涉及思考的过程, 并挑战最符合逻辑的是什么, 并何去何从.

> - DDD is all about constant re-factoring and maturing your model as you get further requirements. More requirements your receive the better and stronger your domain will be. Therefore requirements are gold and something that software engineers should always strive to understand.

- 领域驱动设计是关于随着获得越来越多的需求, 而不断重构和让模型更加成熟. 当添加的需求越多, 领域就将越健壮. 因此, 需求是黄金, 开发者应该始终努力理解这些需求.

## 9. 一些有用链接 (Useful links)

> - [Onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/), a very good example of what domain-driven design is all about

- [洋葱架构](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/), 一个关于领域驱动设计的非常好的示例

> - [Aggregate root](http://martinfowler.com/bliki/DDD_Aggregate.html), great explanation of what aggregate root actually is

- [聚合根](http://martinfowler.com/bliki/DDD_Aggregate.html), 非常好地解释了聚合根是什么

> - [Explanation of Aggregate Root](http://lostechies.com/jimmybogard/2008/05/21/entities-value-objects-aggregates-and-roots/), Entity and Value objects

- [聚合根](http://lostechies.com/jimmybogard/2008/05/21/entities-value-objects-aggregates-and-roots/), 实体以及值对象的解释

> - [Supple Design, Intention revealing interfaces, etc](http://www.cs.colorado.edu/~kena/classes/6448/s05/lectures/lecture30.pdf). Pdf from University of Colorado

- [揭开柔性设计的面纱, 等等](http://www.cs.colorado.edu/~kena/classes/6448/s05/lectures/lecture30.pdf). 来自 Colorado 大学的 PDF 文档.

> **Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*

## 10. 精选评论⁴

*本系列译文将挑选原文中具有代表意义的评论及作者回答, 并仅带的译文, 若想阅读全部评论请导航到原文*.

1. *bmoc* 14 August 2014 at 23:54

    1. **评论**

        非常好的文章! 我们正在尝试使用领域驱动设计, 但是在架构上我还是有些不太确定的地方. 如果你能解答我的疑惑, 我会十分高兴 :).

        您是否能解释下 `领域驱动设计` 的依赖关系 (什么层依赖于什么层) 吗?

        当我从上往下阅读时, 我认为高层依赖于低层, 反过来则不是. 基础设施层则是个例外 - 它使用了领域层, 但是领域层不知道基础设施层.

        我能理解这些:

        1. 表示层, 使用或依赖于分布式接口层或应用层.
        2. 应用层使用了:
            1. 领域层(但是领域层对应用层一无所知).
            2. 基础设施层(基础设施层不知道应用层).
        3. 基础设施层使用了:
            1. 领域层
            2. 数据访问服务和外部服务

        我理解的对吗?

        你会把数据传输对象 (DTO) 放在什么地方? 应用层? 基础设施层? 还是都有?

        应用层的 DTO: 出于很多原因, 我不想把我的领域模型直接暴露给表示层, 所以在应用层创建 DTO, 并在应用层将领域对象转换成 DTO.

    2. **回答**

        感谢你的评论!

        1和2的回答: 是.

        3, 这是个很好的问题, 我想这里有几个问题.

        让我们先从基础设施层开始. 你的 "领域模型层" 甚至不应该知道 "基础设施层". 可通过把数据持久化和任何其它基础设施层的接口放在 "领域模型层" 来实现这一点. 仓储的真实实现位于基础设施层.

        你的基础设施层需要引用 "领域模型层" 以便于基础设施层知道它要处理的实体 / 对象.

        你无需在基础设施层中具有数据传输对象, 它增加了不必要的复杂性. 对于视图或者不属于领域的内容, 可以创建 "值对象", 请参考 [使用值对象表示状态和计算](./images/applied-domain-driven-design-ddd-part-1/Pattern-Language-Overview-med.png).

        数据传输对象之所以出色, 因为它们不包含任何逻辑或方法, 它们仅仅携带数据. 另外, 如果使用类似于 NHibernate 这样的 ORM 框架, 你会注意到它会使用反射向对象添加额外的行为. 最后要做的是将一个具有状态跟踪的实体和具有逻辑的方法发送到表示层, 这就是为什么它们为什么如此出色的原因(你也提到了这一点).

        因此, 如果你想要彻底的分离, 可以查询持久化数据, 获取实体或值对象, 并通过 AutoMapper 将其映射到 DTO. 即便是一个简单的视图查询, 我个人也是这么做的, 将使所有内容保持一致.

        相关的一些代码示例, 请参考: [GitHub](https://github.com/zkavtaskin/Domain-Driven-Design-Example)

        我希望这个回答对你有帮助.

2. *soeng kanel* 2 August 2016 at 17:17

    1. **评论**

        非常好的文章.

        您能解释下什么开发转变成面向领域, 而不是面向 UI/数据库吗? 为什么不面向数据库了?

    2. **回答**

        感谢你的评论!

        领域驱动设计让你的开发由于抽象, 所以更加面上领域且与技术无关. 看看 [洋葱模型](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/).

        一些 "经典" 的应用程序非常面向数据, 例如一些应用程序都涉及存储过程, 为了简单起见, 开发者们仅仅只是将存储过程的数据输出直接映射到 UI, 由于这些存储过程常常被其它的存储过程使用, 并随着业务流程的成熟, 开始包含越来越多的业务逻辑, 随着时间的推移, 这将造成维护的噩梦. 用户界面与数据库缠绕在一起, 数据库与业务逻辑缠绕在一起. 事情变得及其难以改变和理解, 没有人知道业务流程从哪里开始, 也没有人知道在哪里结束.

        N 层架构消除了这种令人困惑的业务逻辑和 UI 交织. 领域驱动设计通过善用面向对象编程和抽象基础设施层, 显著地改善了可维护性.

        我希望这个能回答你的疑问, 再次感谢你阅读此文.

3. *Alexander Kolev* 30 November 2017 at 09:31

    1. **评论**

        很好的文章. 不过我有个疑问: 什么您使用创建工厂方法创建新实例而不是使用传统的构造函数?

        在这种情况下我们会有

        ```csharp
        Cart cart = new Cart(new List() { new Product(), new Product() });
        Customer customer = new Customer("josh", "smith", "josh.smith@microsoft.com");
        Purchase purchase = customer.Checkout(cart);
        ```

    2. **回答**

        你好, Alexander,

        这有一半是偏好, 这么做有两个原因:

        1. 如果使用构造方法创建一个购物车实例, 并且触发领域事件; 那么哪个领域事件应该被调用, `CartCreating` 还是 `CartCreated`? 从技术角度而言, 如果构造器没有完成构造购物车, 那么购物车是不会被创建的.

        2. 构造方法应该是轻量的, 只应该创建对象; 即使领域事件是发布/订阅解耦的模式, 它仍然是同步的. 我是真心不喜欢在构造函数执行处理一些发送电子邮件, 审计操作等等.

## 脚注

[1] 作者是在 2013 年 09 月 26 日开始写下这一系列文章的.

[2] 横向扩展, 又叫水平扩展, 用更多的节点支撑更大量的请求.

[3] 纵向扩展, 又叫垂直扩展, 扩展一个点的能力支撑更大的请求.

[4] 限于译者水平及时间, 评论翻译未作校对, 若有疑问请参考原文链接.
