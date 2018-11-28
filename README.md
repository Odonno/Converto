# Converto

Converto is a C# library which gives you basic functions for type conversion and object transformation.

## Copy

The `Copy` function allows you to strictly copy an object.

```csharp
var newObject = existingObject.Copy();
```

```csharp
if (existingObject.TryCopy(out newObject))
{
}
```

### Using SuccincT library

```csharp
var newObjectOption = existingObject.TryCopy();
var newObject = newObjectOption.Value;
```

## With

The `With` function allows you to create a new object by mutating some properties.

```csharp
var newObject = existingObject.With(new { Name = "Hello" });
```

```csharp
if (existingObject.TryWith(new { Name = "Hello" }, out newObject))
{
}
```

### Using SuccincT library

```csharp
var newObjectOption = existingObject.TryWith(new { Name = "Hello" });
var newObject = newObjectOption.Value;
```

## ConvertTo