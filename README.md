### Infuse
A lightweight dependency injection system designed for Unity

## Introduction
Most medium to large scale software projects are composed of multiple small components which are composed together to make larger components in order to produce sophisticated behaviours while managing the overall complexity of the project. In most development environments, there are well established practices and frameworks with which to accomplish this. However, Unity is not like most development environments.

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
 - No requirement to use serialization to reference components - this avoids broken references between scenes/prefabs and keeps dependencies as implementation details, rather than properties that are exposed to the inspector.
