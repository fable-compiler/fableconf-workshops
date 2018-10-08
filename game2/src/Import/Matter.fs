// ts2fable 0.6.1
module rec Fable.Import.Matter

open System
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

type [<AllowNullLiteral>] IExports =
    /// Installs the given plugins on the `Matter` namespace.
    /// This is a short-hand for `Plugin.use`, see it for more information.
    /// Call this function once at the start of your code, with all of the plugins you wish to install as arguments.
    /// Avoid calling this function multiple times unless you intend to manually control installation order.
    abstract ``use``: [<ParamArray>] plugins: ResizeArray<U2<Plugin, string>> -> unit
    abstract Axes: AxesStatic
    abstract Bodies: BodiesStatic
    abstract Body: BodyStatic
    abstract Bounds: BoundsStatic
    abstract Composite: CompositeStatic
    abstract Composites: CompositesStatic
    abstract Constraint: ConstraintStatic
    abstract Engine: EngineStatic
    abstract Grid: GridStatic
    abstract MouseConstraint: MouseConstraintStatic
    abstract Pairs: PairsStatic
    abstract Query: QueryStatic
    abstract Render: RenderStatic
    abstract Runner: RunnerStatic
    abstract Sleeping: SleepingStatic
    abstract Svg: SvgStatic
    abstract Vector: VectorStatic
    abstract Vertices: VerticesStatic
    abstract World: WorldStatic
    abstract Mouse: MouseStatic
    abstract Events: EventsStatic
    abstract Plugin: PluginStatic

/// The `Matter.Axes` module contains methods for creating and manipulating sets of axes.
type [<AllowNullLiteral>] Axes =
    interface end

/// The `Matter.Axes` module contains methods for creating and manipulating sets of axes.
type [<AllowNullLiteral>] AxesStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Axes
    /// <summary>Creates a new set of axes from the given vertices.</summary>
    /// <param name="vertices"></param>
    abstract fromVertices: vertices: array<Vector> -> array<Vector>
    /// <summary>Rotates a set of axes by the given angle.</summary>
    /// <param name="axes"></param>
    /// <param name="angle"></param>
    abstract rotate: axes: array<Vector> * angle: float -> unit

type [<AllowNullLiteral>] IChamfer =
    abstract radius: U2<float, array<float>> option with get, set
    abstract quality: float option with get, set
    abstract qualityMin: float option with get, set
    abstract qualityMax: float option with get, set

type [<AllowNullLiteral>] IChamferableBodyDefinition =
    inherit IBodyDefinition
    abstract chamfer: IChamfer option with get, set

/// The `Matter.Bodies` module contains factory methods for creating rigid body models
/// with commonly used body configurations (such as rectangles, circles and other polygons).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Bodies =
    interface end

/// The `Matter.Bodies` module contains factory methods for creating rigid body models
/// with commonly used body configurations (such as rectangles, circles and other polygons).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] BodiesStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Bodies
    /// <summary>Creates a new rigid body model with a circle hull.
    /// The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properties section of the `Matter.Body` module for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <param name="options"></param>
    /// <param name="maxSides"></param>
    abstract circle: x: float * y: float * radius: float * ?options: IBodyDefinition * ?maxSides: float -> Body
    /// <summary>Creates a new rigid body model with a regular polygon hull with the given number of sides.
    /// The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properties section of the `Matter.Body` module for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sides"></param>
    /// <param name="radius"></param>
    /// <param name="options"></param>
    abstract polygon: x: float * y: float * sides: float * radius: float * ?options: IChamferableBodyDefinition -> Body
    /// <summary>Creates a new rigid body model with a rectangle hull.
    /// The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properties section of the `Matter.Body` module for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="options"></param>
    abstract rectangle: x: float * y: float * width: float * height: float * ?options: IChamferableBodyDefinition -> Body
    /// <summary>Creates a new rigid body model with a trapezoid hull.
    /// The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properties section of the `Matter.Body` module for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="slope"></param>
    /// <param name="options"></param>
    abstract trapezoid: x: float * y: float * width: float * height: float * slope: float * ?options: IChamferableBodyDefinition -> Body
    /// <summary>Creates a body using the supplied vertices (or an array containing multiple sets of vertices).
    /// If the vertices are convex, they will pass through as supplied.
    /// Otherwise if the vertices are concave, they will be decomposed if [poly-decomp.js](https://github.com/schteppe/poly-decomp.js) is available.
    /// Note that this process is not guaranteed to support complex sets of vertices (e.g. those with holes may fail).
    /// By default the decomposition will discard collinear edges (to improve performance).
    /// It can also optionally discard any parts that have an area less than `minimumArea`.
    /// If the vertices can not be decomposed, the result will fall back to using the convex hull.
    /// The options parameter is an object that specifies any `Matter.Body` properties you wish to override the defaults.
    /// See the properties section of the `Matter.Body` module for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="options"></param>
    /// <param name="flagInternal"></param>
    /// <param name="removeCollinear"></param>
    /// <param name="minimumArea"></param>
    abstract fromVertices: x: float * y: float * vertexSets: array<array<Vector>> * ?options: IBodyDefinition * ?flagInternal: bool * ?removeCollinear: float * ?minimumArea: float -> Body

type [<AllowNullLiteral>] IBodyDefinition =
    /// A `Number` specifying the angle of the body, in radians.
    abstract angle: float option with get, set
    /// A `Number` that _measures_ the current angular speed of the body after the last `Body.update`. It is read-only and always positive (it's the magnitude of `body.angularVelocity`).
    abstract angularSpeed: float option with get, set
    /// A `Number` that _measures_ the current angular velocity of the body after the last `Body.update`. It is read-only.
    /// If you need to modify a body's angular velocity directly, you should apply a torque or simply change the body's `angle` (as the engine uses position-Verlet integration).
    abstract angularVelocity: float option with get, set
    /// A `Number` that _measures_ the area of the body's convex hull, calculated at creation by `Body.create`.
    abstract area: float option with get, set
    /// An array of unique axis vectors (edge normals) used for collision detection.
    /// These are automatically calculated from the given convex hull (`vertices` array) in `Body.create`.
    /// They are constantly updated by `Body.update` during the simulation.
    abstract axes: array<Vector> option with get, set
    /// A `Bounds` object that defines the AABB region for the body.
    /// It is automatically calculated from the given convex hull (`vertices` array) in `Body.create` and constantly updated by `Body.update` during simulation.
    abstract bounds: Bounds option with get, set
    /// A `Number` that defines the density of the body, that is its mass per unit area.
    /// If you pass the density via `Body.create` the `mass` property is automatically calculated for you based on the size (area) of the object.
    /// This is generally preferable to simply setting mass and allows for more intuitive definition of materials (e.g. rock has a higher density than wood).
    abstract density: float option with get, set
    /// A `Vector` that specifies the force to apply in the current step. It is zeroed after every `Body.update`. See also `Body.applyForce`.
    abstract force: Vector option with get, set
    /// A `Number` that defines the friction of the body. The value is always positive and is in the range `(0, 1)`.
    /// A value of `0` means that the body may slide indefinitely.
    /// A value of `1` means the body may come to a stop almost instantly after a force is applied.
    ///
    /// The effects of the value may be non-linear.
    /// High values may be unstable depending on the body.
    /// The engine uses a Coulomb friction model including static and kinetic friction.
    /// Note that collision response is based on _pairs_ of bodies, and that `friction` values are _combined_ with the following formula:
    ///
    ///     Math.min(bodyA.friction, bodyB.friction)
    abstract friction: float option with get, set
    /// A `Number` that defines the air friction of the body (air resistance).
    /// A value of `0` means the body will never slow as it moves through space.
    /// The higher the value, the faster a body slows when moving through space.
    /// The effects of the value are non-linear.
    abstract frictionAir: float option with get, set
    /// An integer `Number` uniquely identifying number generated in `Body.create` by `Common.nextId`.
    abstract id: float option with get, set
    /// A `Number` that defines the moment of inertia (i.e. second moment of area) of the body.
    /// It is automatically calculated from the given convex hull (`vertices` array) and density in `Body.create`.
    /// If you modify this value, you must also modify the `body.inverseInertia` property (`1 / inertia`).
    abstract inertia: float option with get, set
    /// A `Number` that defines the inverse moment of inertia of the body (`1 / inertia`).
    /// If you modify this value, you must also modify the `body.inertia` property.
    abstract inverseInertia: float option with get, set
    /// A `Number` that defines the inverse mass of the body (`1 / mass`).
    /// If you modify this value, you must also modify the `body.mass` property.
    abstract inverseMass: float option with get, set
    /// A flag that indicates whether a body is a sensor. Sensor triggers collision events, but doesn't react with colliding body physically.
    abstract isSensor: bool option with get, set
    /// A flag that indicates whether the body is considered sleeping. A sleeping body acts similar to a static body, except it is only temporary and can be awoken.
    /// If you need to set a body as sleeping, you should use `Sleeping.set` as this requires more than just setting this flag.
    abstract isSleeping: bool option with get, set
    /// A flag that indicates whether a body is considered static. A static body can never change position or angle and is completely fixed.
    /// If you need to set a body as static after its creation, you should use `Body.setStatic` as this requires more than just setting this flag.
    abstract isStatic: bool option with get, set
    /// An arbitrary `String` name to help the user identify and manage bodies.
    abstract label: string option with get, set
    /// A `Number` that defines the mass of the body, although it may be more appropriate to specify the `density` property instead.
    /// If you modify this value, you must also modify the `body.inverseMass` property (`1 / mass`).
    abstract mass: float option with get, set
    /// A `Number` that _measures_ the amount of movement a body currently has (a combination of `speed` and `angularSpeed`). It is read-only and always positive.
    /// It is used and updated by the `Matter.Sleeping` module during simulation to decide if a body has come to rest.
    abstract motion: float option with get, set
    /// A `Vector` that specifies the current world-space position of the body.
    abstract position: Vector option with get, set
    /// An `Object` that defines the rendering properties to be consumed by the module `Matter.Render`.
    abstract render: IBodyRenderOptions option with get, set
    /// A `Number` that defines the restitution (elasticity) of the body. The value is always positive and is in the range `(0, 1)`.
    /// A value of `0` means collisions may be perfectly inelastic and no bouncing may occur.
    /// A value of `0.8` means the body may bounce back with approximately 80% of its kinetic energy.
    /// Note that collision response is based on _pairs_ of bodies, and that `restitution` values are _combined_ with the following formula:
    ///
    ///     Math.max(bodyA.restitution, bodyB.restitution)
    abstract restitution: float option with get, set
    /// A `Number` that defines the number of updates in which this body must have near-zero velocity before it is set as sleeping by the `Matter.Sleeping` module (if sleeping is enabled by the engine).
    abstract sleepThreshold: float option with get, set
    /// A `Number` that specifies a tolerance on how far a body is allowed to 'sink' or rotate into other bodies.
    /// Avoid changing this value unless you understand the purpose of `slop` in physics engines.
    /// The default should generally suffice, although very large bodies may require larger values for stable stacking.
    abstract slop: float option with get, set
    /// A `Number` that _measures_ the current speed of the body after the last `Body.update`. It is read-only and always positive (it's the magnitude of `body.velocity`).
    abstract speed: float option with get, set
    /// A `Number` that allows per-body time scaling, e.g. a force-field where bodies inside are in slow-motion, while others are at full speed.
    abstract timeScale: float option with get, set
    /// A `Number` that specifies the torque (turning force) to apply in the current step. It is zeroed after every `Body.update`.
    abstract torque: float option with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string option with get, set
    /// A `Vector` that _measures_ the current velocity of the body after the last `Body.update`. It is read-only.
    /// If you need to modify a body's velocity directly, you should either apply a force or simply change the body's `position` (as the engine uses position-Verlet integration).
    abstract velocity: Vector option with get, set
    /// An array of `Vector` objects that specify the convex hull of the rigid body.
    /// These should be provided about the origin `(0, 0)`. E.g.
    ///
    ///     [{ x: 0, y: 0 }, { x: 25, y: 50 }, { x: 50, y: 0 }]
    ///
    /// When passed via `Body.create`, the vertices are translated relative to `body.position` (i.e. world-space, and constantly updated by `Body.update` during simulation).
    /// The `Vector` objects are also augmented with additional properties required for efficient collision detection.
    ///
    /// Other properties such as `inertia` and `bounds` are automatically calculated from the passed vertices (unless provided via `options`).
    /// Concave hulls are not currently supported. The module `Matter.Vertices` contains useful methods for working with vertices.
    abstract vertices: array<Vector> option with get, set
    /// An array of bodies that make up this body.
    /// The first body in the array must always be a self reference to the current body instance.
    /// All bodies in the `parts` array together form a single rigid compound body.
    /// Parts are allowed to overlap, have gaps or holes or even form concave bodies.
    /// Parts themselves should never be added to a `World`, only the parent body should be.
    /// Use `Body.setParts` when setting parts to ensure correct updates of all properties.
    abstract parts: array<Body> option with get, set
    /// A self reference if the body is _not_ a part of another body.
    /// Otherwise this is a reference to the body that this is a part of.
    /// See `body.parts`.
    abstract parent: Body option with get, set
    /// A `Number` that defines the static friction of the body (in the Coulomb friction model).
    /// A value of `0` means the body will never 'stick' when it is nearly stationary and only dynamic `friction` is used.
    /// The higher the value (e.g. `10`), the more force it will take to initially get the body moving when nearly stationary.
    /// This value is multiplied with the `friction` property to make it easier to change `friction` and maintain an appropriate amount of static friction.
    abstract frictionStatic: float option with get, set
    /// An `Object` that specifies the collision filtering properties of this body.
    ///
    /// Collisions between two bodies will obey the following rules:
    /// - If the two bodies have the same non-zero value of `collisionFilter.group`,
    ///   they will always collide if the value is positive, and they will never collide
    ///   if the value is negative.
    /// - If the two bodies have different values of `collisionFilter.group` or if one
    ///   (or both) of the bodies has a value of 0, then the category/mask rules apply as follows:
    ///
    /// Each body belongs to a collision category, given by `collisionFilter.category`. This
    /// value is used as a bit field and the category should have only one bit set, meaning that
    /// the value of this property is a power of two in the range [1, 2^31]. Thus, there are 32
    /// different collision categories available.
    ///
    /// Each body also defines a collision bitmask, given by `collisionFilter.mask` which specifies
    /// the categories it collides with (the value is the bitwise AND value of all these categories).
    ///
    /// Using the category/mask rules, two bodies `A` and `B` collide if each includes the other's
    /// category in its mask, i.e. `(categoryA & maskB) !== 0` and `(categoryB & maskA) !== 0`
    /// are both true.
    abstract collisionFilter: ICollisionFilter option with get, set

