# Infuse v0.0.1
A simple, lightweight dependency injection system designed around Unity.

Note that this project is currently barely in an alpha state, so if it blows up your computer, deletes all your git repositories, or gives your cat explosive diarrhea, that's on you.


## Introduction
Most medium to large scale software projects are composed of multiple small components which are composed together to make larger components in order to produce sophisticated behaviours while managing the overall complexity of the project. In most development environments, there are well established practices and frameworks with which to accomplish this.

However, Unity is not like most development environments.

Generally speaking, it is common to develop components as MonoBehaviours within Unity. This is often desirable due to the fact that MonoBehaviour objects can easily be made to interact with other Unity components and show their state within the scene hierarchy, which makes for much easier project design and debugging.

However, Unity MonoBehaviours have several major caveats;
 - Beyond configuring Script Execution Order as a numeric priority (a limited solution at best), there are no guarantees as to when MonoBehaviours are created, destroyed, enabled, disabled or when Unity-specific callbacks are invoked.
 
 - MonoBehaviours do not have constructors, and as such there is no built-in way to pass MonoBehaviour components from a scene to a newly-instantiated prefab.
 
 - MonoBehaviours are interconnected within scenes using serialized fields. These need to be maintained manually and can quickly degrade into an evil mess as the scene hierarchy scales larger and larger.

There are numerous solutions to these problems;
 - Various dependency injection frameworks already exist, although these are often quite heavyweight and may force a software project to be developed in a particular way.
 
 - Services can be referenced via a ScriptableObject which mediates between services and their dependants, although this doesn't always solve the issue of MonoBehaviour initialisation order.
 
 - Some components simply instantiate themselves as singleton objects, and while this is a relatively simple solution, it comes with all the usual issues associated with the use of singletons.
 
 Infuse takes a slightly different approach which is discussed below.


## Design Goals
Infuse is designed with a number of goals;
 - To be simple as possible - a simple API, clearly-defined behaviour, no intrusive editor extensions, and no dependencies other than Unity.
 
 - To interoperate easily with idiomatic Unity projects - allows Infuse to be retrofitted to existing projects without heavy refactoring and doesn't force a project to be designed in a particular way.
 
 - Interoperate with Unity itself - adding or removing MonoBehaviours in play mode shouldn't guarantee a spew of errors in the console.
 
 - No requirement to use serialization to reference components - this avoids broken references between scenes/prefabs and keeps dependencies as hidden implementation details, rather than properties that are exposed to the inspector. This also means that game designers aren't forced to manage implementation details when building prefabs or making changes to a scene.
 
 - No requirement to use base classes - other existing frameworks already require that certain components extend base classes, so this avoids a potential incompatibility. This also avoids forcing the use of base classes where a dependency on Infuse would otherwise be unnecessary.


## Implementation

Infuse is at it's core a very simple dependency injection system - that is, it stores a set of registered services within a container/context, and passes these services to components that require them.

However unlike a 'traditional' dependency injection system, Infuse has to handle the scenarios where dependencies can randomly come in and out of existence within the Unity scene hierarchy. Given that this isn't the behaviour we'd normally expect to be able to handle with a regular constructor/destructor pair, new terminology is assigned to these events;

- Infuse - This is when an object is started as all of it's required dependencies have come available.
- Defuse - This is when an object is stopped as one or more of it's required dependencies are no longer available.

Given that it's quite common for an object to be destroyed or deactivated and then later instantiated or activated at some point in the future, a dependant object can be restarted as a result of a Defuse event followed by an Infuse event. This gives us the option to modify a scene at will without risking throwing exceptions as a result of dangling and/or invalid references.


## Infuse Context

An Infuse Context is a container which tracks the entire state of an Infuse scope, and only comprises of four methods, which we'll discuss in detail later;

```csharp
namespace Infuse
{
    public interface InfuseContext
    {
        void Register(object instance, bool unregisterOnDestroy = true);
        void Unregister(object instance);

        void RegisterService<TServiceType>(object instance) where TServiceType : class;
        void UnregisterService<TServiceType>(object instance) where TServiceType : class;
    }
}
```

Given that the vast majority of projects only require one context, a global context bound to a ScriptableObject is stored within ```InfuseManager``` and wrapped using static methods. This is purely for developer convenience, and we'll use it in the examples below.


## Basic Usage Example

```csharp
using UnityEngine;
using Infuse;

public interface IExampleService
{
    // Exposed interface to ExampleService would be declared here.
}

// The InfuseAs<> declaration ensures that this object will be made available as IExampleService after OnInfuse().
// You could of course simply declare this object as itself using InfuseAs<ExampleService>.
public class ExampleService : MonoBehaviour, IExampleService, InfuseAs<IExampleService>
{
    private void Awake()
    {
        // Register this object with the default context.
        InfuseManager.Register(this);
    }
    
    // As this object has no dependencies, this method is immediately invoked via InfuseManager.Register(this).
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
    private IExampleService _exampleService;
        
    private void Awake()
    {
        // Register this object with the default context.
        InfuseManager.Register(this);
    }

    // Called after ExampleService.OnInfuse() due to the dependency declared as an argument in the method.
    private void OnInfuse(IExampleService exampleService)
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

You'll immediately notice that both these classes have two member functions - ```OnInfuse()``` and ```OnDefuse()```. These are very similar to ```OnEnable()``` and ```OnDisable()```, except that they're called when the dependencies of a component become available or unavailable.

This means that we can have a complex hierarchy of interdependant components within a scene (or multiple scenes) or within prefabs, and they will automatically be started when their dependencies become available, and stopped when their dependencies become unavailable. This behaviour works on-the-fly even when adding or removing components manually in play mode within the editor.

```InfuseManager.Register(this)``` simply registers an object with the built-in default Infuse Context object. It can be invoked at any time, and an object can be unregistered with the ```InfuseManager.Unregister(...)``` call.

In addition, the Infuse Context automatically unregisters a MonoBehaviour when it's destroyCancellationToken is triggered. This means that a MonoBehaviour object will automatically be unregistered when it is destroyed and calling ```InfuseManager.Unregister(this)``` in ```OnDestroy()``` is unnecessary. However this behaviour can be disabled if necessary.


## Unity 6 Awaitable Support

```csharp
using UnityEngine;
using Infuse;

