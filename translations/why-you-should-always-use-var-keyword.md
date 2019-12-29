# [译文] Why You Should Always Use the ‘var’ Keyword in C# (为什么你总是应该在 C# 里使用 "var" 关键字)

> Using the ‘var’ keyword in C# has always spurred a hot debate among developers. I believe ‘var’ should be used at all times. I believe this not because I choose to be “lazy,” as those who argue against it frequently claim. Of all the reasons I use ‘var’, laziness is not one of them.

在 C# 中使用 `var` 关键字一直引起开发人员的激烈争论, 我认为应始终使用 `var`. 我相信这不是因为我选择变得 **懒惰**, 正如经常声称反对它的人的那样, 在我使用 `var` 的所有原因中, 懒惰不是其中之一.

> I’ve argued for the constant use of ‘var’ countless times; this blog post is a collection of thoughts that I have compiled resulting from my arguments. Below are my reasons for using ‘var’ all of the time.

我主张无时无刻地使用 `var`, 这篇博客文章是我根据自己的论点整理出的想法的集合, 以下是我一直使用 `var` 的原因.

[原文](http://blog.michaelbrennan.net/2010/06/why-you-should-always-use-var-keyword.html)

## It decreases code-coupling (减少代码耦合)

> Coupling between code and its dependent code can be reduced by using ‘var’. I do not mean coupling from an architectural perspective nor at an IL-level (the type is inferred anyway), but simply at the code level.

使用 `var` 可以减少代码与其从属代码之间的耦合, 我的意思是, 不是从体系结构的角度耦合, 也不是在IL级别(无论如何推断类型), 而是在代码级别.

## Example (示例)

> Imagine there are 20 explicit type references spanning over twenty code files to an object that returned an another object of type IFoo. By explicit type references, I mean by prefacing each variable name with IFoo. What happens if IFoo changes to IBar, but the interface’s methods are kept the same?

假设有 20 个显式类型引用, 跨越 20 多个代码文件, 返回一个类型为 `IFoo` 的对象. 通过显式类型引用, 我的意思是在每个变量名前面加上 `IFoo`, 如果将 `IFoo` 更改为 `IBar`, 接口的方法保持不变, 会发生什么?

> Wouldn’t you have to change it in 20 distinct places? Doesn’t this increase coupling? If ‘var’ was used, would you have to change anything? Now, one could argue that it is trivial to change IFoo to IBar in a tool like ReSharper and have all of the references changed automatically. However, what if IFoo is outside of our control? It could live outside the solution or it could be a third-party library.

是否需要在 20 个不同的地方就行修改? 这不就是增加了耦合? 若使用 `var`, 需要修改什么吗? 现在, 有人可能会争辩道, 在 ReSharper 之类的工具中将 `IFoo` 修改为 `IBar` 并不重要, 且可以自动更正所有引用. 但是, 倘若 `IFoo` 超出我们的控制范围了? 它可以存在与解决方案之外, 也可以是第三方库.

## It is completely redundant with any expression involving the "new" operator (它对于任何涉及 "new" 运算符的表达式都是完全多余的)

> Especially with generics:

尤其是在泛型:

``` csharp
ICalculator<GBPCurrency, GBPTaxType> calculator = new GBPCalculator<GBPCurrency, GBPTaxType>();
```

> can be shorted to:

可以缩写为:

``` csharp
var calculator = new GBPCalculator<GBPCurrency, GBPTaxType>();
```

> Even if calculator is returned from a method (such as when implementing the repository pattern), if the method name is expressive enough it is obvious the object is a calculator. The name of the variable should be expressive enough for you to know what the object it represents is. This is important to realize: the variable expresses not what type it represents, but what the instance of that type actually is. An instance of a type is truly an object, and should be treated as such.

即使计算器是从方法返回的 (例如, 在实现仓储模式时), 如果方法名具有足够的表达力, 则对象显然是个计算器. 变量的名称应该具有足够的表达力, 让人知道它所表达的对象是什么. 认识到这点十分重要: 变量表示的不是它所表示的类型, 而是该类型的实例实际上是什么. 一个类型的实例确实是一个对象, 应该这样对待.

> There is a distinction between an object and its type: an object exists at runtime, it has properties and behaviors; types simply describe what an object should be. Knowing what type an object should be simply adds more noise to the source code, distracting the coder from what an object really is.

对象和它的类型之间有个区别: 对象的属性和行为存在于运行时, 类型只是简单地表述了对象应该是什么, 知道对象应该是什么类型, 只会给源代码带来更多的干扰, 从而是开发者分心, 无法真正了解对象是什么.

> An object may be brought into this world by following the rules governed by a type, but this is only secondary information. What the object actually is and how it behaves is more important than its type. When we use an object at runtime, we are dependent on its methods and properties, not its type. These methods and properties are an object’s behaviors, and it is behaviors we are dependent upon.

对象可以通过遵循由类型控制的规则而引入, 但这仅仅是辅助信息, 对象实际是什么以及其行为比其类型更重要. 当我们在运行时使用一个对象时, 依赖于它的方法和属性, 而不是它的类型. 这些方法和属性是对象的行为, 也是我们所依赖的行为.

> The argument for knowing a variable's type has been brought up in the past. The move from Hungarian notation in Microsoft-based C++ to non-Hungarian notation found in C# is a great example of this once hot topic. Most Microsoft-based C++ developers at the time felt putting type identifiers in front of variable names was helpful, yet Microsoft published coding standards for C# that conflicted with these feelings.

过去曾提出了解变量类型的争议, 从基于微软的 C++ 中的匈牙利命名法到 C# 中的非匈牙利命名法的转变, 就是这个曾经热门话题的一个很好的例子. 当时, 大多数基于微软的 C++ 开发者都认为将类型标识符放在变量之前是有帮助的, 然而, 这与微软发布的 C# 编码标准与之相冲突.

> It was a major culture change and mind-shift to get developers to accept non-Hungarian notation. I was among those who thought that non-Hungarian variable naming was downright wicked heresy and anyone following such a practice must be lazy and did not care about their profession.  If knowing a variable’s type is so important, shouldn’t we then preface variable names in Hungarian style to know more information about an object's type?

让开发者接受非匈牙利命名法是个重大的文化变革和思想转变. 我曾经就认为, 任何遵循非匈牙利变量命名法的人一定是懒惰的, 不管他是什么职业, 是彻头彻尾邪恶的异端邪教. 如果知道变量的类型是非常重要的, 那么我们难道不应该以匈牙利命名法的风格前缀变量名以了解有关对象的类型的更多信息吗?

## You shouldn't have to care what the type of an object is (不必关心对象的类型)

> You should only care what you are trying do with an object, not what type an object may come from. The methods you are attempting to call on an object are its object contract, not the type. If variable names, methods, and properties are named appropriately, then the type is simply redundant.

应该专注于利用对象做什么, 而非关注对象来自何种类型. 尝试调用对象的方法是对象契约, 而非类型. 若对变量名, 方法和属性作了恰当的命名, 那么类型则是冗余.

> In the previous example, the word "calculator" was repeated three times. In that example, you only need to know that the instance of a type (the object) is a calculator, and it allows you to call a particular method or property.

如前例所示, 单词 `calculator` 重复了三次, 示例中, 只需要知道类型 (对象) 的实例是一个 `calculator` (计算器), 并且它允许调用特定的方法或者属性.

> The only reason a calculator object was created was so that other code could interact with its object contract. Other code needs the calculator’s methods and properties to get something done. This need has no dependency on any type, only on an object’s behaviors..

创建 `calculator` (计算器)对象的惟一原因是, 其他代码可以与其对象契约进行交互. 其他代码需要计算器的方法和属性来完成一些工作. 这种需求不依赖于任何类型, 只依赖于对象的行为.

> For example, as long as the object is a calculator, and the dependent code needs to call a method named ,” then the dependent code is coupled to an object with a method called “CalculateTax” and not a specific type. This allows for much more flexibility, because now the variable can reference any type as long as that type supports the “CalculateTax” method.

比如, 依赖代码需要调用一个名为 "`CalculateTax`" 的方法, 对象只要是个 `calculator` (计算器), 那么依赖代码就会与一个名为 `CalculateTax` 的方法 (而不是特定类型) 耦合到一个对象上, 这允许更大的灵活性, 因为现在变量可以引用任何类型, 只要该类型支持 `CalculateTax` 方法.

## ‘var’ is less noisy than explicitly referencing the type (与显式引用类型相比, “var”的噪声更小)

> As programming languages evolve, we spend less time telling the compiler and the computer what to do and more time expressing problems that exist in the specific domain we are working in.

随着编程语言的发展, 我们花更少的时间告诉编译器和计算机该做什么, 而是花更多的时间来表达我们工作的存在于特定领域的问题.

> For example, there are a number of things in C++ that are very technical with respect to the machine, but have nothing to do with the domain. If you are a customer of Quicken or Microsoft Money, all you really want to do is manage your finances better. These software packages allow you to do that.

比如, 在 C++ 中, 有许多与计算机相关的技术问题, 却与特定领域无关. 如果你是 Quicken (一款家庭及个人财务管理软件) 或 Microsoft Money 的用户, 你真正想做的只是更好地管理你的财务. 这些软件也正是让你这样做的.

> The better a software package can do this for you, the more valuable it is to you. Therefore, from a development perspective value is defined by how well a software package solves a user's problem. When we set out to develop such software, the only code that is valuable is the code that contributes to solving a particular user’s problem. The rest of the code is unfortunately a necessary waste, but is required due to limitations of technology.

一个软件为你做得越好, 它对你就越有价值. 因此, 从开发的角度来看, 价值是由软件解决用户问题的能力来定义的. 当我们开始开发这样的软件时, 唯一有价值的代码是帮助解决特定用户问题的代码. 遗憾的是, 剩下的代码是必要的浪费, 这是由于技术限制而必需的.

> If we had infinite memory, we would not need to worry about deleting pointers in C++ or garbage collection in C#. However, memory is a limitation and therefore the technician in us has to find ways of coping with this limitation.

如果我们的内存是无限的, 就不必担心删除 C++ 中的指针或 C# 中的垃圾回收. 然而, 内存是有限的, 因此我们的技术人员必须寻找对付这种限制的办法.

> The inclusion of ‘var’ into the C# language was done for a reason and bookmarks another iteration of C# (particularly C# 3.0). It allows us to spend less time telling the compiler what to do and more time thinking about the problem we are trying to solve.

将 "var" 包含在 C# 语言中是有原因的, 标志着 C# 的一次革新 (尤其是 C# 3.0). 它让我们可以花更少的时间告诉编译器应该做什么, 花更多的时间思考我们需要解决的问题.

> Often I hear dogma like "use var only when using anonymous types." Why then should you use an anonymous type? Under these conditions you usually do not have a choice, such as when assigning variables to the results of LINQ expressions. Why do you not have a choice when using LINQ expressions? It's because the expression is accomplishing something more functional and typing concerns are the least of your worries.

经常听到这样的教条: "只能在使用匿名类型时才能 `var`." 那么为什么要使用匿名类型呢? 在这些情况下, 通常没有选择的余地, 比如在将变量分配给 LINQ 表达式的结果时. 为什么在使用 LINQ 表达式时没得选? 这正是因为表达式实现了一些更为实用的功能, 且类型问题是最不需要担心的.

> In the ideal C# world, we would not have to put any words in front of a variable name at all. In fact, prefacing a variable with anything just confuses the developer even further, and allows for poor variable names to become a standard whereby everyone is reliant upon explicit type references.

在理想的 C# 编程环境中, 我们压根就不需要在变量名前缀任何单词. 实际上, 在变量前缀任何东西只会使开发者更加困惑, 并让糟糕变量名成为每个开发者都在显式依赖引用的标准.

## Arguments against using ‘var’ (反对使用“var”的论据)

> Some of the arguments I have heard against using ‘var’ and my responses to these are:

我看到一些反对使用 `var` 的论点, 对此我的回答是：

> - “It reduces clarity” – How? By removing the noise in front of a variable name, your brain has only one thing to focus on: the variable name. It increases clarity.

"它降低清晰度" - 为什么? 通过消除变量前面的噪音, 大脑只需要关注唯一的一件事: 变量名. 它增加了清晰度.

> - “It reduces readability and adds ambiguity” – This is similar to #1: readability can be increased by removing words in front of the variable and by choosing appropriate variable names and method names. Focusing on type distracts you from the real business problem you are trying to solve.

"它降低了可读性, 同时增加了模糊性" - 这与第一条相似: 可读性可以通过删除变量前面的单词, 以及选择适当的变量名和方法名来提高. 专注于类型会分散对要解决的实际业务问题的注意力.

> - “It litters the codebase” – This is usually an argument for consistency. If your codebase uses explicit type references everywhere, then by all means do not use ‘var’. Consistency is far more important. Either change all explicit references in the codebase to ‘var’ or do not use ‘var’ at all. This is a more general argument that applies to many more issues, such as naming conventions, physical organization policies, etc.

"它让代码库变得乱糟糟" - 这大概是一致性的理由. 如果现存的代码库已经使用显示类型引用, 那么别使用 `var`, 一致性更重要. 要么将代码库中所有显示引用修改为 `var`, 要么从不使用 `var`. 这是个更普遍的观点, 适用于很多情形, 比如命名约定, 物理文件组织策略等等.

> As a final thought, why do we preface interface names with “I” but not class names with “C” as we did in the days when Microsoft-C++ was the popular kid in school?

最后思考下, 当在 Microsoft C++ 在学校大受欢迎时, 为什么我们要在接口名前缀 "I" 而不在类名前缀 "C"?
