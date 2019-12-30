# C# 8 is old news. Onward, to C# 9!

[Did you know that planning is already underway for the ninth version of the C# language?](https://github.com/dotnet/csharplang/milestone/15)

Now, to be fair, this has been in the planning phases long, **LONG**, before C# 8 shipped to us back in September 2019, as you can see from some of the discussion on the issues themselves. Most folks don’t follow the day-to-day planning of the language itself (myself included), but it’s interesting to peer into the discussions every now and then.

And, since this is Christmas Day, let’s peek in on five (there are a LOT more!) C# 9 language “gifts” we might receive sometime in 2020 (subject to change, of course!)

## 1. [Simplified Parameter NULL Validation Code](https://github.com/dotnet/csharplang/issues/2145)

The short version is that by decorating the value of a parameter to a method with a small annotation, we simplify the internal logic by not needing null validation / guard clauses, thus reducing boilerplate validation code. For example:

```csharp
// Before
void Insert(string s) {
 if (s is null) {
   throw new ArgumentNullException(nameof(s));
 }
 ...
}

// After
void Insert(string s!) {
  ...
}
```

## 2. [Switch expression as a statement expression](https://github.com/dotnet/csharplang/issues/2632)

This one is still in the discussion phase, but the general idea is to allow a switch expression as an expression statement. For example:

```csharp
private void M(bool c, ref int x, ref string s)
{
  c switch { true => x = 1, false => s = null };
}
```

or

```csharp
private void M(bool c, ref int x, ref string s)
  => c switch { true => x = 1, false => s = null };
```

## 3. [Primary Constructors](https://github.com/dotnet/csharplang/issues/2691)

This one means to simplify all those boilerplate constructors, fields, property getters/setters, etc., that we’re so used to. For example:

```csharp
// From This:
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

//To This:
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

## 4. [Record](https://github.com/dotnet/csharplang/issues/39)

Slightly similar in nature to Primary Constructions (mentioned above), the goal of this proposal is to remove the necessity of writing so much boilerplate code when creating a new class / struct. It seems possible that if Record types make it in, that Primary Constructors will not (opinion). For example:

```csharp
//From Something Like This:
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

//To Something Like This
public class Person(string Name, DateTime DateOfBirth);
```

## 5. [Discriminated Unions / enum class](https://github.com/dotnet/csharplang/blob/master/proposals/discriminated-unions.md)

This one took a smidge to wrap my brain around. It uses keywords that we’re all used to (plus a new one), combines them together, and adds Record’s (mentioned above) into the mix. It is, in my own words, a “cleaner” way of creating abstract base classes, and concrete types that inherit from them. For example:

```csharp
// From Something Like This:
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

// To Something Like This:
enum class Shape
{
  Rectangle(float Width, float Length);
  Circle(float Radius);
}
```

These five proposals only skim the surface of the discussions going on around C# 9. Head over to the GitHub project, take a look, and even get involved! It’s a new Microsoft, and our opinions are welcome!

Since I’m the final post of the year, I’d like to offer a major thank you to everyone that volunteered, wrote, tweeted, retweeted, liked, hearted, shared, read, etc., to this amazing event.

This community…OUR community, is comprised of amazing folks, and I consider myself grateful to even be considered a part of it.

Whatever holiday you celebrate this time of year, I hope it’s wonderful.

See you next year, friends.

## Final Note

A big thank you goes out to Matthew D. Groves for bringing this amazing tradition to life (now in its third year)!

I consider myself lucky to call Matthew a friend, and our community is a better place because of him.

Make sure and follow Matt on Twitter, and watch his live streams on Twitch. You won’t be disappointed.

Merry Christmas, Matt.