type [<AllowNullLiteral>] IBodyRenderOptions =
    /// A flag that indicates if the body should be rendered.
    abstract visible: bool option with get, set
    /// An `Object` that defines the sprite properties to use when rendering, if any.
    abstract sprite: IBodyRenderOptionsSprite option with get, set
    /// A String that defines the fill style to use when rendering the body (if a sprite is not defined). It is the same as when using a canvas, so it accepts CSS style property values.
    /// Default: a random colour
    abstract fillStyle: string option with get, set
    /// A Number that defines the line width to use when rendering the body outline (if a sprite is not defined). A value of 0 means no outline will be rendered.
    /// Default: 1.5
    abstract lineWidth: float option with get, set
    /// A String that defines the stroke style to use when rendering the body outline (if a sprite is not defined). It is the same as when using a canvas, so it accepts CSS style property values.
    /// Default: a random colour
    abstract strokeStyle: string option with get, set
    abstract opacity: float option with get, set

type [<AllowNullLiteral>] IBodyRenderOptionsSprite =
    /// An `String` that defines the path to the image to use as the sprite texture, if any.
    abstract texture: string with get, set
    /// A `Number` that defines the scaling in the x-axis for the sprite, if any.
    abstract xScale: float with get, set
    /// A `Number` that defines the scaling in the y-axis for the sprite, if any.
    abstract yScale: float with get, set

/// The `Matter.Body` module contains methods for creating and manipulating body models.
/// A `Matter.Body` is a rigid body that can be simulated by a `Matter.Engine`.
/// Factories for commonly used body configurations (such as rectangles, circles and other polygons) can be found in the module `Matter.Bodies`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Body =
    /// A `Number` specifying the angle of the body, in radians.
    abstract angle: float with get, set
    /// A `Number` that _measures_ the current angular speed of the body after the last `Body.update`. It is read-only and always positive (it's the magnitude of `body.angularVelocity`).
    abstract angularSpeed: float with get, set
    /// A `Number` that _measures_ the current angular velocity of the body after the last `Body.update`. It is read-only.
    /// If you need to modify a body's angular velocity directly, you should apply a torque or simply change the body's `angle` (as the engine uses position-Verlet integration).
    abstract angularVelocity: float with get, set
    /// A `Number` that _measures_ the area of the body's convex hull, calculated at creation by `Body.create`.
    abstract area: float with get, set
    /// An array of unique axis vectors (edge normals) used for collision detection.
    /// These are automatically calculated from the given convex hull (`vertices` array) in `Body.create`.
    /// They are constantly updated by `Body.update` during the simulation.
    abstract axes: array<Vector> with get, set
    /// A `Bounds` object that defines the AABB region for the body.
    /// It is automatically calculated from the given convex hull (`vertices` array) in `Body.create` and constantly updated by `Body.update` during simulation.
    abstract bounds: Bounds with get, set
    abstract circleRadius: float with get, set
    /// A `Number` that defines the density of the body, that is its mass per unit area.
    /// If you pass the density via `Body.create` the `mass` property is automatically calculated for you based on the size (area) of the object.
    /// This is generally preferable to simply setting mass and allows for more intuitive definition of materials (e.g. rock has a higher density than wood).
    abstract density: float with get, set
    /// A `Vector` that specifies the force to apply in the current step. It is zeroed after every `Body.update`. See also `Body.applyForce`.
    abstract force: Vector with get, set
    /// A `Number` that defines the friction of the body. The value is always positive and is in the range `(0, 1)`.
    /// A value of `0` means that the body may slide indefinitely.
    /// A value of `1` means the body may come to a stop almost instantly after a force is applied.
    ///
    /// The effects of the value may be non-linear.
    /// High values may be unstable depending on the body.
    /// The engine uses a Coulomb friction model including static and kinetic friction.
    /// Note that collision response is based on _pairs_ of bodies, and that `friction` values are _combined_ with the following formula:
    ///
    ///     Math.min(bodyA.friction, bodyB.friction)
    abstract friction: float with get, set
    /// A `Number` that defines the air friction of the body (air resistance).
    /// A value of `0` means the body will never slow as it moves through space.
    /// The higher the value, the faster a body slows when moving through space.
    /// The effects of the value are non-linear.
    abstract frictionAir: float with get, set
    /// An integer `Number` uniquely identifying number generated in `Body.create` by `Common.nextId`.
    abstract id: float with get, set
    /// A `Number` that defines the moment of inertia (i.e. second moment of area) of the body.
    /// It is automatically calculated from the given convex hull (`vertices` array) and density in `Body.create`.
    /// If you modify this value, you must also modify the `body.inverseInertia` property (`1 / inertia`).
    abstract inertia: float with get, set
    /// A `Number` that defines the inverse moment of inertia of the body (`1 / inertia`).
    /// If you modify this value, you must also modify the `body.inertia` property.
    abstract inverseInertia: float with get, set
    /// A `Number` that defines the inverse mass of the body (`1 / mass`).
    /// If you modify this value, you must also modify the `body.mass` property.
    abstract inverseMass: float with get, set
    /// A flag that indicates whether the body is considered sleeping. A sleeping body acts similar to a static body, except it is only temporary and can be awoken.
    /// If you need to set a body as sleeping, you should use `Sleeping.set` as this requires more than just setting this flag.
    abstract isSleeping: bool with get, set
    /// A flag that indicates whether a body is considered static. A static body can never change position or angle and is completely fixed.
    /// If you need to set a body as static after its creation, you should use `Body.setStatic` as this requires more than just setting this flag.
    abstract isStatic: bool with get, set
    /// An arbitrary `String` name to help the user identify and manage bodies.
    abstract label: string with get, set
    /// A `Number` that defines the mass of the body, although it may be more appropriate to specify the `density` property instead.
    /// If you modify this value, you must also modify the `body.inverseMass` property (`1 / mass`).
    abstract mass: float with get, set
    /// A `Number` that _measures_ the amount of movement a body currently has (a combination of `speed` and `angularSpeed`). It is read-only and always positive.
    /// It is used and updated by the `Matter.Sleeping` module during simulation to decide if a body has come to rest.
    abstract motion: float with get, set
    /// A `Vector` that specifies the current world-space position of the body.
    abstract position: Vector with get, set
    /// An `Object` that defines the rendering properties to be consumed by the module `Matter.Render`.
    abstract render: IBodyRenderOptions with get, set
    /// A `Number` that defines the restitution (elasticity) of the body. The value is always positive and is in the range `(0, 1)`.
    /// A value of `0` means collisions may be perfectly inelastic and no bouncing may occur.
    /// A value of `0.8` means the body may bounce back with approximately 80% of its kinetic energy.
    /// Note that collision response is based on _pairs_ of bodies, and that `restitution` values are _combined_ with the following formula:
    ///
    ///     Math.max(bodyA.restitution, bodyB.restitution)
    abstract restitution: float with get, set
    /// A `Number` that defines the number of updates in which this body must have near-zero velocity before it is set as sleeping by the `Matter.Sleeping` module (if sleeping is enabled by the engine).
    abstract sleepThreshold: float with get, set
    /// A `Number` that specifies a tolerance on how far a body is allowed to 'sink' or rotate into other bodies.
    /// Avoid changing this value unless you understand the purpose of `slop` in physics engines.
    /// The default should generally suffice, although very large bodies may require larger values for stable stacking.
    abstract slop: float with get, set
    /// A `Number` that _measures_ the current speed of the body after the last `Body.update`. It is read-only and always positive (it's the magnitude of `body.velocity`).
    abstract speed: float with get, set
    /// A `Number` that allows per-body time scaling, e.g. a force-field where bodies inside are in slow-motion, while others are at full speed.
    abstract timeScale: float with get, set
    /// A `Number` that specifies the torque (turning force) to apply in the current step. It is zeroed after every `Body.update`.
    abstract torque: float with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string with get, set
    /// A `Vector` that _measures_ the current velocity of the body after the last `Body.update`. It is read-only.
    /// If you need to modify a body's velocity directly, you should either apply a force or simply change the body's `position` (as the engine uses position-Verlet integration).
    abstract velocity: Vector with get, set
    /// An array of `Vector` objects that specify the convex hull of the rigid body.
    /// These should be provided about the origin `(0, 0)`. E.g.
    ///
    ///     [{ x: 0, y: 0 }, { x: 25, y: 50 }, { x: 50, y: 0 }]
    ///
    /// When passed via `Body.create`, the vertices are translated relative to `body.position` (i.e. world-space, and constantly updated by `Body.update` during simulation).
    /// The `Vector` objects are also augmented with additional properties required for efficient collision detection.
    ///
    /// Other properties such as `inertia` and `bounds` are automatically calculated from the passed vertices (unless provided via `options`).
    /// Concave hulls are not currently supported. The module `Matter.Vertices` contains useful methods for working with vertices.
    abstract vertices: array<Vector> with get, set
    /// An array of bodies that make up this body.
    /// The first body in the array must always be a self reference to the current body instance.
    /// All bodies in the `parts` array together form a single rigid compound body.
    /// Parts are allowed to overlap, have gaps or holes or even form concave bodies.
    /// Parts themselves should never be added to a `World`, only the parent body should be.
    /// Use `Body.setParts` when setting parts to ensure correct updates of all properties.
    abstract parts: array<Body> with get, set
    /// A self reference if the body is _not_ a part of another body.
    /// Otherwise this is a reference to the body that this is a part of.
    /// See `body.parts`.
    abstract parent: Body with get, set
    /// A `Number` that defines the static friction of the body (in the Coulomb friction model).
    /// A value of `0` means the body will never 'stick' when it is nearly stationary and only dynamic `friction` is used.
    /// The higher the value (e.g. `10`), the more force it will take to initially get the body moving when nearly stationary.
    /// This value is multiplied with the `friction` property to make it easier to change `friction` and maintain an appropriate amount of static friction.
    abstract frictionStatic: float with get, set
    /// An `Object` that specifies the collision filtering properties of this body.
    ///
    /// Collisions between two bodies will obey the following rules:
    /// - If the two bodies have the same non-zero value of `collisionFilter.group`,
    ///   they will always collide if the value is positive, and they will never collide
    ///   if the value is negative.
    /// - If the two bodies have different values of `collisionFilter.group` or if one
    ///   (or both) of the bodies has a value of 0, then the category/mask rules apply as follows:
    ///
    /// Each body belongs to a collision category, given by `collisionFilter.category`. This
    /// value is used as a bit field and the category should have only one bit set, meaning that
    /// the value of this property is a power of two in the range [1, 2^31]. Thus, there are 32
    /// different collision categories available.
    ///
    /// Each body also defines a collision bitmask, given by `collisionFilter.mask` which specifies
    /// the categories it collides with (the value is the bitwise AND value of all these categories).
    ///
    /// Using the category/mask rules, two bodies `A` and `B` collide if each includes the other's
    /// category in its mask, i.e. `(categoryA & maskB) !== 0` and `(categoryB & maskA) !== 0`
    /// are both true.
    abstract collisionFilter: ICollisionFilter with get, set