// The InfuseAs<> declaration ensures that this service will be made available as ExampleService after OnInfuse().
public class ExampleService : MonoBehaviour, InfuseAs<ExampleService>
{
    private void Awake()
    {
        // Register this object with the default context.
        InfuseManager.Register(this);
    }
    
    // As this object has no dependencies, this method is immediately invoked via InfuseManager.Register(this).
    private async Awaitable OnInfuse()
    {
        // Service starting.
        await InitializeAsync();
    }
    
    private async Awaitable InitializeAsync()
    {
        await Awaitable.NextFrameAsync();
        
        // We can perform any other asynchronous operations we like here.
    }
}
```

This is similar to the example above, except the ```OnInfuse()``` function is asynchronous and returns an Awaitable object as provided by Unity 6. This means that a service can have a heavyweight initialisation function which takes multiple frames to complete, and it's dependencies will wait until it has been fully initialised before their ```OnInfuse()``` methods are called.


## Multiple Service Registration

Sometimes it can become necessary for multiple instances of the same class of service to be registered at one time. By default, this obviously isn't going to work because ```OnInfuse()``` recognises services by their class type, so only one instance of any given class can reliably be provided.

However, Infuse provides a component called a Service Container, which allows a service to add itself to a container of multiple services, rather than presenting itself as one and only one instance. In order to enable this functionality, we can use the following declaration;

```csharp
// After OnInfuse() completes, this service will be added to an InfuseServiceCollection<ExampleService>.
public class ExampleService : MonoBehaviour, InfuseAs<InfuseServiceCollection<ExampleService>>
{
    private void Awake()
    {
        InfuseManager.Register(this);
    }
}
```

We can now use this service collection as a dependency;

```csharp
public class ExampleClient : MonoBehaviour
{
    private void Awake()
    {
        InfuseManager.Register(this);
    }

    private void OnInfuse(InfuseServiceCollection<ExampleService> exampleServiceCollection)
    {
        // ...
    }
    
    private void OnDefuse()
    {
        // ...
    }
}
```

An ```InfuseServiceCollection``` becomes available as a dependency (calling ```ExampleClient.OnInfuse()```) as soon as it contains one or more instances. It becomes unavailable (calling ```ExampleClient.OnDefuse()```) when it is empty.

This is only one example of a Service Container, and Infuse also comes with an ```InfuseServiceStack``` which allows one service to override another. You can also write your own classes which should inherit from ```ServiceContainer```.


## Registering External Components

Sometimes it's neither possible or desirable to extend some existing components that are outside of a project in order to register them with Infuse. However we can easily register them manually as in this example;

```csharp
[RequireComponent(typeof(Camera))]
public class RegisterCamera : MonoBehaviour
{
    private void OnEnable()
    {
        InfuseManager.RegisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
    }

    private void OnDisable()
    {
        InfuseManager.UnregisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
    }
}
```

In this case, ```InfuseManager.RegisterService()``` and ```InfuseManager.UnregisterService()``` are distinctly different to ```InfuseManager.Register()``` and ```InfuseManager.Diffuse()``` in that they do not directly register / unregister the Camera object with Infuse, but they do add it to an ```InfuseServiceCollection<Camera>```, which allows another component to query for the collection of cameras as in the previous example.

This example is implemented in full in ExampleScene3, which is bundled with the Infuse package in the Examples directory.


## Debugging

A common problem when using Infuse or any similar system is a missing or invalid dependency stopping a dependant service from starting. To aid with this type of issue, Infuse has a custom editor for InfuseScriptableContext objects, which can be found by highlighting the InfuseGlobalContext asset within Resources in the Infuse package. In this example it's displaying a context populated by running ExampleScene1;

<img width="435" alt="ContextInspector1" src="https://github.com/user-attachments/assets/f5ecace6-b935-44b4-880d-06dea3d8ce97" />

So let's delete ExampleServiceB in the scene hierarchy;

<img width="435" alt="ContextInspector2" src="https://github.com/user-attachments/assets/0f838b80-a148-455e-9833-31a69d8739ab" />

In the screenshot above, you'll see that the line ```Infuse.Examples.ExampleServiceB (0)``` indicates that there are no more active instances of ExampleServiceB, and just below it, the line ```Provides : Infuse.Examples.IExampleServiceB``` is now in red as it is no longer being provided.

You'll also see that the line ```Infuse.Examples.ExampleServiceC (1)``` is in red. This is because as it requires IExampleServiceB (also shown in red just below), it's also no longer available.

Finally, because ExampleServiceC also isn't available, the instances of ExampleClient aren't available either, as they also depend on ExampleServiceC.

We can of course, add ExampleServiceB back into the scene hierarchy while we're still in play mode, which restores all the services back to their original available state.
