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

## ConvertTo