/// The `Matter.Body` module contains methods for creating and manipulating body models.
/// A `Matter.Body` is a rigid body that can be simulated by a `Matter.Engine`.
/// Factories for commonly used body configurations (such as rectangles, circles and other polygons) can be found in the module `Matter.Bodies`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] BodyStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Body
    /// <summary>Applies a force to a body from a given world-space position, including resulting torque.</summary>
    /// <param name="body"></param>
    /// <param name="position"></param>
    /// <param name="force"></param>
    abstract applyForce: body: Body * position: Vector * force: Vector -> unit
    /// <summary>Creates a new rigid body model. The options parameter is an object that specifies any properties you wish to override the defaults.
    /// All properties have default values, and many are pre-calculated automatically based on other properties.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="options"></param>
    abstract create: options: IBodyDefinition -> Body
    /// <summary>Rotates a body by a given angle relative to its current angle, without imparting any angular velocity.</summary>
    /// <param name="body"></param>
    /// <param name="rotation"></param>
    abstract rotate: body: Body * rotation: float -> unit
    /// <summary>Returns the next unique group index for which bodies will collide.
    /// If `isNonColliding` is `true`, returns the next unique group index for which bodies will _not_ collide.
    /// See `body.collisionFilter` for more information.</summary>
    /// <param name="isNonColliding"></param>
    abstract nextGroup: isNonColliding: bool -> float
    /// Returns the next unique category bitfield (starting after the initial default category `0x0001`).
    /// There are 32 available. See `body.collisionFilter` for more information.
    abstract nextCategory: unit -> float
    /// <summary>Given a property and a value (or map of), sets the property(s) on the body, using the appropriate setter functions if they exist.
    /// Prefer to use the actual setter functions in performance critical situations.</summary>
    /// <param name="body"></param>
    /// <param name="settings">A property name (or map of properties and values) to set on the body.</param>
    /// <param name="value">The value to set if `settings` is a single property name.</param>
    abstract set: body: Body * settings: obj option * ?value: obj option -> unit
    /// <summary>Sets the mass of the body. Inverse mass and density are automatically updated to reflect the change.</summary>
    /// <param name="body"></param>
    /// <param name="mass"></param>
    abstract setMass: body: Body * mass: float -> unit
    /// <summary>Sets the density of the body. Mass is automatically updated to reflect the change.</summary>
    /// <param name="body"></param>
    /// <param name="density"></param>
    abstract setDensity: body: Body * density: float -> unit
    /// <summary>Sets the moment of inertia (i.e. second moment of area) of the body of the body.
    /// Inverse inertia is automatically updated to reflect the change. Mass is not changed.</summary>
    /// <param name="body"></param>
    abstract setInertia: body: Body * interna: float -> unit
    /// <summary>Sets the body's vertices and updates body properties accordingly, including inertia, area and mass (with respect to `body.density`).
    /// Vertices will be automatically transformed to be orientated around their centre of mass as the origin.
    /// They are then automatically translated to world space based on `body.position`.
    ///
    /// The `vertices` argument should be passed as an array of `Matter.Vector` points (or a `Matter.Vertices` array).
    /// Vertices must form a convex hull, concave hulls are not supported.</summary>
    /// <param name="body"></param>
    /// <param name="vertices"></param>
    abstract setVertices: body: Body * vertices: array<Vector> -> unit
    /// <summary>Sets the parts of the `body` and updates mass, inertia and centroid.
    /// Each part will have its parent set to `body`.
    /// By default the convex hull will be automatically computed and set on `body`, unless `autoHull` is set to `false.`
    /// Note that this method will ensure that the first part in `body.parts` will always be the `body`.</summary>
    /// <param name="body"></param>
    /// <param name="autoHull"></param>
    abstract setParts: body: Body * parts: ResizeArray<Body> * ?autoHull: bool -> unit
    /// <summary>Sets the position of the body instantly. Velocity, angle, force etc. are unchanged.</summary>
    /// <param name="body"></param>
    /// <param name="position"></param>
    abstract setPosition: body: Body * position: Vector -> unit
    /// <summary>Sets the angle of the body instantly. Angular velocity, position, force etc. are unchanged.</summary>
    /// <param name="body"></param>
    /// <param name="angle"></param>
    abstract setAngle: body: Body * angle: float -> unit
    /// <summary>Sets the linear velocity of the body instantly. Position, angle, force etc. are unchanged. See also `Body.applyForce`.</summary>
    /// <param name="body"></param>
    /// <param name="velocity"></param>
    abstract setVelocity: body: Body * velocity: Vector -> unit
    /// <summary>Sets the angular velocity of the body instantly. Position, angle, force etc. are unchanged. See also `Body.applyForce`.</summary>
    /// <param name="body"></param>
    /// <param name="velocity"></param>
    abstract setAngularVelocity: body: Body * velocity: float -> unit
    /// <summary>Sets the body as static, including isStatic flag and setting mass and inertia to Infinity.</summary>
    /// <param name="body"></param>
    /// <param name="isStatic"></param>
    abstract setStatic: body: Body * isStatic: bool -> unit
    /// <summary>Scales the body, including updating physical properties (mass, area, axes, inertia), from a world-space point (default is body centre).</summary>
    /// <param name="body"></param>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="point"></param>
    abstract scale: body: Body * scaleX: float * scaleY: float * ?point: Vector -> unit
    /// <summary>Moves a body by a given vector relative to its current position, without imparting any velocity.</summary>
    /// <param name="body"></param>
    /// <param name="translation"></param>
    abstract translate: body: Body * translation: Vector -> unit
    /// <summary>Performs a simulation step for the given `body`, including updating position and angle using Verlet integration.</summary>
    /// <param name="body"></param>
    /// <param name="deltaTime"></param>
    /// <param name="timeScale"></param>
    /// <param name="correction"></param>
    abstract update: body: Body * deltaTime: float * timeScale: float * correction: float -> unit

type [<AllowNullLiteral>] Bounds =
    abstract min: Vector with get, set
    abstract max: Vector with get, set

/// The `Matter.Bounds` module contains methods for creating and manipulating axis-aligned bounding boxes (AABB).
type [<AllowNullLiteral>] BoundsStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Bounds
    /// <summary>Creates a new axis-aligned bounding box (AABB) for the given vertices.</summary>
    /// <param name="vertices"></param>
    abstract create: vertices: Vertices -> Bounds
    /// <summary>Updates bounds using the given vertices and extends the bounds given a velocity.</summary>
    /// <param name="bounds"></param>
    /// <param name="vertices"></param>
    /// <param name="velocity"></param>
    abstract update: bounds: Bounds * vertices: Vertices * velocity: Vector -> unit
    /// <summary>Returns true if the bounds contains the given point.</summary>
    /// <param name="bounds"></param>
    /// <param name="point"></param>
    abstract contains: bounds: Bounds * point: Vector -> bool
    /// <summary>Returns true if the two bounds intersect.</summary>
    /// <param name="boundsA"></param>
    /// <param name="boundsB"></param>
    abstract overlaps: boundsA: Bounds * boundsB: Bounds -> bool
    /// <summary>Translates the bounds by the given vector.</summary>
    /// <param name="bounds"></param>
    /// <param name="vector"></param>
    abstract translate: bounds: Bounds * vector: Vector -> unit
    /// <summary>Shifts the bounds to the given position.</summary>
    /// <param name="bounds"></param>
    /// <param name="position"></param>
    abstract shift: bounds: Bounds * position: Vector -> unit

type [<AllowNullLiteral>] ICompositeDefinition =
    /// An array of `Body` that are _direct_ children of this composite.
    /// To add or remove bodies you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allBodies` method.
    abstract bodies: array<Body> option with get, set
    /// An array of `Composite` that are _direct_ children of this composite.
    /// To add or remove composites you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allComposites` method.
    abstract composites: array<Composite> option with get, set
    /// An array of `Constraint` that are _direct_ children of this composite.
    /// To add or remove constraints you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allConstraints` method.
    abstract constraints: array<Constraint> option with get, set
    /// An integer `Number` uniquely identifying number generated in `Composite.create` by `Common.nextId`.
    abstract id: float option with get, set
    /// A flag that specifies whether the composite has been modified during the current step.
    /// Most `Matter.Composite` methods will automatically set this flag to `true` to inform the engine of changes to be handled.
    /// If you need to change it manually, you should use the `Composite.setModified` method.
    abstract isModified: bool option with get, set
    /// An arbitrary `String` name to help the user identify and manage composites.
    abstract label: string option with get, set
    /// The `Composite` that is the parent of this composite. It is automatically managed by the `Matter.Composite` methods.
    abstract parent: Composite option with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: String option with get, set

