4.0 Update


Huge performance improvements for Recast graphs
====
New algorithms and optimization of the existing ones makes scanning and updating recast graphs significantly faster. The scan is now fully multi-threaded and it handles large worlds with lots of objects much better.
Speedups range from 2x to over 1000x with around 5x being the most common for small to medium sized worlds. The larger the world and the more cores your computer has, the greater the speedup. The 1000x+ speedup was observed when scanning a several square kilometer world (used in a real game). If your game has a procedural world and you use a recast graph to scan it when the game starts, this should improve your game's startup times significantly.

Object pooling is also used in more places which reduces the number of allocations. This is particularly important when recalculating tiles or using navmesh cutting during runtime.
[video]

Improved graph rendering
====
Graph rendering has undergone a massive overhaul to both improve the style of them as well as improve the performance.
In 3.x graph rendering used Unity's Gizmo system which, while nice, requires the rendered lines and surfaces to be recalculated every frame. To improve upon this a custom persistent line and surface drawing system has been developed which allows the meshes to be cached over several frames if the graph is not updated.
For large grid graphs in particular this improves the frame rate in the scene view significantly. For a 1000x1000 grid graph you could in 3.x barely render it in the scene view. Each frame would take over 4 seconds to render. In 4.x this has been reduced to around 90 ms which is a perfectly interactable frame rate.
Take a look at the video below for a comparison.
[video]

As a bonus, the new custom line rendering system will give you nice smooth anti-aliased lines on Windows even when anti-aliasing is disabled in the rendering settings (Unity Gizmos will become aliased).

The graph rendering style has been improved for grid graphs. Now you can render the surface of the nodes as well as the outline of them. In 3.x only the connections between the nodes are visible which is often confusing. Hexagon graphs (which are grid graphs with certain settings) can with the new rendering code be visualized much better.
[image]
[image]

2D for everything
=====
This is a great update for all of you that are working on 2D games or have been thinking of making one.
Many of the internal systems have been reworked or rewritten to be able to handle 2D worlds or even better, worlds with any rotation.
Local avoidance now works for both for 2D games and 3D games, the only thing you need to do is to flip a switch on the RVOSimulator component. You can read more about the changes to the local avoidance system below.
The AIPath script has been rewritten completely to support movement with any graph rotation. It can now also optionally use the Y-axis as the forward axis for the character instead of the Z-axis which is often desired in 2D games.

The funnel modifier now also includes an optional pre-processing step where it flattens the path corridor and transforms it to the XZ plane. This makes it possible to use the funnel modifier for 2D games and even on curved worlds.
[image]

The layered grid graph now also supports arbitrary rotations. It had some partial support in 3.x, but not everything worked.

Recast graphs can now be rotated and navmesh cutting has been reworked to support this.

A new example scene for 2D games with local avoidance has been added and there is also a new documentation page and video tutorial [link] about how to configure pathfinding for 2D games.

Async scanning
====
In 3.x all graphs have to be scanned synchronously, i.e in a single frame. This was problematic if your graphs were large and took some time to scan as the game would freeze during the calculation time.
In 4.0 all graph types have been reworked to support asynchronous scanning which means that you can show a progress bar while the graphs are being scanned and the game will not freeze.

Turn based games
====
New functionality (primarily) for turn based games has been introduced.
In turn based games one often want very detailed control over which units can walk on which nodes and how much it costs for a character to traverse each node. It has been possible to do this via some elaborate combination of graph updates and tags, but possible does not mean easy and it does not mean performant or stable.
In 4.0 a 'traversal provider' (which is an interface that your scripts can implement) can be added to paths which allows you to control exactly what nodes a character should consider blocked and how large the cost of traversing those nodes should be. The package comes with a built in implementation of a traversal provider called 'BlockManager' and an accompanying component called 'SingleNodeBlocker'. The SingleNodeBlocker components can be attached to any objects (for example the units in a turn based game) and has a very simple API with which you can block nodes it occupies. With the BlockManager you can then easily do things like "allow this character to traverse all nodes except those occupied by this list of SingleNodeBlocker components" or "don't allow this character to traverse any nodes occupied by SingleNodeBlocker components except the ones in this list". This is useful if you for example want to search for a path where the character should not be blocked by itself, but other characters should not be able to move through it.

A new example scene has been added which shows how to use these components. The example scene also shows how to visualize all nodes within a certain distance from the character which is useful in turn based games to limit the distance a character can move in a single turn. It also showcases a hexagon graph which is a particularly popular graph type in turn based games.

These utilities can of course be used for games that are not turn based as well, and I expect that they will be, but turn based games are the main target.
[image]

RichAI/AIPath
====
The RichAI script has been improved and the AIPath script has been almost completely rewritten.
Among the most notable improvements are that the AIPath script now slows down and accelerates much more realistically and precisely. By using trajectory optimization [link] the path of the agent is optimized to reach the end point of the path with a zero velocity to reduce any overshoot. This makes it able to stop much more precisely at the end points of paths without spinning around or overshooting.

The AIPath script can now use gravity and there is an option to use either the gravity set in the Unity project settings or a custom gravity which is useful for many games. This option has also been added to the RichAI script. Furthermore the way the AIPath and RichAI components integrate with rigidbodies has been improved and matches the behavior when not using rigidbodies a lot closer than before.

When calculating many or long paths at the same time, especially on slow computers, it can take a small amount of time before the movement script gets back the result of the path calculation. During that time it may have moved a distance away from where it was when it requested the path and therefore the AIPath script has had code for detecting where it should start to follow the calculated path. If the latency for calculaing paths grew too large this algorithm could sometimes not keep up and the agent may for example turn around for a short amount of time and move in the wrong direction on the path. In 4.0 this algorithm has been improved to be both more performant and better handle cases when the latency grows large. This means it should tolerate slower computers (or more units/larger worlds) better.

Code quality
====
The code quality has been significantly improved in version 4.0. I have worked hard to clean up messy areas of code, to add more documentation comments and to refactor existing classes to improve encapsulation. This should make Intellisense suggestions less noisy. For those of you that like to read the source code of packages that you use, this should hopefully make it more enjoyable for you and make it easier to understand the code.

Local avoidance
====
The local avoidance algorithm has been rewritten completely. You will not notice many changes in behavior other than that they are now better at not being pushed through walls in high density crowds, however the configuration has been greatly simplified and some new features have been added. Each agent now has priority setting. Lower priority agents will avoid higher priority agents more. As mentioned in previously the local avoidance system now supports the XY plane as well so you can now use local avoidance in your 2D games.
In 3.x the local avoidance algorithm had various parameters that were not always easy to know the best values for, with the new algorithm it should be much easier to configure.

The new algorithm also allows for much better control over where exactly the character should stop. The previous algorithm was based only on velocities which works great when agents are moving, but it did not know at which point an agent intends to stop and this could sometimes lead to agents jiggling a bit when they reached their target point instead of stopping completely.

