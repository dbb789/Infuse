# Infuse
A simple, lightweight dependency injection system designed around Unity.


## Introduction
Most medium to large scale software projects are composed of multiple small components which are composed together to make larger components in order to produce sophisticated behaviours while managing the overall complexity of the project. In most development environments, there are well established practices and frameworks with which to accomplish this.

However, Unity is not like most development environments.

Generally speaking, it is common to develop components as MonoBehaviours within Unity. This is often desirable due to the fact that MonoBehaviour objects can easily be made to interact with other Unity components and show their state within the scene hierarchy, which makes for much easier project design and debugging.

However, Unity MonoBehaviours have several major caveats;
 - Beyond configuring Script Execution Order as a numeric priority (a limited solution at best), there are no guarantees as to when MonoBehaviours are created, destroyed, enabled, disabled or when Unity-specific callbacks are invoked.
 
 - MonoBehaviours do not have constructors, and as such there is no built-in way to pass MonoBehaviour components from a scene to a newly-instantiated prefab.
 
 - MonoBehaviours are interconnected within scenes using serialized fields. These need to be maintained manually and can quickly degrade into an evil mess as the scene hierarchy scales larger and larger.

There are numerous solutions to these problems;
 - Various dependency injection frameworks exist, although these are often quite heavyweight and force a software project to be developed in a particular way.
 
 - Services can be referenced via a ScriptableObject which essentially mediates between services and their dependants, although this doesn't always solve the issue of MonoBehaviour initialisation order.
 
 - Some components simply instantiate themselves as singleton objects, and while this is a relatively simple solution, it comes with all the usual issues associated with the use of singletons.
 
 Infuse takes a slightly different approach which is discussed below.


## Design Goals
Infuse is designed with a number of goals;
 - To be simple as possible - a simple API, clearly-defined behaviour, no intrusive editor extensions, and no dependencies other than Unity.
 
 - To interoperate easily with idiomatic Unity projects - allows Infuse to be retrofitted to existing projects and allows developers to choose how they design their own software.
 
 - Interoperate with Unity itself - adding or removing MonoBehaviours in play mode shouldn't guarantee a spew of errors in the console.
 
 - No requirement to use serialization to reference components - this avoids broken references between scenes/prefabs and keeps dependencies as hidden implementation details, rather than properties that are exposed to the inspector.
 
 - No requirements to use base classes - other existing frameworks already require that certain components extend existing base classes, so of course this would immediately introduce a potential incompatibility. This can potentially also force the use of base classes where a dependency on Infuse would otherwise be unnecessary.


## Basic Usage Example

```csharp
using UnityEngine;
using Infuse;

public class ExampleService : MonoBehaviour, InfuseAs<ExampleService>
{
    private void Awake()
    {
        // Register this object with the default context.
        InfuseManager.Infuse(this);
    }
    
    // As this object has no dependencies, this method is immediately invoked via InfuseManager.Infuse(this).
    private void OnInfuse()
    {
        // Service starting.
    }
    
    // This method will be called automatically when this object is destroyed.
    private void OnDefuse()
    {
        // Service stopping
    }
}


public class ExampleClient : MonoBehaviour
{
    private ExampleService _exampleService;
        
    private void Awake()
    {
        // Register this object with the default context.
        InfuseManager.Infuse(this);
    }

    // Called after ExampleService.OnInfuse() due to the dependency declared as an argument in the method.
    private void OnInfuse(ExampleService exampleService)
    {
        _exampleService = exampleService;
    }

    // Called before ExampleService.OnDefuse(), again due to the dependency declared in OnInfuse() above.
    private void OnDefuse()
    {
        _exampleService = null;
    }
}
```

You'll immediately notice that both these classes have two member functions - OnInfuse() and OnDefuse(). These are very similar to OnEnable() and OnDisable(), except that they're called when the dependencies of a component become available or unavailable.

This means that we can have a complex hierarchy of interdependent components within a scene (or multiple scenes) or within prefabs, and they will automatically be started when their dependencies become available, and stopped when their dependencies become unavailable. This behaviour works on-the-fly even when adding or removing components manually in play mode within the editor.

InfuseManager.Infuse(this) simply registers an object with the built-in default Infuse Context object. It can be invoked at any time, and an object can be unregistered with the InfuseManager.Defuse(...) call.

In addition, the Infuse Context automatically unregisters a MonoBehaviour when it's destroyCancellationToken is triggered. This means that a MonoBehaviour object will automatically be unregistered when it is destroyed and calling InfuseManager.Defuse(this) in OnDestroy() is unnecessary. However this behaviour can be disabled if necessary.