/// The `Matter.Composite` module contains methods for creating and manipulating composite bodies.
/// A composite body is a collection of `Matter.Body`, `Matter.Constraint` and other `Matter.Composite`, therefore composites form a tree structure.
/// It is important to use the functions in this module to modify composites, rather than directly modifying their properties.
/// Note that the `Matter.World` object is also a type of `Matter.Composite` and as such all composite methods here can also operate on a `Matter.World`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Composite =
    /// An array of `Body` that are _direct_ children of this composite.
    /// To add or remove bodies you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allBodies` method.
    abstract bodies: array<Body> with get, set
    /// An array of `Composite` that are _direct_ children of this composite.
    /// To add or remove composites you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allComposites` method.
    abstract composites: array<Composite> with get, set
    /// An array of `Constraint` that are _direct_ children of this composite.
    /// To add or remove constraints you should use `Composite.add` and `Composite.remove` methods rather than directly modifying this property.
    /// If you wish to recursively find all descendants, you should use the `Composite.allConstraints` method.
    abstract constraints: array<Constraint> with get, set
    /// An integer `Number` uniquely identifying number generated in `Composite.create` by `Common.nextId`.
    abstract id: float with get, set
    /// A flag that specifies whether the composite has been modified during the current step.
    /// Most `Matter.Composite` methods will automatically set this flag to `true` to inform the engine of changes to be handled.
    /// If you need to change it manually, you should use the `Composite.setModified` method.
    abstract isModified: bool with get, set
    /// An arbitrary `String` name to help the user identify and manage composites.
    abstract label: string with get, set
    /// The `Composite` that is the parent of this composite. It is automatically managed by the `Matter.Composite` methods.
    abstract parent: Composite with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: String with get, set

/// The `Matter.Composite` module contains methods for creating and manipulating composite bodies.
/// A composite body is a collection of `Matter.Body`, `Matter.Constraint` and other `Matter.Composite`, therefore composites form a tree structure.
/// It is important to use the functions in this module to modify composites, rather than directly modifying their properties.
/// Note that the `Matter.World` object is also a type of `Matter.Composite` and as such all composite methods here can also operate on a `Matter.World`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] CompositeStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Composite
    /// <summary>Generic add function. Adds one or many body(s), constraint(s) or a composite(s) to the given composite.
    /// Triggers `beforeAdd` and `afterAdd` events on the `composite`.</summary>
    /// <param name="composite"></param>
    /// <param name="object"></param>
    abstract add: composite: Composite * ``object``: U3<Body, Composite, Constraint> -> Composite
    /// <summary>Returns all bodies in the given composite, including all bodies in its children, recursively.</summary>
    /// <param name="composite"></param>
    abstract allBodies: composite: Composite -> array<Body>
    /// <summary>Returns all composites in the given composite, including all composites in its children, recursively.</summary>
    /// <param name="composite"></param>
    abstract allComposites: composite: Composite -> array<Composite>
    /// <summary>Returns all constraints in the given composite, including all constraints in its children, recursively.</summary>
    /// <param name="composite"></param>
    abstract allConstraints: composite: Composite -> array<Composite>
    /// <summary>Removes all bodies, constraints and composites from the given composite.
    /// Optionally clearing its children recursively.</summary>
    /// <param name="composite"></param>
    /// <param name="keepStatic"></param>
    /// <param name="deep"></param>
    abstract clear: composite: Composite * keepStatic: bool * ?deep: bool -> unit
    /// <summary>Creates a new composite. The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properites section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="options"></param>
    abstract create: ?options: ICompositeDefinition -> Composite
    /// <summary>Searches the composite recursively for an object matching the type and id supplied, null if not found.</summary>
    /// <param name="composite"></param>
    /// <param name="id"></param>
    /// <param name="type"></param>
    abstract get: composite: Composite * id: float * ``type``: string -> U3<Body, Composite, Constraint>
    /// <summary>Moves the given object(s) from compositeA to compositeB (equal to a remove followed by an add).</summary>
    /// <param name="compositeA"></param>
    /// <param name="objects"></param>
    /// <param name="compositeB"></param>
    abstract move: compositeA: Composite * objects: array<U3<Body, Composite, Constraint>> * compositeB: Composite -> Composite
    /// <summary>Assigns new ids for all objects in the composite, recursively.</summary>
    /// <param name="composite"></param>
    abstract rebase: composite: Composite -> Composite
    /// <summary>Generic remove function. Removes one or many body(s), constraint(s) or a composite(s) to the given composite.
    /// Optionally searching its children recursively.
    /// Triggers `beforeRemove` and `afterRemove` events on the `composite`.</summary>
    /// <param name="composite"></param>
    /// <param name="object"></param>
    /// <param name="deep"></param>
    abstract remove: composite: Composite * ``object``: U3<Body, Composite, Constraint> * ?deep: bool -> Composite
    /// <summary>Sets the composite's `isModified` flag.
    /// If `updateParents` is true, all parents will be set (default: false).
    /// If `updateChildren` is true, all children will be set (default: false).</summary>
    /// <param name="composite"></param>
    /// <param name="isModified"></param>
    /// <param name="updateParents"></param>
    /// <param name="updateChildren"></param>
    abstract setModified: composite: Composite * isModified: bool * ?updateParents: bool * ?updateChildren: bool -> unit
    /// <summary>Translates all children in the composite by a given vector relative to their current positions,
    /// without imparting any velocity.</summary>
    /// <param name="composite"></param>
    /// <param name="translation"></param>
    /// <param name="recursive"></param>
    abstract translate: composite: Composite * translation: Vector * ?recursive: bool -> unit
    /// <summary>Rotates all children in the composite by a given angle about the given point, without imparting any angular velocity.</summary>
    /// <param name="composite"></param>
    /// <param name="rotation"></param>
    /// <param name="point"></param>
    /// <param name="recursive"></param>
    abstract rotate: composite: Composite * rotation: float * point: Vector * ?recursive: bool -> unit
    /// <summary>Scales all children in the composite, including updating physical properties (mass, area, axes, inertia), from a world-space point.</summary>
    /// <param name="composite"></param>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="point"></param>
    /// <param name="recursive"></param>
    abstract scale: composite: Composite * scaleX: float * scaleY: float * point: Vector * ?recursive: bool -> unit

/// The `Matter.Composites` module contains factory methods for creating composite bodies
/// with commonly used configurations (such as stacks and chains).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Composites =
    /// <summary>Creates a composite with a Newton's Cradle setup of bodies and constraints.</summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <param name="size"></param>
    /// <param name="length"></param>
    abstract newtonsCradle: xx: float * yy: float * _number: float * size: float * length: float -> Composite

/// The `Matter.Composites` module contains factory methods for creating composite bodies
/// with commonly used configurations (such as stacks and chains).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] CompositesStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Composites
    /// <summary>Creates a composite with simple car setup of bodies and constraints.</summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="wheelSize"></param>
    abstract car: xx: float * yy: float * width: float * height: float * wheelSize: float -> Composite
    /// <summary>Chains all bodies in the given composite together using constraints.</summary>
    /// <param name="composite"></param>
    /// <param name="xOffsetA"></param>
    /// <param name="yOffsetA"></param>
    /// <param name="xOffsetB"></param>
    /// <param name="yOffsetB"></param>
    /// <param name="options"></param>
    abstract chain: composite: Composite * xOffsetA: float * yOffsetA: float * xOffsetB: float * yOffsetB: float * options: obj option -> Composite
    /// <summary>Connects bodies in the composite with constraints in a grid pattern, with optional cross braces.</summary>
    /// <param name="composite"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="crossBrace"></param>
    /// <param name="options"></param>
    abstract mesh: composite: Composite * columns: float * rows: float * crossBrace: bool * options: obj option -> Composite
    /// <summary>Create a new composite containing bodies created in the callback in a pyramid arrangement.
    /// This function uses the body's bounds to prevent overlaps.</summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="columnGap"></param>
    /// <param name="rowGap"></param>
    /// <param name="callback"></param>
    abstract pyramid: xx: float * yy: float * columns: float * rows: float * columnGap: float * rowGap: float * callback: Function -> Composite
    /// <summary>Creates a simple soft body like object.</summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="columnGap"></param>
    /// <param name="rowGap"></param>
    /// <param name="crossBrace"></param>
    /// <param name="particleRadius"></param>
    /// <param name="particleOptions"></param>
    /// <param name="constraintOptions"></param>
    abstract softBody: xx: float * yy: float * columns: float * rows: float * columnGap: float * rowGap: float * crossBrace: bool * particleRadius: float * particleOptions: obj option * constraintOptions: obj option -> Composite
    /// <summary>Create a new composite containing bodies created in the callback in a grid arrangement.
    /// This function uses the body's bounds to prevent overlaps.</summary>
    /// <param name="xx"></param>
    /// <param name="yy"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="columnGap"></param>
    /// <param name="rowGap"></param>
    /// <param name="callback"></param>
    abstract stack: xx: float * yy: float * columns: float * rows: float * columnGap: float * rowGap: float * callback: Function -> Composite

type [<AllowNullLiteral>] IConstraintDefinition =
    /// The first possible `Body` that this constraint is attached to.
    abstract bodyA: Body option with get, set
    /// The second possible `Body` that this constraint is attached to.
    abstract bodyB: Body option with get, set
    /// An integer `Number` uniquely identifying number generated in `Composite.create` by `Common.nextId`.
    abstract id: float option with get, set
    /// An arbitrary `String` name to help the user identify and manage bodies.
    abstract label: string option with get, set
    /// A `Number` that specifies the target resting length of the constraint.
    /// It is calculated automatically in `Constraint.create` from initial positions of the `constraint.bodyA` and `constraint.bodyB`.
    abstract length: float option with get, set
    /// A `Vector` that specifies the offset of the constraint from center of the `constraint.bodyA` if defined, otherwise a world-space position.
    abstract pointA: Vector option with get, set
    /// A `Vector` that specifies the offset of the constraint from center of the `constraint.bodyA` if defined, otherwise a world-space position.
    abstract pointB: Vector option with get, set
    /// An `Object` that defines the rendering properties to be consumed by the module `Matter.Render`.
    abstract render: IConstraintRenderDefinition option with get, set
    /// A `Number` that specifies the stiffness of the constraint, i.e. the rate at which it returns to its resting `constraint.length`.
    /// A value of `1` means the constraint should be very stiff.
    /// A value of `0.2` means the constraint acts like a soft spring.
    abstract stiffness: float option with get, set
    /// A `Number` that specifies the damping of the constraint,
    /// i.e. the amount of resistance applied to each body based on their velocities to limit the amount of oscillation.
    /// Damping will only be apparent when the constraint also has a very low `stiffness`.
    /// A value of `0.1` means the constraint will apply heavy damping, resulting in little to no oscillation.
    /// A value of `0` means the constraint will apply no damping.
    abstract damping: float option with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string option with get, set

type [<AllowNullLiteral>] IConstraintRenderDefinition =
    /// A `Number` that defines the line width to use when rendering the constraint outline.
    /// A value of `0` means no outline will be rendered.
    abstract lineWidth: float with get, set
    /// A `String` that defines the stroke style to use when rendering the constraint outline.
    /// It is the same as when using a canvas, so it accepts CSS style property values.
    abstract strokeStyle: string with get, set
    /// A flag that indicates if the constraint should be rendered.
    abstract visible: bool with get, set

