# Ni-Essentials

Ni-Essentials is a foundational module that provides a collection of basic utilities and functions commonly used across various projects. It serves as a generic toolkit to streamline development and maintain consistency across different modules and applications.

> ⚠️ Documentation for all available methods, classes and structures is not complete at the moment. For more detailed information we recommend to study the code as well as the attached xml-comments.

## Features

- Various attributes for inspector, as well as corresponding VisualElement for UIToolkit.
- Highly optimized custom collections.
- Helpers to modify Unity PlayerLoop and keep track of Resolution changes.
- Classes and extensions to help with unsafe.
- Various utilities and extensions.

## Requirements

* Unity 2022.2 or later
* Text Mesh Pro 3.0.6 or later

## Installation

### Manual

1. Clone this repository or download the source files.
2. Copy the `Ni-Essentials` folder into your Unity project's `Assets` directory.

### UPM

1. Open Package Manager from Window > Package Manager.
2. Click the "+" button > Add package from git URL.
3. Enter the following URL:

```
https://github.com/HoSHIZA/Ni-Essentials.git
```

### Manual with `manifest.json`

1. Open `manifest.json`.
2. Add the following line to the file:

```
"com.ni-games.essentials" : "https://github.com/HoSHIZA/Ni-Essentials.git"
```

## Attributes

> ⚠️ All attributes are implemented via CustomPropertyDrawer, so they will work with any CustomEditor that supports CustomPropertyDrawer.

> You can disable all attributes using the preprocessor directive `NIGAMES_INSPECTOR_ATTRIBUTES_DISABLE`.

### ReadOnly

Makes the field in the inspector read-only.

```csharp
[SerializeField, ReadOnly] private int _readOnlyField;
```

### Reference

Displays a field for selecting a managed reference in the inspector. When used, you must mark the desired classes with the `[System.Serializable]` attribute.<br>
<br>
Works only when paired with SerializeReference.

```csharp
public interface IManagedObject {}
public interface ISomeManagedObject {}

[Serializable]
public class A : IManagedObject {}

[Category("Category")] // Specifies the category in the selection menu.
[DisplayName("New Name")] // Renames the class in the selection menu.
[Serializable]
public class B : IManagedObject {}

[Category("")] // Removes from all categories, including the inheritance tree.
[Serializable]
public class C : IManagedObject {}

[Serializable]
public class D : ISomeManagedObject {}

[SerializeReference, Reference] private IManagedObject _managedReferenceField;

// Shows only objects inherited from `ISomeManagedObject` for selection.
[SerializeReference, Reference(typeof(ISomeManagedObject))] private IManagedObject _managedReferenceField;
```

### Scene Picker

Creates a dropdown menu in the inspector for selecting a scene.<br>
<br>
Applies only to a field of type `string` and returns the name of the scene.

```csharp
[SerializeField, ScenePicker] private string _sceneName;
```

### Type Picker

Creates a dropdown menu in the inspector for selecting a `Type`.<br>
<br>
Applies only to a field of type `string` and returns AssemblyQualified type name.

```csharp
[SerializeField, TypePicker] private string _typeName;

// Get type.
public Type Type => Type.GetType(_typeName);
```

### Object Picker

> ⚠️ Not recommended for use as there is a bug breaking the inspector.

Creates a dropdown menu in the inspector for selecting a `UnityEngine.Object`.

```csharp
[SerializeField, ObjectPicker] private Object _object;
```

## Collections

### FastList

A high-performance minimal array-based list implementation.

## Easing

Built-in set of `Func<float, float>` functions for easing, as well as a class of utilities to work with them.

```csharp
using NiPrefs.Essentianls.Easing;

float t = 0.5f;
float easedT = EaseUtility.Evaluate(t, Ease.InCubic); 
float easedT = EaseUtility.Evaluate(t, Ease.InOutBounce);

Func<float, float> easeFunc = EaseFunction.Linear;
```

## Helpers

### PlayerLoopHelper

Class to help modify Unity PlayerLoop. It allows you to add your own runners, and also provides access to PlayerLoop callbacks for each timing via static `Action`.

```csharp
using NiPrefs.Essentianls.Helpers;

public struct CustomUpdate { } // Update
public struct CustomFixedUpdate { } // FixedUpdate

// Method to modify PlayerLoop, use it to modify PlayerLoop conveniently.
// Automatically applies PlayerLoop changes when the method completes.
PlayerLoopHelper.ModifyLoop(systems =>
{
    // Inserts a custom runner into the Loop of the specified timing.
    systems.InsertLoop<Update, CustomUpdate>(static () => { /* Action */ });
    
    // Inserts a custom runner into the Loop of the specified timing.
    systems.InsertLoop<FixedUpdate, CustomFixedUpdate>(static () => { /* Action */ });
    
    // Attempt to remove the runner.
    systems.TryRemoveLoop<FixedUpdate, CustomFixedUpdate>();
});
```

It is also possible to subscribe to events from PlayerLoop.

```csharp
PlayerLoopHelper.OnFixedUpdate += static () => { /* Action */ };
```

or...

```csharp
PlayerLoopHelper.Subscribe(PlayerLoopTiming.EarlyUpdate, static () => { /* Action */ });
```

### ScreenHelper

Helps you keep track of changes in screen resolution or orientation.

```csharp
var isLandscape = ScreenHelper.IsLandscape;
var isPortrait = ScreenHelper.IsPortrait;

ScreenHelper.ResolutionChanged += newResolution => { /* Action */ };
ScreenHelper.OrientationChanged += newOrientation => { /* Action */ };

ScreenHelper.SetPollingTime(2f); // Sets the polling time (in seconds). If 0, each frame is polled.
```

### InitHelper

Helps to safely initialize any values when domain reloading is disabled in the editor.

## Utility

### AssetDatabaseUtility

Class of utilities that slightly extends the capabilities of AssetDatabase.

### ComplexConvert

Class providing additional functionality for complex type conversion in different ways.

## Features

### Blocker

Class for creating a UI blocker that prevents an event from propagating further down the hierarchy.

## Unsafe

### ManagedPtr

Class that helps to safely get a pointer to a managed object from unmanaged code using `GCHandle`.

### NiUnsafe

Wrapper for many basic functions for memory manipulation.

### NiUnsafe Extensions

Extensions to make it easier to work with `IntPtr` when using different types and delegates.

## Extensions

TODO

## TODO

* Other miscellaneous collections.
* Cryptography utilities.
* Improvement of README.md.

## Known Bugs

* Inspector breaks in some scenarios using `ObjectPickerField`. 

## License

This project is licensed under the [MIT License](LICENSE).