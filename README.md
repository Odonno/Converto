# Converto

| Package           | NuGet                                                                                                         |
|-------------------|---------------------------------------------------------------------------------------------------------------|
| Converto          | [![NuGet](https://img.shields.io/nuget/v/Converto.svg)](https://www.nuget.org/packages/Converto/)                 |
| Converto.SuccincT | [![NuGet](https://img.shields.io/nuget/v/Converto.SuccincT.svg)](https://www.nuget.org/packages/Converto.SuccincT/) |

Converto is a C# library which gives you basic functions for type conversion and object transformation.

## Getting started

### Copy

The `Copy` function allows you to strictly copy an object.

```csharp
var newObject = existingObject.Copy();
```

```csharp
if (existingObject.TryCopy(out newObject))
{
}
```

#### Using SuccincT library

```csharp
var newObjectOption = existingObject.TryCopy();
var newObject = newObjectOption.Value;
```

### With

The `With` function allows you to create a new object by mutating some properties.

```csharp
var newObject = existingObject.With(new { Name = "Hello" });
```

```csharp
if (existingObject.TryWith(new { Name = "Hello" }, out newObject))
{
}
```

#### Using SuccincT library

```csharp
var newObjectOption = existingObject.TryWith(new { Name = "Hello" });
var newObject = newObjectOption.Value;
```

### ConvertTo

The `ConvertTo` function allows you to create an object of a different type using the matching properties of another object.

```csharp
var newObject = objectOfTypeA.ConvertTo<TypeB>();
```

```csharp
if (objectOfTypeA.TryConvertTo<TypeB>(out newObject))
{
}
```

#### Using SuccincT library

```csharp
var newObjectOption = objectOfTypeA.TryConvertTo<TypeB>();
var newObject = newObjectOption.Value;
```

### IsDeepEqual

The `IsDeepEqual` function detects if two objects have strictly the same properties (not necessarily the same object).

```csharp
bool isDeepEqual = IsDeepEqual(object1, object2);
```