/// The `Matter.Constraint` module contains methods for creating and manipulating constraints.
/// Constraints are used for specifying that a fixed distance must be maintained between two bodies (or a body and a fixed world-space position).
/// The stiffness of constraints can be modified to create springs or elastic.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Constraint =
    /// The first possible `Body` that this constraint is attached to.
    abstract bodyA: Body with get, set
    /// The second possible `Body` that this constraint is attached to.
    abstract bodyB: Body with get, set
    /// An integer `Number` uniquely identifying number generated in `Composite.create` by `Common.nextId`.
    abstract id: float with get, set
    /// An arbitrary `String` name to help the user identify and manage bodies.
    abstract label: string with get, set
    /// A `Number` that specifies the target resting length of the constraint.
    /// It is calculated automatically in `Constraint.create` from initial positions of the `constraint.bodyA` and `constraint.bodyB`.
    abstract length: float with get, set
    /// A `Vector` that specifies the offset of the constraint from center of the `constraint.bodyA` if defined, otherwise a world-space position.
    abstract pointA: Vector with get, set
    /// A `Vector` that specifies the offset of the constraint from center of the `constraint.bodyA` if defined, otherwise a world-space position.
    abstract pointB: Vector with get, set
    /// An `Object` that defines the rendering properties to be consumed by the module `Matter.Render`.
    abstract render: IConstraintRenderDefinition with get, set
    /// A `Number` that specifies the stiffness of the constraint, i.e. the rate at which it returns to its resting `constraint.length`.
    /// A value of `1` means the constraint should be very stiff.
    /// A value of `0.2` means the constraint acts like a soft spring.
    abstract stiffness: float with get, set
    /// A `Number` that specifies the damping of the constraint,
    /// i.e. the amount of resistance applied to each body based on their velocities to limit the amount of oscillation.
    /// Damping will only be apparent when the constraint also has a very low `stiffness`.
    /// A value of `0.1` means the constraint will apply heavy damping, resulting in little to no oscillation.
    /// A value of `0` means the constraint will apply no damping.
    abstract damping: float with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string with get, set

/// The `Matter.Constraint` module contains methods for creating and manipulating constraints.
/// Constraints are used for specifying that a fixed distance must be maintained between two bodies (or a body and a fixed world-space position).
/// The stiffness of constraints can be modified to create springs or elastic.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] ConstraintStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Constraint
    /// <summary>Creates a new constraint.
    /// All properties have default values, and many are pre-calculated automatically based on other properties.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="options"></param>
    abstract create: options: IConstraintDefinition -> Constraint

type [<AllowNullLiteral>] IEngineDefinition =
    /// An integer `Number` that specifies the number of position iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    abstract positionIterations: float option with get, set
    /// An integer `Number` that specifies the number of velocity iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    abstract velocityIterations: float option with get, set
    /// An integer `Number` that specifies the number of constraint iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    /// The default value of `2` is usually very adequate.
    abstract constraintIterations: float option with get, set
    /// A flag that specifies whether the engine should allow sleeping via the `Matter.Sleeping` module.
    /// Sleeping can improve stability and performance, but often at the expense of accuracy.
    abstract enableSleeping: bool option with get, set
    /// An `Object` containing properties regarding the timing systems of the engine.
    abstract timing: IEngineTimingOptions option with get, set
    /// An instance of a broadphase controller. The default value is a `Matter.Grid` instance created by `Engine.create`.
    abstract grid: Grid option with get, set
    /// A `World` composite object that will contain all simulated bodies and constraints.
    abstract world: World option with get, set

type [<AllowNullLiteral>] IEngineTimingOptions =
    /// A `Number` that specifies the global scaling factor of time for all bodies.
    /// A value of `0` freezes the simulation.
    /// A value of `0.1` gives a slow-motion effect.
    /// A value of `1.2` gives a speed-up effect.
    abstract timeScale: float with get, set
    /// A `Number` that specifies the current simulation-time in milliseconds starting from `0`.
    /// It is incremented on every `Engine.update` by the given `delta` argument.
    abstract timestamp: float with get, set

/// The `Matter.Engine` module contains methods for creating and manipulating engines.
/// An engine is a controller that manages updating the simulation of the world.
/// See `Matter.Runner` for an optional game loop utility.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Engine =
    /// An instance of a broadphase controller. The default value is a `Matter.Grid` instance created by `Engine.create`.
    abstract broadphase: Grid with get, set
    /// An integer `Number` that specifies the number of constraint iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    /// The default value of `2` is usually very adequate.
    abstract constraintIterations: float with get, set
    /// A flag that specifies whether the engine is running or not.
    abstract enabled: bool with get, set
    /// A flag that specifies whether the engine should allow sleeping via the `Matter.Sleeping` module.
    /// Sleeping can improve stability and performance, but often at the expense of accuracy.
    abstract enableSleeping: bool with get, set
    /// Collision pair set for this `Engine`.
    abstract pairs: obj option with get, set
    /// An integer `Number` that specifies the number of position iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    abstract positionIterations: float with get, set
    /// An instance of a `Render` controller. The default value is a `Matter.Render` instance created by `Engine.create`.
    /// One may also develop a custom renderer module based on `Matter.Render` and pass an instance of it to `Engine.create` via `options.render`.
    ///
    /// A minimal custom renderer object must define at least three functions: `create`, `clear` and `world` (see `Matter.Render`).
    /// It is also possible to instead pass the _module_ reference via `options.render.controller` and `Engine.create` will instantiate one for you.
    abstract render: Render with get, set
    /// An `Object` containing properties regarding the timing systems of the engine.
    abstract timing: IEngineTimingOptions with get, set
    /// An integer `Number` that specifies the number of velocity iterations to perform each update.
    /// The higher the value, the higher quality the simulation will be at the expense of performance.
    abstract velocityIterations: float with get, set
    /// A `World` composite object that will contain all simulated bodies and constraints.
    abstract world: World with get, set

/// The `Matter.Engine` module contains methods for creating and manipulating engines.
/// An engine is a controller that manages updating the simulation of the world.
/// See `Matter.Runner` for an optional game loop utility.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] EngineStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Engine
    /// <summary>Clears the engine including the world, pairs and broadphase.</summary>
    /// <param name="engine"></param>
    abstract clear: engine: Engine -> unit
    /// <summary>Creates a new engine. The options parameter is an object that specifies any properties you wish to override the defaults.
    /// All properties have default values, and many are pre-calculated automatically based on other properties.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="element"></param>
    /// <param name="options"></param>
    abstract create: ?element: U2<HTMLElement, IEngineDefinition> * ?options: IEngineDefinition -> Engine
    /// <summary>Merges two engines by keeping the configuration of `engineA` but replacing the world with the one from `engineB`.</summary>
    /// <param name="engineA"></param>
    /// <param name="engineB"></param>
    abstract merge: engineA: Engine * engineB: Engine -> unit
    /// <summary>Moves the simulation forward in time by `delta` ms.
    /// The `correction` argument is an optional `Number` that specifies the time correction factor to apply to the update.
    /// This can help improve the accuracy of the simulation in cases where `delta` is changing between updates.
    /// The value of `correction` is defined as `delta / lastDelta`, i.e. the percentage change of `delta` over the last step.
    /// Therefore the value is always `1` (no correction) when `delta` constant (or when no correction is desired, which is the default).
    /// See the paper on <a href="http://lonesock.net/article/verlet.html">Time Corrected Verlet</a> for more information.
    ///
    /// Triggers `beforeUpdate` and `afterUpdate` events.
    /// Triggers `collisionStart`, `collisionActive` and `collisionEnd` events.</summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    /// <param name="correction"></param>
    abstract update: engine: Engine * ?delta: float * ?correction: float -> Engine
    /// An alias for `Runner.run`, see `Matter.Runner` for more information.
    abstract run: enige: Engine -> unit

type [<AllowNullLiteral>] IGridDefinition =
    interface end

/// The `Matter.Grid` module contains methods for creating and manipulating collision broadphase grid structures.
type [<AllowNullLiteral>] Grid =
    interface end

/// The `Matter.Grid` module contains methods for creating and manipulating collision broadphase grid structures.
type [<AllowNullLiteral>] GridStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Grid
    /// <summary>Creates a new grid.</summary>
    /// <param name="options"></param>
    abstract create: ?options: IGridDefinition -> Grid
    /// <summary>Updates the grid.</summary>
    /// <param name="grid"></param>
    /// <param name="bodies"></param>
    /// <param name="engine"></param>
    /// <param name="forceUpdate"></param>
    abstract update: grid: Grid * bodies: array<Body> * engine: Engine * forceUpdate: bool -> unit
    /// <summary>Clears the grid.</summary>
    /// <param name="grid"></param>
    abstract clear: grid: Grid -> unit

type [<AllowNullLiteral>] IMouseConstraintDefinition =
    /// The `Constraint` object that is used to move the body during interaction.
    abstract ``constraint``: Constraint option with get, set
    /// An `Object` that specifies the collision filter properties.
    /// The collision filter allows the user to define which types of body this mouse constraint can interact with.
    /// See `body.collisionFilter` for more information.
    abstract collisionFilter: ICollisionFilter option with get, set
    /// The `Body` that is currently being moved by the user, or `null` if no body.
    abstract body: Body option with get, set
    /// The `Mouse` instance in use. If not supplied in `MouseConstraint.create`, one will be created.
    abstract mouse: Mouse option with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string option with get, set

/// The `Matter.MouseConstraint` module contains methods for creating mouse constraints.
/// Mouse constraints are used for allowing user interaction, providing the ability to move bodies via the mouse or touch.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] MouseConstraint =
    /// The `Constraint` object that is used to move the body during interaction.
    abstract ``constraint``: Constraint with get, set
    /// An `Object` that specifies the collision filter properties.
    /// The collision filter allows the user to define which types of body this mouse constraint can interact with.
    /// See `body.collisionFilter` for more information.
    abstract collisionFilter: ICollisionFilter with get, set
    /// The `Body` that is currently being moved by the user, or `null` if no body.
    abstract body: Body with get, set
    /// The `Mouse` instance in use. If not supplied in `MouseConstraint.create`, one will be created.
    abstract mouse: Mouse with get, set
    /// A `String` denoting the type of object.
    abstract ``type``: string with get, set

/// The `Matter.MouseConstraint` module contains methods for creating mouse constraints.
/// Mouse constraints are used for allowing user interaction, providing the ability to move bodies via the mouse or touch.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] MouseConstraintStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> MouseConstraint
    /// <summary>Creates a new mouse constraint.
    /// All properties have default values, and many are pre-calculated automatically based on other properties.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="engine"></param>
    /// <param name="options"></param>
    abstract create: engine: Engine * ?options: IMouseConstraintDefinition -> MouseConstraint

/// The `Matter.Pairs` module contains methods for creating and manipulating collision pair sets.
type [<AllowNullLiteral>] Pairs =
    interface end

/// The `Matter.Pairs` module contains methods for creating and manipulating collision pair sets.
type [<AllowNullLiteral>] PairsStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Pairs
    /// <summary>Clears the given pairs structure.</summary>
    /// <param name="pairs"></param>
    abstract clear: pairs: obj option -> obj option

