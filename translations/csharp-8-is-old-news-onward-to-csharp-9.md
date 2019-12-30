# C# 8 is old news. Onward, to C# 9! (C# 已成旧闻, 向前, 抵达 C# 9!)

> [Did you know that planning is already underway for the ninth version of the C# language?](https://github.com/dotnet/csharplang/milestone/15)

[第九版 C# 语言已经在开发中了, 你晓得伐?](https://github.com/dotnet/csharplang/milestone/15)

> Now, to be fair, this has been in the planning phases long, **LONG**, before C# 8 shipped to us back in September 2019, as you can see from some of the discussion on the issues themselves. Most folks don’t follow the day-to-day planning of the language itself (myself included), but it’s interesting to peer into the discussions every now and then.

现在, 公平地讲, 在 C# 8 于2019年9月交付给开发者之前, 这个已经在计划阶段 **很久很久很久** 了. 可以从一些关于 C# 9 的 issues 的讨论中看到. 大多数人不太会遵循语言本身的日常规划(连我自己都是), 但时不时地参与下讨论还是蛮有意思.

> And, since this is Christmas Day, let’s peek in on five (there are a LOT more!) C# 9 language “gifts” we might receive sometime in 2020 (subject to change, of course!)

并且, 由于今天是圣诞节(作者是2019年12月25日写的这篇文章), 所有让我们悄悄咪咪地看下五个 (还有**更多**!) 关于C# 9 的 **"礼物"**, 我们很有可能在2020年的某天收到(当然, 可能也不会!).

## 1. [Simplified Parameter NULL Validation Code (简化 NULL 参数验证代码)](https://github.com/dotnet/csharplang/issues/2145)

> The short version is that by decorating the value of a parameter to a method with a small annotation, we simplify the internal logic by not needing null validation / guard clauses, thus reducing boilerplate validation code. For example:

简化版是通过在 `方法参数` 上带有一个小注解以装饰 `参数`, 这样就不需要做 null 检测或守卫子句, 从而简化了内部的逻辑, 减少样板验证代码. 比如:

```csharp
// Before (之前)
void Insert(string s) {
 if (s is null) {
   throw new ArgumentNullException(nameof(s));
 }
 ...
}

// After (现在, 参数后面加了个叹号)
void Insert(string s!) {
  ...
}
```

## 2. [Switch expression as a statement expression (Switch 表达式作为语句表达式)](https://github.com/dotnet/csharplang/issues/2632)

> This one is still in the discussion phase, but the general idea is to allow a switch expression as an expression statement. For example:

这个仍处于讨论阶段, 但总体思路是允许将 switch 表达式作为 expression 语句. 例如:

```csharp
private void M(bool c, ref int x, ref string s)
{
  c switch { true => x = 1, false => s = null };
}
```

> or

或者

```csharp
private void M(bool c, ref int x, ref string s)
  => c switch { true => x = 1, false => s = null };
```

## 3. [Primary Constructors (主构造函数)](https://github.com/dotnet/csharplang/issues/2691)

> This one means to simplify all those boilerplate constructors, fields, property getters/setters, etc., that we’re so used to. For example:

意味着简化所有的我们之前习惯的那些样板构造函数, 字段, 属性存取器等. 例如:

```csharp
// From This: (从这种形式)
class Person
{
    private string _firstName;

    public Person(string firstName)
    {
        _firstName = firstName;
    }

    public string FirstName
    {
        get => _firstName;
        set {
          if (value == null) {
              throw new NullArgumentException(nameof(FirstName)); 
          }
          _firstName = value;
        }
    }
}

//To This: (到这种形式)
class Person(string firstName)
{
    public string FirstName
    {
        get => firstName;
        set {
          if (value == null){
            throw new NullArgumentException(nameof(FirstName));
          }
          firstName = value;
        }
    }
}
```

## 4. [Record (记录)](https://github.com/dotnet/csharplang/issues/39)

Slightly similar in nature to Primary Constructions (mentioned above), the goal of this proposal is to remove the necessity of writing so much boilerplate code when creating a new class / struct. It seems possible that if Record types make it in, that Primary Constructors will not (opinion). For example:

与上面提到的基本结构在本质上稍微相似, 该建议的目的是在创造新类/结构体时, 消除编写大量必要的样板代码. 如果 `记录` 类型出现, 那么 `主构造函数` 就不再有了(意见). 例如:

```csharp
//From Something Like This: (从类似于这样的)
public class Person
{
  public string Name { get; }
  public DateTime DateOfBirth { get; }

  public Person(string Name, DateTime DateOfBirth)
  {
    this.Name = Name;
    this.DateOfBirth = DateOfBirth;
  }
}

//To Something Like This (到类似于这样)
public class Person(string Name, DateTime DateOfBirth);
```

## 5. [Discriminated Unions / enum class (可辨识联合 / 枚举类)](https://github.com/dotnet/csharplang/blob/master/proposals/discriminated-unions.md)

> This one took a smidge to wrap my brain around. It uses keywords that we’re all used to (plus a new one), combines them together, and adds Record’s (mentioned above) into the mix. It is, in my own words, a “cleaner” way of creating abstract base classes, and concrete types that inherit from them. For example:

这个让我有点摸不着头脑. 它使用我们都惯用的关键字(加上一个新关键字), 将它们组合在一起, 并将记录 (上面提及的) 添加到混合中. 用我们自己话说, 它是创建抽象基类和继承自它们的具体类型的 "更简洁" 的方法. 比如:

(译者注: Discriminated Unions, 可辨识联合/可区分的联合. 也称变体类型(variant type), 通常是某一个枚举类型, 包含一个联合和一个标签的数据结构. 能够存储一组不同但是固定的类型中某个类型的对象, 具体是哪个类型由标签字段决定. 这种数据结构在解释器, 数据库和数据通信中非常有用. 链接: [标签联合](https://en.wikipedia.org/wiki/Tagged_union))

```csharp
// From Something Like This: (从类似于这样的)
public partial abstract class Shape { }

public class Rectangle : Shape {

  public float Width { get; }
  public float Length { get; }

  public Rectangle(float Width, float Length){
    this.Width = Width;
    this.Length = Length;
  }
}

public class Circle : Shape {

  public float Radius { get; }

  public Circle(float Radius)
  {
    this.Radius = Radius;
  }
}

// To Something Like This: (到类似于这样的)
enum class Shape
{
  Rectangle(float Width, float Length);
  Circle(float Radius);
}
```

> These five proposals only skim the surface of the discussions going on around C# 9. Head over to the GitHub project, take a look, and even get involved! It’s a new Microsoft, and our opinions are welcome!

这五个提案仅仅只是围绕 C# 9 正在进行的讨论做的简要介绍, 可以到 GitHub 项目看一看, 甚至可以参与进来! 这是一个新的全新的微软, 我们的意见将受到欢迎!

> Since I’m the final post of the year, I’d like to offer a major thank you to everyone that volunteered, wrote, tweeted, retweeted, liked, hearted, shared, read, etc., to this amazing event.

由于这是我今年最后一篇博文, 因此我要向在这项令人赞叹的活动中自干五的人致以深深的感谢.

> This community…OUR community, is comprised of amazing folks, and I consider myself grateful to even be considered a part of it.

这个社区, 我们的社区, 是由非常棒棒的人组成的, 我超开心自己被认为是这个社区的中的一份子.

> Whatever holiday you celebrate this time of year, I hope it’s wonderful.

不管每年的这个圣诞节庆祝什么, 我都希望它是棒棒哒.

> See you next year, friends.

朋友们, 明年见!
