# WispFramework

## The Goal
The goal of this project is to facilitate development of a robust code-base by providing tools, utilities, and
extensions.

## Example Code

### Subject Observable Pattern

```csharp
// a subject whose state can be observed and throws an event when changed
Sub<bool> hasCoffee = new Sub<bool>(false); 
// triggers the ValueChanged event
hasCoffee.Value = true; 
```

### ExpirableCache

```csharp
var cache = new ExpirableCache<Guid, string>(TimeSpan.FromSeconds(1f)); 
// when expiration occurs, the item is removed from the cache
cache[k1] = "I will expire in 1 second"; 
```

### Dynamic Object

```csharp
// initialize a dynamic that generates a random string when Value is requested
var dyn = new Dynamic<string>(() => RandomUtil.RandomString(1, 5));

// will print a random string every time
for (int i = 0; i < 10; i++)
{ 
    Console.WriteLine(dyn.Value);
}

// when throttled we use the cached data until expiry time
dyn = new Dynamic<string>(() => RandomUtil.RandomString(1, 5))
    .Throttle(TimeSpan.FromSeconds(1));

// will print the same value unless 1 second has elapsed, in which case a new value will be generated
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(dyn.Value);
}
```

### Sub and EventAwaiter

```csharp
public static async void Run()
{
    // we do not have coffee
    Sub<bool> hasCoffee = new Sub<bool>(false); 
    Task.Run(() => NewMethod(hasCoffee));
    
    string input;

    // if input is 'give coffee' set hasCoffee value to true
    while ((input = Console.ReadLine()) != "exit")
    {
        if (input == "give coffee")
        {
            hasCoffee.Value = true;
        }
    }
}

private static async Task NewMethod(Sub<bool> hasCoffee)
{
    // here we await for the ValueChanged event to occur
    var waitForCoffee = new EventAwaiter<ValueChangedEventArgs<bool>>(
        h => hasCoffee.ValueChanged += h,
        h => hasCoffee.ValueChanged -= h);

    await waitForCoffee.Task;

    try
    {
        Console.WriteLine($"Value changed for hasCoffee to {hasCoffee}");
    }
    catch (TimeoutException)
    {
        Console.WriteLine("We did not get coffee in time!");
    }
}
```

### Containers

```csharp
Container ct = new Container();

ct.Register(new Car());
Console.WriteLine("Car found: " + ct.Resolve<Car>().Name);

ct.Register(new Boat());
Console.WriteLine("Boat found: " + ct.Resolve<Boat>().Name);

var specialBoat = new Boat() { Name = "Boat2" };
ct.Register(specialBoat, "Boat2");

var result = ct.Resolve<Boat>("Boat2");
result.cont.Register(result);
    
Console.WriteLine($"Special boat: {result.Name}");

Console.WriteLine($"Special boat: {result.cont.Resolve<Boat>().Name}");

var vehicles = ct.ResolveMany<IVehicle>().Select(t => t.Name);
var vehicleListStr = String.Join((string) ",", (IEnumerable<string>) vehicles);
Console.WriteLine($"Resolved IVehicles {vehicleListStr}");
Console.ReadLine();
```

### Lazy Factory

```csharp
var lazyFactory = new LazyFactory<int, Dynamic<string>>()
    .SetInitializer(i =>
{
    return new Dynamic<string>()
        .SetEvaluator(() => $"Dyn {i}: " + " " +
                            RandomUtil.RandomString(RandomUtil.Range(1,
                                10)))
        .Throttle(TimeSpan.FromSeconds(5));
});

Task.Run(async () =>
{
    while (true)
    {
        for (var i = 0; i < 3; i++)
        { 
            await Task.Delay(100);
            Console.WriteLine(lazyFactory[i].Value);
        }
    }
});
```

### Task Extensions

```csharp
var time = DateTime.Now;
await Task.Run(async () =>
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
    })
    .TimeoutAfter(TimeSpan.FromSeconds(1));
Assert.IsTrue(DateTime.Now - time < TimeSpan.FromSeconds(2));
```