type [<AllowNullLiteral>] IPair =
    abstract id: float with get, set
    abstract bodyA: Body with get, set
    abstract bodyB: Body with get, set
    abstract contacts: obj option with get, set
    abstract activeContacts: obj option with get, set
    abstract separation: float with get, set
    abstract isActive: bool with get, set
    abstract timeCreated: float with get, set
    abstract timeUpdated: float with get, set
    abstract inverseMass: float with get, set
    abstract friction: float with get, set
    abstract frictionStatic: float with get, set
    abstract restitution: float with get, set
    abstract slop: float with get, set

/// The `Matter.Query` module contains methods for performing collision queries.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Query =
    interface end

type [<AllowNullLiteral>] ICollision =
    abstract bodyA: Body with get, set
    abstract bodyB: Body with get, set

/// The `Matter.Query` module contains methods for performing collision queries.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] QueryStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Query
    /// <summary>Casts a ray segment against a set of bodies and returns all collisions, ray width is optional. Intersection points are not provided.</summary>
    /// <param name="bodies"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="rayWidth"></param>
    abstract ray: bodies: array<Body> * startPoint: Vector * endPoint: Vector * ?rayWidth: float -> array<ICollision>
    /// <summary>Returns all bodies whose bounds are inside (or outside if set) the given set of bounds, from the given set of bodies.</summary>
    /// <param name="bodies"></param>
    /// <param name="bounds"></param>
    /// <param name="outside"></param>
    abstract region: bodies: array<Body> * bounds: Bounds * ?outside: bool -> array<Body>
    /// <summary>Returns all bodies whose vertices contain the given point, from the given set of bodies.</summary>
    /// <param name="bodies"></param>
    /// <param name="point"></param>
    abstract point: bodies: array<Body> * point: Vector -> array<Body>

type [<AllowNullLiteral>] IRenderDefinition =
    /// A back-reference to the `Matter.Render` module.
    abstract controller: obj option with get, set
    /// A reference to the `Matter.Engine` instance to be used.
    abstract engine: Engine with get, set
    /// A reference to the element where the canvas is to be inserted (if `render.canvas` has not been specified)
    abstract element: HTMLElement option with get, set
    /// The canvas element to render to. If not specified, one will be created if `render.element` has been specified.
    abstract canvas: HTMLCanvasElement option with get, set
    /// The configuration options of the renderer.
    abstract options: IRendererOptions option with get, set
    /// A `Bounds` object that specifies the drawing view region.
    /// Rendering will be automatically transformed and scaled to fit within the canvas size (`render.options.width` and `render.options.height`).
    /// This allows for creating views that can pan or zoom around the scene.
    /// You must also set `render.options.hasBounds` to `true` to enable bounded rendering.
    abstract bounds: Bounds option with get, set
    /// The 2d rendering context from the `render.canvas` element.
    abstract context: CanvasRenderingContext2D option with get, set
    /// The sprite texture cache.
    abstract textures: obj option with get, set

type [<AllowNullLiteral>] IRendererOptions =
    /// The target width in pixels of the `render.canvas` to be created.
    abstract width: float option with get, set
    /// The target height in pixels of the `render.canvas` to be created.
    abstract height: float option with get, set
    /// A flag that specifies if `render.bounds` should be used when rendering.
    abstract hasBounds: bool option with get, set
    /// Render wireframes only
    abstract wireframes: bool option with get, set

/// The `Matter.Render` module is a simple HTML5 canvas based renderer for visualising instances of `Matter.Engine`.
/// It is intended for development and debugging purposes, but may also be suitable for simple games.
/// It includes a number of drawing options including wireframe, vector with support for sprites and viewports.
type [<AllowNullLiteral>] Render =
    /// A back-reference to the `Matter.Render` module.
    abstract controller: obj option with get, set
    /// A reference to the element where the canvas is to be inserted (if `render.canvas` has not been specified)
    abstract element: HTMLElement with get, set
    /// The canvas element to render to. If not specified, one will be created if `render.element` has been specified.
    abstract canvas: HTMLCanvasElement with get, set
    /// The configuration options of the renderer.
    abstract options: IRendererOptions with get, set
    /// A `Bounds` object that specifies the drawing view region.
    /// Rendering will be automatically transformed and scaled to fit within the canvas size (`render.options.width` and `render.options.height`).
    /// This allows for creating views that can pan or zoom around the scene.
    /// You must also set `render.options.hasBounds` to `true` to enable bounded rendering.
    abstract bounds: Bounds with get, set
    /// The 2d rendering context from the `render.canvas` element.
    abstract context: CanvasRenderingContext2D with get, set
    /// The sprite texture cache.
    abstract textures: obj option with get, set

/// The `Matter.Render` module is a simple HTML5 canvas based renderer for visualising instances of `Matter.Engine`.
/// It is intended for development and debugging purposes, but may also be suitable for simple games.
/// It includes a number of drawing options including wireframe, vector with support for sprites and viewports.
type [<AllowNullLiteral>] RenderStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Render
    /// <summary>Creates a new renderer. The options parameter is an object that specifies any properties you wish to override the defaults.
    /// All properties have default values, and many are pre-calculated automatically based on other properties.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="options"></param>
    abstract create: options: IRenderDefinition -> Render
    /// <summary>Continuously updates the render canvas on the `requestAnimationFrame` event.</summary>
    /// <param name="render"></param>
    abstract run: render: Render -> unit
    /// <summary>Ends execution of `Render.run` on the given `render`, by canceling the animation frame request event loop.</summary>
    /// <param name="render"></param>
    abstract stop: render: Render -> unit
    /// <summary>Sets the pixel ratio of the renderer and updates the canvas.
    /// To automatically detect the correct ratio, pass the string `'auto'` for `pixelRatio`.</summary>
    /// <param name="render"></param>
    /// <param name="pixelRatio"></param>
    abstract setPixelRatio: render: Render * pixelRatio: float -> unit
    /// Renders the given `engine`'s `Matter.World` object.
    /// This is the entry point for all rendering and should be called every time the scene changes.
    abstract world: render: Render -> unit

type [<AllowNullLiteral>] IRunnerOptions =
    /// A `Boolean` that specifies if the runner should use a fixed timestep (otherwise it is variable).
    /// If timing is fixed, then the apparent simulation speed will change depending on the frame rate (but behaviour will be deterministic).
    /// If the timing is variable, then the apparent simulation speed will be constant (approximately, but at the cost of determininism).
    abstract isFixed: bool option with get, set
    /// A `Number` that specifies the time step between updates in milliseconds.
    /// If `engine.timing.isFixed` is set to `true`, then `delta` is fixed.
    /// If it is `false`, then `delta` can dynamically change to maintain the correct apparent simulation speed.
    abstract delta: float option with get, set

/// The `Matter.Runner` module is an optional utility which provides a game loop,
/// that handles updating and rendering a `Matter.Engine` for you within a browser.
/// It is intended for demo and testing purposes, but may be adequate for simple games.
/// If you are using your own game loop instead, then you do not need the `Matter.Runner` module.
/// Instead just call `Engine.update(engine, delta)` in your own loop.
/// Note that the method `Engine.run` is an alias for `Runner.run`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Runner =
    /// A flag that specifies whether the runner is running or not.
    abstract enabled: bool with get, set
    /// A `Boolean` that specifies if the runner should use a fixed timestep (otherwise it is variable).
    /// If timing is fixed, then the apparent simulation speed will change depending on the frame rate (but behaviour will be deterministic).
    /// If the timing is variable, then the apparent simulation speed will be constant (approximately, but at the cost of determininism).
    abstract isFixed: bool with get, set
    /// A `Number` that specifies the time step between updates in milliseconds.
    /// If `engine.timing.isFixed` is set to `true`, then `delta` is fixed.
    /// If it is `false`, then `delta` can dynamically change to maintain the correct apparent simulation speed.
    abstract delta: float with get, set

/// The `Matter.Runner` module is an optional utility which provides a game loop,
/// that handles updating and rendering a `Matter.Engine` for you within a browser.
/// It is intended for demo and testing purposes, but may be adequate for simple games.
/// If you are using your own game loop instead, then you do not need the `Matter.Runner` module.
/// Instead just call `Engine.update(engine, delta)` in your own loop.
/// Note that the method `Engine.run` is an alias for `Runner.run`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] RunnerStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Runner
    /// <summary>Creates a new Runner. The options parameter is an object that specifies any properties you wish to override the defaults.</summary>
    /// <param name="options"></param>
    abstract create: options: IRunnerOptions -> Runner
    /// <summary>Continuously ticks a `Matter.Engine` by calling `Runner.tick` on the `requestAnimationFrame` event.</summary>
    /// <param name="engine"></param>
    abstract run: runner: Runner * engine: Engine -> Runner
    /// <summary>Continuously ticks a `Matter.Engine` by calling `Runner.tick` on the `requestAnimationFrame` event.</summary>
    /// <param name="engine"></param>
    abstract run: engine: Engine -> Runner
    /// <summary>A game loop utility that updates the engine and renderer by one step (a 'tick').
    /// Features delta smoothing, time correction and fixed or dynamic timing.
    /// Triggers `beforeTick`, `tick` and `afterTick` events on the engine.
    /// Consider just `Engine.update(engine, delta)` if you're using your own loop.</summary>
    /// <param name="runner"></param>
    /// <param name="engine"></param>
    /// <param name="time"></param>
    abstract tick: runner: Runner * engine: Engine * time: float -> unit
    /// <summary>Ends execution of `Runner.run` on the given `runner`, by canceling the animation frame request event loop.
    /// If you wish to only temporarily pause the engine, see `engine.enabled` instead.</summary>
    /// <param name="runner"></param>
    abstract stop: runner: Runner -> unit
    /// <summary>Alias for `Runner.run`.</summary>
    /// <param name="runner"></param>
    /// <param name="engine"></param>
    abstract start: runner: Runner * engine: Engine -> unit

/// The `Matter.Sleeping` module contains methods to manage the sleeping state of bodies.
type [<AllowNullLiteral>] Sleeping =
    interface end

/// The `Matter.Sleeping` module contains methods to manage the sleeping state of bodies.
type [<AllowNullLiteral>] SleepingStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Sleeping
    abstract set: body: Body * isSleeping: bool -> unit

/// The `Matter.Svg` module contains methods for converting SVG images into an array of vector points.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Svg =
    interface end

/// The `Matter.Svg` module contains methods for converting SVG images into an array of vector points.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] SvgStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Svg
    /// <summary>Converts an SVG path into an array of vector points.
    /// If the input path forms a concave shape, you must decompose the result into convex parts before use.
    /// See `Bodies.fromVertices` which provides support for this.
    /// Note that this function is not guaranteed to support complex paths (such as those with holes).</summary>
    /// <param name="path"></param>
    /// <param name="sampleLength"></param>
    abstract pathToVertices: path: SVGPathElement * sampleLength: float -> array<Vector>

/// The `Matter.Vector` module contains methods for creating and manipulating vectors.
/// Vectors are the basis of all the geometry related operations in the engine.
/// A `Matter.Vector` object is of the form `{ x: 0, y: 0 }`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Vector =
    abstract x: float with get, set
    abstract y: float with get, set

/// The `Matter.Vector` module contains methods for creating and manipulating vectors.
/// Vectors are the basis of all the geometry related operations in the engine.
/// A `Matter.Vector` object is of the form `{ x: 0, y: 0 }`.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] VectorStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Vector
    /// <summary>Creates a new vector.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    abstract create: ?x: float * ?y: float -> Vector
    /// <summary>Returns a new vector with `x` and `y` copied from the given `vector`.</summary>
    /// <param name="vector"></param>
    abstract clone: vector: Vector -> Vector
    /// <summary>Returns the cross-product of three vectors.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <param name="vectorC"></param>
    abstract cross3: vectorA: Vector * vectorB: Vector * vectorC: Vector -> float
    /// <summary>Adds the two vectors.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <param name="output"></param>
    abstract add: vectorA: Vector * vectorB: Vector * ?output: Vector -> Vector
    /// <summary>Returns the angle in radians between the two vectors relative to the x-axis.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    abstract angle: vectorA: Vector * vectorB: Vector -> float
    /// <summary>Returns the cross-product of two vectors.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    abstract cross: vectorA: Vector * vectorB: Vector -> float
    /// <summary>Divides a vector and a scalar.</summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    abstract div: vector: Vector * scalar: float -> Vector
    /// <summary>Returns the dot-product of two vectors.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    abstract dot: vectorA: Vector * vectorB: Vector -> Number
    /// <summary>Returns the magnitude (length) of a vector.</summary>
    /// <param name="vector"></param>
    abstract magnitude: vector: Vector -> float
    /// <summary>Returns the magnitude (length) of a vector (therefore saving a `sqrt` operation).</summary>
    /// <param name="vector"></param>
    abstract magnitudeSquared: vector: Vector -> float
    /// <summary>Multiplies a vector and a scalar.</summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    abstract mult: vector: Vector * scalar: float -> Vector
    /// <summary>Negates both components of a vector such that it points in the opposite direction.</summary>
    /// <param name="vector"></param>
    abstract neg: vector: Vector -> Vector
    /// <summary>Normalises a vector (such that its magnitude is `1`).</summary>
    /// <param name="vector"></param>
    abstract normalise: vector: Vector -> Vector
    /// <summary>Returns the perpendicular vector. Set `negate` to true for the perpendicular in the opposite direction.</summary>
    /// <param name="vector"></param>
    /// <param name="negate"></param>
    abstract perp: vector: Vector * ?negate: bool -> Vector
    /// <summary>Rotates the vector about (0, 0) by specified angle.</summary>
    /// <param name="vector"></param>
    /// <param name="angle"></param>
    abstract rotate: vector: Vector * angle: float -> Vector
    /// <summary>Rotates the vector about a specified point by specified angle.</summary>
    /// <param name="vector"></param>
    /// <param name="angle"></param>
    /// <param name="point"></param>
    /// <param name="output"></param>
    abstract rotateAbout: vector: Vector * angle: float * point: Vector * ?output: Vector -> Vector
    /// <summary>Subtracts the two vectors.</summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    abstract sub: vectorA: Vector * vectorB: Vector * ?optional: Vector -> Vector

/// The `Matter.Vertices` module contains methods for creating and manipulating sets of vertices.
/// A set of vertices is an array of `Matter.Vector` with additional indexing properties inserted by `Vertices.create`.
/// A `Matter.Body` maintains a set of vertices to represent the shape of the object (its convex hull).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] Vertices =
    interface end

/// The `Matter.Vertices` module contains methods for creating and manipulating sets of vertices.
/// A set of vertices is an array of `Matter.Vector` with additional indexing properties inserted by `Vertices.create`.
/// A `Matter.Body` maintains a set of vertices to represent the shape of the object (its convex hull).
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] VerticesStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Vertices
    /// <summary>Returns the average (mean) of the set of vertices.</summary>
    /// <param name="vertices"></param>
    abstract mean: vertices: array<Vector> -> array<Vector>
    /// <summary>Sorts the input vertices into clockwise order in place.</summary>
    /// <param name="vertices"></param>
    abstract clockwiseSort: vertices: array<Vector> -> array<Vector>
    /// <summary>Returns true if the vertices form a convex shape (vertices must be in clockwise order).</summary>
    /// <param name="vertices"></param>
    abstract isConvex: vertices: array<Vector> -> bool
    /// <summary>Returns the convex hull of the input vertices as a new array of points.</summary>
    /// <param name="vertices"></param>
    abstract hull: vertices: array<Vector> -> array<Vector>
    /// <summary>Returns the area of the set of vertices.</summary>
    /// <param name="vertices"></param>
    /// <param name="signed"></param>
    abstract area: vertices: array<Vector> * signed: bool -> float
    /// <summary>Returns the centre (centroid) of the set of vertices.</summary>
    /// <param name="vertices"></param>
    abstract centre: vertices: array<Vector> -> Vector
    /// <summary>Chamfers a set of vertices by giving them rounded corners, returns a new set of vertices.
    /// The radius parameter is a single number or an array to specify the radius for each vertex.</summary>
    /// <param name="vertices"></param>
    /// <param name="radius"></param>
    /// <param name="quality"></param>
    /// <param name="qualityMin"></param>
    /// <param name="qualityMax"></param>
    abstract chamfer: vertices: array<Vector> * radius: U2<float, array<float>> * quality: float * qualityMin: float * qualityMax: float -> unit
    /// <summary>Returns `true` if the `point` is inside the set of `vertices`.</summary>
    /// <param name="vertices"></param>
    /// <param name="point"></param>
    abstract contains: vertices: array<Vector> * point: Vector -> bool
    /// <summary>Creates a new set of `Matter.Body` compatible vertices.
    /// The `points` argument accepts an array of `Matter.Vector` points orientated around the origin `(0, 0)`, for example:
    ///
    ///     [{ x: 0, y: 0 }, { x: 25, y: 50 }, { x: 50, y: 0 }]
    ///
    /// The `Vertices.create` method returns a new array of vertices, which are similar to Matter.Vector objects,
    /// but with some additional references required for efficient collision detection routines.
    ///
    /// Note that the `body` argument is not optional, a `Matter.Body` reference must be provided.</summary>
    /// <param name="points"></param>
    /// <param name="body"></param>
    abstract create: points: array<Vector> * body: Body -> unit
    /// <summary>Parses a string containing ordered x y pairs separated by spaces (and optionally commas),
    /// into a `Matter.Vertices` object for the given `Matter.Body`.
    /// For parsing SVG paths, see `Svg.pathToVertices`.</summary>
    /// <param name="path"></param>
    /// <param name="body"></param>
    abstract fromPath: path: string * body: Body -> array<Vector>
    /// <summary>Returns the moment of inertia (second moment of area) of the set of vertices given the total mass.</summary>
    /// <param name="vertices"></param>
    /// <param name="mass"></param>
    abstract inertia: vertices: array<Vector> * mass: float -> float
    /// <summary>Rotates the set of vertices in-place.</summary>
    /// <param name="vertices"></param>
    /// <param name="angle"></param>
    /// <param name="point"></param>
    abstract rotate: vertices: array<Vector> * angle: float * point: Vector -> unit
    /// <summary>Scales the vertices from a point (default is centre) in-place.</summary>
    /// <param name="vertices"></param>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="point"></param>
    abstract scale: vertices: array<Vector> * scaleX: float * scaleY: float * point: Vector -> unit
    /// <summary>Translates the set of vertices in-place.</summary>
    /// <param name="vertices"></param>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    abstract translate: vertices: array<Vector> * vector: Vector * scalar: float -> unit

type [<AllowNullLiteral>] IWorldDefinition =
    inherit ICompositeDefinition
    abstract gravity: Gravity option with get, set
    abstract bounds: Bounds option with get, set

type [<AllowNullLiteral>] Gravity =
    inherit Vector
    abstract scale: float with get, set

/// The `Matter.World` module contains methods for creating and manipulating the world composite.
/// A `Matter.World` is a `Matter.Composite` body, which is a collection of `Matter.Body`, `Matter.Constraint` and other `Matter.Composite`.
/// A `Matter.World` has a few additional properties including `gravity` and `bounds`.
/// It is important to use the functions in the `Matter.Composite` module to modify the world composite, rather than directly modifying its properties.
/// There are also a few methods here that alias those in `Matter.Composite` for easier readability.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] World =
    inherit Composite
    abstract gravity: Gravity with get, set
    abstract bounds: Bounds with get, set

/// The `Matter.World` module contains methods for creating and manipulating the world composite.
/// A `Matter.World` is a `Matter.Composite` body, which is a collection of `Matter.Body`, `Matter.Constraint` and other `Matter.Composite`.
/// A `Matter.World` has a few additional properties including `gravity` and `bounds`.
/// It is important to use the functions in the `Matter.Composite` module to modify the world composite, rather than directly modifying its properties.
/// There are also a few methods here that alias those in `Matter.Composite` for easier readability.
///
/// See the included usage [examples](https://github.com/liabru/matter-js/tree/master/examples).
type [<AllowNullLiteral>] WorldStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> World
    /// <summary>Add objects or arrays of objects of types: Body, Constraint, Composite</summary>
    /// <param name="world"></param>
    /// <param name="body"></param>
    abstract add: world: World * body: U7<Body, array<Body>, Composite, array<Composite>, Constraint, array<Constraint>, MouseConstraint> -> World
    /// <summary>An alias for Composite.addBody since World is also a Composite</summary>
    /// <param name="world"></param>
    /// <param name="body"></param>
    abstract addBody: world: World * body: Body -> World
    /// <summary>An alias for Composite.add since World is also a Composite</summary>
    /// <param name="world"></param>
    /// <param name="composite"></param>
    abstract addComposite: world: World * composite: Composite -> World
    /// <summary>An alias for Composite.addConstraint since World is also a Composite</summary>
    /// <param name="world"></param>
    /// <param name="constraint"></param>
    abstract addConstraint: world: World * ``constraint``: Constraint -> World
    /// <summary>An alias for Composite.clear since World is also a Composite</summary>
    /// <param name="world"></param>
    /// <param name="keepStatic"></param>
    abstract clear: world: World * keepStatic: bool -> unit
    /// <summary>Creates a new world composite. The options parameter is an object that specifies any properties you wish to override the defaults.
    /// See the properties section below for detailed information on what you can pass via the `options` object.</summary>
    /// <param name="options"></param>
    abstract create: options: IWorldDefinition -> World

type [<AllowNullLiteral>] ICollisionFilter =
    abstract category: float with get, set
    abstract mask: float with get, set
    abstract group: float with get, set

type [<AllowNullLiteral>] IMousePoint =
    abstract x: float with get, set
    abstract y: float with get, set

type [<AllowNullLiteral>] Mouse =
    abstract element: HTMLElement with get, set
    abstract absolute: IMousePoint with get, set
    abstract position: IMousePoint with get, set
    abstract mousedownPosition: IMousePoint with get, set
    abstract mouseupPosition: IMousePoint with get, set
    abstract offset: IMousePoint with get, set
    abstract scale: IMousePoint with get, set
    abstract wheelDelta: float with get, set
    abstract button: float with get, set
    abstract pixelRatio: float with get, set

type [<AllowNullLiteral>] MouseStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Mouse
    abstract create: element: HTMLElement -> Mouse
    abstract setElement: mouse: Mouse * element: HTMLElement -> unit
    abstract clearSourceEvents: mouse: Mouse -> unit
    abstract setOffset: mouse: Mouse * offset: Vector -> unit
    abstract setScale: mouse: Mouse * scale: Vector -> unit

type [<AllowNullLiteral>] IEvent<'T> =
    /// The name of the event
    abstract name: string with get, set
    /// The source object of the event
    abstract source: 'T with get, set

type [<AllowNullLiteral>] IEventComposite<'T> =
    inherit IEvent<'T>
    /// EventObjects (may be a single body, constraint, composite or a mixed array of these)
    abstract ``object``: obj option with get, set

type [<AllowNullLiteral>] IEventTimestamped<'T> =
    inherit IEvent<'T>
    /// The engine.timing.timestamp of the event
    abstract timestamp: float with get, set

type [<AllowNullLiteral>] IEventCollision<'T> =
    inherit IEventTimestamped<'T>
    /// The collision pair
    abstract pairs: array<IPair> with get, set

type [<AllowNullLiteral>] Events =
    interface end

type [<AllowNullLiteral>] EventsStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Events
    /// Fired when a body starts sleeping (where `this` is the body).
    [<Emit "$0.on($1,'sleepStart',$2)">] abstract on_sleepStart: obj: Body * callback: (IEvent<Body> -> unit) -> unit
    /// Fired when a body ends sleeping (where `this` is the body).
    [<Emit "$0.on($1,'sleepEnd',$2)">] abstract on_sleepEnd: obj: Body * callback: (IEvent<Body> -> unit) -> unit
    /// Fired when a call to `Composite.add` is made, before objects have been added.
    [<Emit "$0.on($1,'beforeAdd',$2)">] abstract on_beforeAdd: obj: Engine * callback: (IEventComposite<Composite> -> unit) -> unit
    /// Fired when a call to `Composite.add` is made, after objects have been added.
    [<Emit "$0.on($1,'afterAdd',$2)">] abstract on_afterAdd: obj: Engine * callback: (IEventComposite<Composite> -> unit) -> unit
    /// Fired when a call to `Composite.remove` is made, before objects have been removed.
    [<Emit "$0.on($1,'beforeRemove',$2)">] abstract on_beforeRemove: obj: Engine * callback: (IEventComposite<Composite> -> unit) -> unit
    /// Fired when a call to `Composite.remove` is made, after objects have been removed.
    [<Emit "$0.on($1,'afterRemove',$2)">] abstract on_afterRemove: obj: Engine * callback: (IEventComposite<Composite> -> unit) -> unit
    /// Fired after engine update and all collision events
    [<Emit "$0.on($1,'afterUpdate',$2)">] abstract on_afterUpdate: obj: Engine * callback: (IEventTimestamped<Engine> -> unit) -> unit
    /// Fired before rendering
    [<Emit "$0.on($1,'beforeRender',$2)">] abstract on_beforeRender: obj: Engine * callback: (IEventTimestamped<Render> -> unit) -> unit
    /// Fired after rendering
    [<Emit "$0.on($1,'afterRender',$2)">] abstract on_afterRender: obj: Engine * callback: (IEventTimestamped<Render> -> unit) -> unit
    /// Fired just before an update
    [<Emit "$0.on($1,'beforeUpdate',$2)">] abstract on_beforeUpdate: obj: Engine * callback: (IEventTimestamped<Engine> -> unit) -> unit
    /// Fired after engine update, provides a list of all pairs that are colliding in the current tick (if any)
    [<Emit "$0.on($1,'collisionActive',$2)">] abstract on_collisionActive: obj: Engine * callback: (IEventCollision<Engine> -> unit) -> unit
    /// Fired after engine update, provides a list of all pairs that have ended collision in the current tick (if any)
    [<Emit "$0.on($1,'collisionEnd',$2)">] abstract on_collisionEnd: obj: Engine * callback: (IEventCollision<Engine> -> unit) -> unit
    /// Fired after engine update, provides a list of all pairs that have started to collide in the current tick (if any)
    [<Emit "$0.on($1,'collisionStart',$2)">] abstract on_collisionStart: obj: Engine * callback: (IEventCollision<Engine> -> unit) -> unit
    /// Fired at the start of a tick, before any updates to the engine or timing
    [<Emit "$0.on($1,'beforeTick',$2)">] abstract on_beforeTick: obj: Engine * callback: (IEventTimestamped<Runner> -> unit) -> unit
    /// Fired after engine timing updated, but just before update
    [<Emit "$0.on($1,'tick',$2)">] abstract on_tick: obj: Engine * callback: (IEventTimestamped<Runner> -> unit) -> unit
    /// Fired at the end of a tick, after engine update and after rendering
    [<Emit "$0.on($1,'afterTick',$2)">] abstract on_afterTick: obj: Engine * callback: (IEventTimestamped<Runner> -> unit) -> unit
    /// Fired before rendering
    [<Emit "$0.on($1,'beforeRender',$2)">] abstract on_beforeRender: obj: Engine * callback: (IEventTimestamped<Runner> -> unit) -> unit
    /// Fired after rendering
    [<Emit "$0.on($1,'afterRender',$2)">] abstract on_afterRender: obj: Engine * callback: (IEventTimestamped<Runner> -> unit) -> unit
    /// <summary>Fired when the mouse is down (or a touch has started) during the last step</summary>
    /// <param name="obj"></param>
    /// <param name="callback"></param>
    [<Emit "$0.on($1,'mousedown',$2)">] abstract on_mousedown: obj: MouseConstraint * callback: (obj option -> unit) -> unit
    /// <summary>Fired when the mouse has moved (or a touch moves) during the last step</summary>
    /// <param name="obj"></param>
    /// <param name="callback"></param>
    [<Emit "$0.on($1,'mousemove',$2)">] abstract on_mousemove: obj: MouseConstraint * callback: (obj option -> unit) -> unit
    /// <summary>Fired when the mouse is up (or a touch has ended) during the last step</summary>
    /// <param name="obj"></param>
    /// <param name="callback"></param>
    [<Emit "$0.on($1,'mouseup',$2)">] abstract on_mouseup: obj: MouseConstraint * callback: (obj option -> unit) -> unit
    abstract on: obj: obj option * name: string * callback: (obj option -> unit) -> unit
    /// <summary>Removes the given event callback. If no callback, clears all callbacks in eventNames. If no eventNames, clears all events.</summary>
    /// <param name="obj"></param>
    /// <param name="eventName"></param>
    /// <param name="callback"></param>
    abstract off: obj: obj option * eventName: string * callback: (obj option -> unit) -> unit
    /// <summary>Fires all the callbacks subscribed to the given object's eventName, in the order they subscribed, if any.</summary>
    /// <param name="object"></param>
    /// <param name="eventNames"></param>
    /// <param name="event"></param>
    abstract trigger: ``object``: obj option * eventNames: string * ?``event``: (obj option -> unit) -> unit

type Dependency = obj

type [<AllowNullLiteral>] Plugin =
    abstract name: string with get, set
    abstract version: string with get, set
    abstract install: (unit -> unit) with get, set
    abstract ``for``: string option with get, set

type [<AllowNullLiteral>] PluginStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> Plugin
    /// <summary>Registers a plugin object so it can be resolved later by name.</summary>
    /// <param name="plugin">The plugin to register.</param>
    abstract register: plugin: Plugin -> Plugin
    /// <summary>Resolves a dependency to a plugin object from the registry if it exists.
    /// The `dependency` may contain a version, but only the name matters when resolving.</summary>
    /// <param name="dependency">The dependency.</param>
    abstract resolve: dependency: string -> Plugin option
    /// <summary>Returns `true` if the object meets the minimum standard to be considered a plugin.
    /// This means it must define the following properties:
    /// - `name`
    /// - `version`
    /// - `install`</summary>
    /// <param name="obj">The obj to test.</param>
    abstract isPlugin: obj: PluginStaticIsPluginObj -> bool
    /// <summary>Returns a pretty printed plugin name and version.</summary>
    /// <param name="plugin">The plugin.</param>
    abstract toString: plugin: U2<string, Plugin> -> string
    /// <summary>Returns `true` if `plugin.for` is applicable to `module` by comparing against `module.name` and `module.version`.
    /// If `plugin.for` is not specified then it is assumed to be applicable.
    /// The value of `plugin.for` is a string of the format `'module-name'` or `'module-name@version'`.</summary>
    /// <param name="plugin">The plugin.</param>
    /// <param name="module">The module.</param>
    abstract isFor: plugin: Plugin * ``module``: PluginStaticIsForModule -> bool
    /// <summary>Installs the plugins by calling `plugin.install` on each plugin specified in `plugins` if passed, otherwise `module.uses`.
    /// For installing plugins on `Matter` see the convenience function `Matter.use`.
    /// Plugins may be specified either by their name or a reference to the plugin object.
    /// Plugins themselves may specify further dependencies, but each plugin is installed only once.
    /// Order is important, a topological sort is performed to find the best resulting order of installation.
    /// This sorting attempts to satisfy every dependency's requested ordering, but may not be exact in all cases.
    /// This function logs the resulting status of each dependency in the console, along with any warnings.
    /// - A green tick ✅ indicates a dependency was resolved and installed.
    /// - An orange diamond 🔶 indicates a dependency was resolved but a warning was thrown for it or one if its dependencies.
    /// - A red cross ❌ indicates a dependency could not be resolved.
    /// Avoid calling this function multiple times on the same module unless you intend to manually control installation order.</summary>
    /// <param name="module">The module install plugins on.</param>
    /// <param name="plugins">The plugins to install on module (optional, defaults to `module.uses`).</param>
    abstract ``use``: ``module``: PluginStaticUseModule * plugins: ResizeArray<U2<Plugin, string>> -> unit
    /// <summary>Recursively finds all of a module's dependencies and returns a flat dependency graph.</summary>
    /// <param name="module">The module.</param>
    abstract dependencies: ``module``: Dependency * ?tracked: PluginStaticDependenciesTracked -> U2<obj, string> option
    /// <summary>Parses a dependency string into its components.
    /// The `dependency` is a string of the format `'module-name'` or `'module-name@version'`.
    /// See documentation for `Plugin.versionParse` for a description of the format.
    /// This function can also handle dependencies that are already resolved (e.g. a module object).</summary>
    /// <param name="dependency">The dependency of the format `'module-name'` or `'module-name</param>
    abstract dependencyParse: dependency: Dependency -> obj
    /// <summary>Parses a version string into its components.
    /// Versions are strictly of the format `x.y.z` (as in [semver](http://semver.org/)).
    /// Versions may optionally have a prerelease tag in the format `x.y.z-alpha`.
    /// Ranges are a strict subset of [npm ranges](https://docs.npmjs.com/misc/semver#advanced-range-syntax).
    /// Only the following range types are supported:
    /// - Tilde ranges e.g. `~1.2.3`
    /// - Caret ranges e.g. `^1.2.3`
    /// - Exact version e.g. `1.2.3`
    /// - Any version `*`</summary>
    /// <param name="range">The version string.</param>
    abstract versionParse: range: string -> obj
    /// <summary>Returns `true` if `version` satisfies the given `range`.
    /// See documentation for `Plugin.versionParse` for a description of the format.
    /// If a version or range is not specified, then any version (`*`) is assumed to satisfy.</summary>
    /// <param name="version">The version string.</param>
    /// <param name="range">The range string.</param>
    abstract versionSatisfies: version: string * range: string -> bool

type [<AllowNullLiteral>] PluginStaticIsPluginObj =
    interface end

type [<AllowNullLiteral>] PluginStaticIsForModule =
    abstract name: string option with get, set
    [<Emit "$0[$1]{{=$2}}">] abstract Item: ``_``: string -> obj option with get, set

type [<AllowNullLiteral>] PluginStaticUseModule =
    abstract uses: ResizeArray<U2<Plugin, string>> option with get, set
    [<Emit "$0[$1]{{=$2}}">] abstract Item: ``_``: string -> obj option with get, set

type [<AllowNullLiteral>] PluginStaticDependenciesTracked =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: ``_``: string -> ResizeArray<string> with get, set