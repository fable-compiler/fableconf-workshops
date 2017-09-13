namespace Fable.Import.P2

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

type [<AllowNullLiteral>] [<Import("AABB","p2")>] AABB(?options: obj) =
    member __.upperBound with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.lowerBound with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.setFromPoints(points: array<(float*float)>, position: (float*float), angle: float, skinSize: float): unit = jsNative
    member __.copy(aabb: AABB): unit = jsNative
    member __.extend(aabb: AABB): unit = jsNative
    member __.overlaps(aabb: AABB): bool = jsNative
    member __.containsPoint(point: (float*float)): bool = jsNative
    member __.overlapsRay(ray: Ray): float = jsNative

and [<AllowNullLiteral>] [<Import("Broadphase","p2")>] Broadphase(``type``: float) =
    member __.AABB with get(): float = jsNative and set(v: float): unit = jsNative
    member __.BOUNDING_CIRCLE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.NAIVE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.SAP with get(): float = jsNative and set(v: float): unit = jsNative
    member __.``type`` with get(): float = jsNative and set(v: float): unit = jsNative
    member __.result with get(): array<Body> = jsNative and set(v: array<Body>): unit = jsNative
    member __.world with get(): World = jsNative and set(v: World): unit = jsNative
    member __.boundingVolumeType with get(): float = jsNative and set(v: float): unit = jsNative
    static member boundingRadiusCheck(bodyA: Body, bodyB: Body): bool = jsNative
    static member aabbCheck(bodyA: Body, bodyB: Body): bool = jsNative
    static member canCollide(bodyA: Body, bodyB: Body): bool = jsNative
    member __.setWorld(world: World): unit = jsNative
    member __.getCollisionPairs(world: World): array<Body> = jsNative
    member __.boundingVolumeCheck(bodyA: Body, bodyB: Body): bool = jsNative

and [<AllowNullLiteral>] [<Import("NaiveBroadphase","p2")>] NaiveBroadphase() =
    inherit Broadphase(0.)

and [<AllowNullLiteral>] [<Import("Narrowphase","p2")>] Narrowphase() =
    member __.contactEquations with get(): array<ContactEquation> = jsNative and set(v: array<ContactEquation>): unit = jsNative
    member __.frictionEquations with get(): array<FrictionEquation> = jsNative and set(v: array<FrictionEquation>): unit = jsNative
    member __.enableFriction with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.enableEquations with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.slipForce with get(): float = jsNative and set(v: float): unit = jsNative
    member __.frictionCoefficient with get(): float = jsNative and set(v: float): unit = jsNative
    member __.surfaceVelocity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.reuseObjects with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.resuableContactEquations with get(): array<obj> = jsNative and set(v: array<obj>): unit = jsNative
    member __.reusableFrictionEquations with get(): array<obj> = jsNative and set(v: array<obj>): unit = jsNative
    member __.restitution with get(): float = jsNative and set(v: float): unit = jsNative
    member __.stiffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.relaxation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.frictionStiffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.frictionRelaxation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.enableFrictionReduction with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.contactSkinSize with get(): float = jsNative and set(v: float): unit = jsNative
    member __.collidedLastStep(bodyA: Body, bodyB: Body): bool = jsNative
    member __.reset(): unit = jsNative
    member __.createContactEquation(bodyA: Body, bodyB: Body, shapeA: Shape, shapeB: Shape): ContactEquation = jsNative
    member __.createFrictionFromContact(c: ContactEquation): FrictionEquation = jsNative
    member __.bodiesOverlap(bodyA: Body, bodyB: Body, ?checkCollisionMasks: bool): bool = jsNative

and [<AllowNullLiteral>] [<Import("SAPBroadphase","p2")>] SAPBroadphase() =
    inherit Broadphase(0.)
    member __.axisList with get(): array<Body> = jsNative and set(v: array<Body>): unit = jsNative
    member __.axisIndex with get(): float = jsNative and set(v: float): unit = jsNative
    member __.sortList(): unit = jsNative

and [<AllowNullLiteral>] [<Import("Constraint","p2")>] Constraint(bodyA: Body, bodyB: Body, ``type``: float, ?options: obj) =
    member __.DISTANCE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.GEAR with get(): float = jsNative and set(v: float): unit = jsNative
    member __.LOCK with get(): float = jsNative and set(v: float): unit = jsNative
    member __.PRISMATIC with get(): float = jsNative and set(v: float): unit = jsNative
    member __.REVOLUTE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.``type`` with get(): float = jsNative and set(v: float): unit = jsNative
    member __.equeations with get(): array<Equation> = jsNative and set(v: array<Equation>): unit = jsNative
    member __.bodyA with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.bodyB with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.collideConnected with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.update(): unit = jsNative
    member __.setStiffness(stiffness: float): unit = jsNative
    member __.setRelaxation(relaxation: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("DistanceConstraint","p2")>] DistanceConstraint(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Constraint(bodyA, bodyB, 0.)
    member __.localAnchorA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.localAnchorB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.distance with get(): float = jsNative and set(v: float): unit = jsNative
    member __.maxForce with get(): float = jsNative and set(v: float): unit = jsNative
    member __.upperLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.upperLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.lowerLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.lowerLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.position with get(): float = jsNative and set(v: float): unit = jsNative
    member __.setMaxForce(f: float): unit = jsNative
    member __.getMaxForce(): float = jsNative

and [<AllowNullLiteral>] [<Import("GearConstraint","p2")>] GearConstraint(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Constraint(bodyA, bodyB, 0.)
    member __.ratio with get(): float = jsNative and set(v: float): unit = jsNative
    member __.angle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.setMaxTorque(torque: float): unit = jsNative
    member __.getMaxTorque(): float = jsNative

and [<AllowNullLiteral>] [<Import("LockConstraint","p2")>] LockConstraint(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Constraint(bodyA, bodyB, 0.)
    member __.setMaxForce(force: float): unit = jsNative
    member __.getMaxForce(): float = jsNative

and [<AllowNullLiteral>] [<Import("PrismaticConstraint","p2")>] PrismaticConstraint(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Constraint(bodyA, bodyB, 0.)
    member __.localAnchorA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.localAnchorB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.localAxisA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.position with get(): float = jsNative and set(v: float): unit = jsNative
    member __.velocity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.lowerLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.upperLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.lowerLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.upperLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.upperLimitEquation with get(): ContactEquation = jsNative and set(v: ContactEquation): unit = jsNative
    member __.lowerLimitEquation with get(): ContactEquation = jsNative and set(v: ContactEquation): unit = jsNative
    member __.motorEquation with get(): Equation = jsNative and set(v: Equation): unit = jsNative
    member __.motorEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.motorSpeed with get(): float = jsNative and set(v: float): unit = jsNative
    member __.enableMotor(): unit = jsNative
    member __.disableMotor(): unit = jsNative
    member __.setLimits(lower: float, upper: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("RevoluteConstraint","p2")>] RevoluteConstraint(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Constraint(bodyA, bodyB, 0.)
    member __.pivotA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.pivotB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.motorEquation with get(): RotationalVelocityEquation = jsNative and set(v: RotationalVelocityEquation): unit = jsNative
    member __.motorEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.angle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.lowerLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.upperLimitEnabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.lowerLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.upperLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.upperLimitEquation with get(): ContactEquation = jsNative and set(v: ContactEquation): unit = jsNative
    member __.lowerLimitEquation with get(): ContactEquation = jsNative and set(v: ContactEquation): unit = jsNative
    member __.enableMotor(): unit = jsNative
    member __.disableMotor(): unit = jsNative
    member __.motorIsEnabled(): bool = jsNative
    member __.setLimits(lower: float, upper: float): unit = jsNative
    member __.setMotorSpeed(speed: float): unit = jsNative
    member __.getMotorSpeed(): float = jsNative

and [<AllowNullLiteral>] [<Import("AngleLockEquation","p2")>] AngleLockEquation(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Equation(bodyA, bodyB)
    member __.computeGq(): float = jsNative
    member __.setRatio(ratio: float): float = jsNative
    member __.setMaxTorque(torque: float): float = jsNative

and [<AllowNullLiteral>] [<Import("ContactEquation","p2")>] ContactEquation(bodyA: Body, bodyB: Body) =
    inherit Equation(bodyA, bodyB)
    member __.contactPointA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.penetrationVec with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.contactPointB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.normalA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.restitution with get(): float = jsNative and set(v: float): unit = jsNative
    member __.firstImpact with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.shapeA with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.shapeB with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.computeB(a: float, b: float, h: float): float = jsNative

and [<AllowNullLiteral>] [<Import("Equation","p2")>] Equation(bodyA: Body, bodyB: Body, ?minForce: float, ?maxForce: float) =
    member __.DEFAULT_STIFFNESS with get(): float = jsNative and set(v: float): unit = jsNative
    member __.DEFAULT_RELAXATION with get(): float = jsNative and set(v: float): unit = jsNative
    member __.minForce with get(): float = jsNative and set(v: float): unit = jsNative
    member __.maxForce with get(): float = jsNative and set(v: float): unit = jsNative
    member __.bodyA with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.bodyB with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.stiffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.relaxation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.G with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.offset with get(): float = jsNative and set(v: float): unit = jsNative
    member __.a with get(): float = jsNative and set(v: float): unit = jsNative
    member __.b with get(): float = jsNative and set(v: float): unit = jsNative
    member __.epsilon with get(): float = jsNative and set(v: float): unit = jsNative
    member __.timeStep with get(): float = jsNative and set(v: float): unit = jsNative
    member __.needsUpdate with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.multiplier with get(): float = jsNative and set(v: float): unit = jsNative
    member __.relativeVelocity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.enabled with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.gmult(G: (float*float), vi: (float*float), wi: (float*float), vj: (float*float), wj: (float*float)): float = jsNative
    member __.computeB(a: float, b: float, h: float): float = jsNative
    member __.computeGq(): float = jsNative
    member __.computeGW(): float = jsNative
    member __.computeGWlambda(): float = jsNative
    member __.computeGiMf(): float = jsNative
    member __.computeGiMGt(): float = jsNative
    member __.addToWlambda(deltalambda: float): float = jsNative
    member __.computeInvC(eps: float): float = jsNative

and [<AllowNullLiteral>] [<Import("FrictionEquation","p2")>] FrictionEquation(bodyA: Body, bodyB: Body, slipForce: float) =
    inherit Equation(bodyA, bodyB)
    member __.contactPointA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.contactPointB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.t with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.shapeA with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.shapeB with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.frictionCoefficient with get(): float = jsNative and set(v: float): unit = jsNative
    member __.setSlipForce(slipForce: float): float = jsNative
    member __.getSlipForce(): float = jsNative
    member __.computeB(a: float, b: float, h: float): float = jsNative

and [<AllowNullLiteral>] [<Import("RotationalLockEquation","p2")>] RotationalLockEquation(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Equation(bodyA, bodyB)
    member __.angle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.computeGq(): float = jsNative

and [<AllowNullLiteral>] [<Import("RotationalVelocityEquation","p2")>] RotationalVelocityEquation(bodyA: Body, bodyB: Body) =
    inherit Equation(bodyA, bodyB)
    member __.computeB(a: float, b: float, h: float): float = jsNative

and [<AllowNullLiteral>] [<Import("EventEmitter","p2")>] EventEmitter() =
    member __.on(``type``: string, listener: obj->unit, ?context: obj): EventEmitter = jsNative
    member __.has(``type``: string, listener: obj->unit): bool = jsNative
    member __.off(``type``: string, listener: obj->unit): EventEmitter = jsNative
    member __.emit(``event``: obj): EventEmitter = jsNative

and [<AllowNullLiteral>] ContactMaterialOptions =
    abstract friction: float option with get, set
    abstract restitution: float option with get, set
    abstract stiffness: float option with get, set
    abstract relaxation: float option with get, set
    abstract frictionStiffness: float option with get, set
    abstract frictionRelaxation: float option with get, set
    abstract surfaceVelocity: float option with get, set

and [<AllowNullLiteral>] [<Import("ContactMaterial","p2")>] ContactMaterial(materialA: Material, materialB: Material, ?options: ContactMaterialOptions) =
    member __.idCounter with get(): float = jsNative and set(v: float): unit = jsNative
    member __.id with get(): float = jsNative and set(v: float): unit = jsNative
    member __.materialA with get(): Material = jsNative and set(v: Material): unit = jsNative
    member __.materialB with get(): Material = jsNative and set(v: Material): unit = jsNative
    member __.friction with get(): float = jsNative and set(v: float): unit = jsNative
    member __.restitution with get(): float = jsNative and set(v: float): unit = jsNative
    member __.stiffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.relaxation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.frictionStuffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.frictionRelaxation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.surfaceVelocity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.contactSkinSize with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("Material","p2")>] Material(?id: float) =
    member __.idCounter with get(): float = jsNative and set(v: float): unit = jsNative
    member __.id with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("vec2","p2")>] vec2() =
    static member crossLength(a: (float*float), b: (float*float)): float = jsNative
    static member crossVZ(out: (float*float), vec: (float*float), zcomp: float): float = jsNative
    static member crossZV(out: (float*float), zcomp: float, vec: (float*float)): float = jsNative
    static member rotate(out: (float*float), a: (float*float), angle: float): unit = jsNative
    static member rotate90cw(out: (float*float), a: (float*float)): float = jsNative
    static member centroid(out: (float*float), a: (float*float), b: (float*float), c: (float*float)): (float*float) = jsNative
    static member create(): (float*float) = jsNative
    static member clone(a: (float*float)): (float*float) = jsNative
    static member fromValues(x: float, y: float): (float*float) = jsNative
    static member copy(out: (float*float), a: (float*float)): (float*float) = jsNative
    static member set(out: (float*float), x: float, y: float): (float*float) = jsNative
    static member toLocalFrame(out: (float*float), worldPoint: (float*float), framePosition: (float*float), frameAngle: float): unit = jsNative
    static member toGlobalFrame(out: (float*float), localPoint: (float*float), framePosition: (float*float), frameAngle: float): unit = jsNative
    static member add(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member subtract(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member sub(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member multiply(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member mul(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member divide(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member div(out: (float*float), a: (float*float), b: (float*float)): (float*float) = jsNative
    static member scale(out: (float*float), a: (float*float), b: float): (float*float) = jsNative
    static member distance(a: (float*float), b: (float*float)): float = jsNative
    static member dist(a: (float*float), b: (float*float)): float = jsNative
    static member squaredDistance(a: (float*float), b: (float*float)): float = jsNative
    static member sqrDist(a: (float*float), b: (float*float)): float = jsNative
    static member length(a: (float*float)): float = jsNative
    static member len(a: (float*float)): float = jsNative
    static member squaredLength(a: (float*float)): float = jsNative
    static member sqrLen(a: (float*float)): float = jsNative
    static member negate(out: (float*float), a: (float*float)): (float*float) = jsNative
    static member normalize(out: (float*float), a: (float*float)): (float*float) = jsNative
    static member dot(a: (float*float), b: (float*float)): float = jsNative
    static member str(a: (float*float)): string = jsNative

and [<AllowNullLiteral>] BodyOptions =
    abstract mass: float option with get, set
    abstract position: (float*float) option with get, set
    abstract velocity: (float*float) option with get, set
    abstract angle: float option with get, set
    abstract angularVelocity: float option with get, set
    abstract force: (float*float) option with get, set
    abstract angularForce: float option with get, set
    abstract fixedRotation: bool option with get, set
    abstract allowSleep: bool option with get, set
    abstract collisionResponse: bool option with get, set
    abstract ccdIterations: float option with get, set
    abstract ccdSpeedThreshold: float option with get, set
    abstract gravityScale: float option with get, set
    abstract sleepSpeedLimit: float option with get, set
    abstract sleepTimeLimit: float option with get, set
    abstract damping: float option with get, set
    abstract angularDamping: float option with get, set

and [<AllowNullLiteral>] [<Import("Body","p2")>] Body(?options: BodyOptions) =
    inherit EventEmitter()
    member __.sleepyEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.sleepEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.wakeUpEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.DYNAMIC with get(): float = jsNative and set(v: float): unit = jsNative
    member __.STATIC with get(): float = jsNative and set(v: float): unit = jsNative
    member __.KINEMATIC with get(): float = jsNative and set(v: float): unit = jsNative
    member __.AWAKE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.SLEEPY with get(): float = jsNative and set(v: float): unit = jsNative
    member __.SLEEPING with get(): float = jsNative and set(v: float): unit = jsNative
    member __.id with get(): float = jsNative and set(v: float): unit = jsNative
    member __.world with get(): World = jsNative and set(v: World): unit = jsNative
    member __.shapes with get(): array<Shape> = jsNative and set(v: array<Shape>): unit = jsNative
    member __.mass with get(): float = jsNative and set(v: float): unit = jsNative
    member __.invMass with get(): float = jsNative and set(v: float): unit = jsNative
    member __.inertia with get(): float = jsNative and set(v: float): unit = jsNative
    member __.invInertia with get(): float = jsNative and set(v: float): unit = jsNative
    member __.invMassSolve with get(): float = jsNative and set(v: float): unit = jsNative
    member __.invInertiaSolve with get(): float = jsNative and set(v: float): unit = jsNative
    member __.fixedRotation with get(): float = jsNative and set(v: float): unit = jsNative
    member __.position with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.interpolatedPosition with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.interpolatedAngle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.previousPosition with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.previousAngle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.velocity with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.vlambda with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.wlambda with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.angle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.angularVelocity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.force with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.angularForce with get(): float = jsNative and set(v: float): unit = jsNative
    member __.damping with get(): float = jsNative and set(v: float): unit = jsNative
    member __.angularDamping with get(): float = jsNative and set(v: float): unit = jsNative
    member __.``type`` with get(): float = jsNative and set(v: float): unit = jsNative
    member __.boundingRadius with get(): float = jsNative and set(v: float): unit = jsNative
    member __.aabb with get(): AABB = jsNative and set(v: AABB): unit = jsNative
    member __.aabbNeedsUpdate with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.allowSleep with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.wantsToSleep with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.sleepState with get(): float = jsNative and set(v: float): unit = jsNative
    member __.sleepSpeedLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.sleepTimeLimit with get(): float = jsNative and set(v: float): unit = jsNative
    member __.gravityScale with get(): float = jsNative and set(v: float): unit = jsNative
    member __.collisionResponse with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.updateSolveMassProperties(): unit = jsNative
    member __.setDensity(density: float): unit = jsNative
    member __.getArea(): float = jsNative
    member __.getAABB(): AABB = jsNative
    member __.updateAABB(): unit = jsNative
    member __.updateBoundingRadius(): unit = jsNative
    member __.addShape(shape: Shape, ?offset: (float*float), ?angle: float): unit = jsNative
    member __.removeShape(shape: Shape): bool = jsNative
    member __.updateMassProperties(): unit = jsNative
    member __.applyForce(force: (float*float), ?relativePoint: (float*float)): unit = jsNative
    member __.applyForceLocal(localForce: (float*float), ?localPoint: (float*float)): unit = jsNative
    member __.toLocalFrame(out: (float*float), worldPoint: (float*float)): unit = jsNative
    member __.toWorldFrame(out: (float*float), localPoint: (float*float)): unit = jsNative
    member __.fromPolygon(path: array<(float*float)>, ?options: obj): bool = jsNative
    member __.adjustCenterOfMass(): unit = jsNative
    member __.setZeroForce(): unit = jsNative
    member __.resetConstraintVelocity(): unit = jsNative
    member __.applyDamping(dy: float): unit = jsNative
    member __.wakeUp(): unit = jsNative
    member __.sleep(): unit = jsNative
    member __.sleepTick(time: float, dontSleep: bool, dt: float): unit = jsNative
    member __.getVelocityFromPosition(story: (float*float), dt: float): (float*float) = jsNative
    member __.getAngularVelocityFromPosition(timeStep: float): float = jsNative
    member __.overlaps(body: Body): bool = jsNative

and [<AllowNullLiteral>] [<Import("Spring","p2")>] Spring(bodyA: Body, bodyB: Body, ?options: obj) =
    member __.stiffness with get(): float = jsNative and set(v: float): unit = jsNative
    member __.damping with get(): float = jsNative and set(v: float): unit = jsNative
    member __.bodyA with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.bodyB with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.applyForce(): unit = jsNative

and [<AllowNullLiteral>] [<Import("LinearSpring","p2")>] LinearSpring(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Spring(null, null)
    member __.localAnchorA with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.localAnchorB with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.restLength with get(): float = jsNative and set(v: float): unit = jsNative
    member __.setWorldAnchorA(worldAnchorA: (float*float)): unit = jsNative
    member __.setWorldAnchorB(worldAnchorB: (float*float)): unit = jsNative
    member __.getWorldAnchorA(result: (float*float)): (float*float) = jsNative
    member __.getWorldAnchorB(result: (float*float)): (float*float) = jsNative
    member __.applyForce(): unit = jsNative

and [<AllowNullLiteral>] [<Import("Ray","p2")>] Ray(?options: obj) =
    member __.from with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.``to`` with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.length with get(): float = jsNative and set(v: float): unit = jsNative
    member __.CLOSEST with get(): float = jsNative and set(v: float): unit = jsNative
    member __.ANY with get(): float = jsNative and set(v: float): unit = jsNative
    member __.ALL with get(): float = jsNative and set(v: float): unit = jsNative
    member __.update(): unit = jsNative

and [<AllowNullLiteral>] [<Import("RaycastResult","p2")>] RaycastResult() =
    member __.normal with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.shape with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.body with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.faceIndex with get(): float = jsNative and set(v: float): unit = jsNative
    member __.fraction with get(): float = jsNative and set(v: float): unit = jsNative
    member __.isStopped with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.reset(): unit = jsNative
    member __.getHitDistance(): float = jsNative
    member __.hasHit(): bool = jsNative
    member __.stop(): bool = jsNative
    member __.getHitPoint(out: (float*float), ray: Ray): unit = jsNative
    member __.shouldStop(ray: Ray): bool = jsNative
    member __.set(normal: (float*float), shape: Shape, body: Body, fraction: float, faceIndex: float): bool = jsNative

and [<AllowNullLiteral>] [<Import("RotationalSpring","p2")>] RotationalSpring(bodyA: Body, bodyB: Body, ?options: obj) =
    inherit Spring(bodyA, bodyB)
    member __.restAngle with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] CapsuleOptions =
    inherit SharedShapeOptions
    abstract length: float option with get, set
    abstract radius: float option with get, set

and [<AllowNullLiteral>] [<Import("Capsule","p2")>] Capsule(?options: CapsuleOptions) =
    inherit Shape()
    member __.length with get(): float = jsNative and set(v: float): unit = jsNative
    member __.radius with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] CircleOptions =
    inherit SharedShapeOptions
    abstract radius: float option with get, set

and [<AllowNullLiteral>] [<Import("Circle","p2")>] Circle(?options: CircleOptions) =
    inherit Shape()
    member __.radius with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] ConvexOptions =
    inherit SharedShapeOptions
    abstract vertices: array<(float*float)> option with get, set
    abstract axes: array<(float*float)> option with get, set

and [<AllowNullLiteral>] [<Import("Convex","p2")>] Convex(?options: ConvexOptions) =
    inherit Shape()
    member __.vertices with get(): array<(float*float)> = jsNative and set(v: array<(float*float)>): unit = jsNative
    member __.axes with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.centerOfMass with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.triangles with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.boundingRadius with get(): float = jsNative and set(v: float): unit = jsNative
    static member triangleArea(a: (float*float), b: (float*float), c: (float*float)): float = jsNative
    member __.projectOntoLocalAxis(localAxis: (float*float), result: (float*float)): unit = jsNative
    member __.projectOntoWorldAxis(localAxis: (float*float), shapeOffset: (float*float), shapeAngle: float, result: (float*float)): unit = jsNative
    member __.updateCenterOfMass(): unit = jsNative

and [<AllowNullLiteral>] HeightfieldOptions =
    inherit SharedShapeOptions
    abstract heights: (float*float) option with get, set
    abstract minValue: float option with get, set
    abstract maxValue: float option with get, set
    abstract elementWidth: float option with get, set

and [<AllowNullLiteral>] [<Import("Heightfield","p2")>] Heightfield(?options: HeightfieldOptions) =
    inherit Shape()
    member __.data with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.maxValue with get(): float = jsNative and set(v: float): unit = jsNative
    member __.minValue with get(): float = jsNative and set(v: float): unit = jsNative
    member __.elementWidth with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] SharedShapeOptions =
    abstract position: (float*float) option with get, set
    abstract angle: float option with get, set
    abstract collisionGroup: int option with get, set
    abstract collisionResponse: bool option with get, set
    abstract collisionMask: int option with get, set
    abstract sensor: bool option with get, set

and [<AllowNullLiteral>] ShapeOptions =
    inherit SharedShapeOptions
    abstract ``type``: float option with get, set

and [<AllowNullLiteral>] [<Import("Shape","p2")>] Shape(?options: ShapeOptions) =
    member __.idCounter with get(): float = jsNative and set(v: float): unit = jsNative
    member __.CIRCLE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.PARTICLE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.PLANE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.CONVEX with get(): float = jsNative and set(v: float): unit = jsNative
    member __.LINE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.BOX with get(): float = jsNative and set(v: float): unit = jsNative
    member __.CAPSULE with get(): float = jsNative and set(v: float): unit = jsNative
    member __.HEIGHTFIELD with get(): float = jsNative and set(v: float): unit = jsNative
    member __.``type`` with get(): float = jsNative and set(v: float): unit = jsNative
    member __.id with get(): float = jsNative and set(v: float): unit = jsNative
    member __.position with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.angle with get(): float = jsNative and set(v: float): unit = jsNative
    member __.boundingRadius with get(): float = jsNative and set(v: float): unit = jsNative
    member __.collisionGroup with get(): float = jsNative and set(v: float): unit = jsNative
    member __.collisionResponse with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.collisionMask with get(): float = jsNative and set(v: float): unit = jsNative
    member __.material with get(): Material = jsNative and set(v: Material): unit = jsNative
    member __.area with get(): float = jsNative and set(v: float): unit = jsNative
    member __.sensor with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.computeMomentOfInertia(mass: float): float = jsNative
    member __.updateBoundingRadius(): float = jsNative
    member __.updateArea(): unit = jsNative
    member __.computeAABB(out: AABB, position: (float*float), angle: float): unit = jsNative

and [<AllowNullLiteral>] LineOptions =
    inherit SharedShapeOptions
    abstract length: float option with get, set

and [<AllowNullLiteral>] [<Import("Line","p2")>] Line(?options: LineOptions) =
    inherit Shape()
    member __.length with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("Particle","p2")>] Particle(?options: SharedShapeOptions) =
    inherit Shape()

and [<AllowNullLiteral>] [<Import("Plane","p2")>] Plane(?options: SharedShapeOptions) =
    inherit Shape()

and [<AllowNullLiteral>] BoxOptions =
    abstract width: float option with get, set
    abstract height: float option with get, set

and [<AllowNullLiteral>] [<Import("Box","p2")>] Box(?options: BoxOptions) =
    inherit Shape()
    member __.width with get(): float = jsNative and set(v: float): unit = jsNative
    member __.height with get(): float = jsNative and set(v: float): unit = jsNative

and [<AllowNullLiteral>] [<Import("Solver","p2")>] Solver(?options: obj, ?``type``: float) =
    inherit EventEmitter()
    member __.GS with get(): float = jsNative and set(v: float): unit = jsNative
    member __.ISLAND with get(): float = jsNative and set(v: float): unit = jsNative
    member __.``type`` with get(): float = jsNative and set(v: float): unit = jsNative
    member __.equations with get(): array<Equation> = jsNative and set(v: array<Equation>): unit = jsNative
    member __.equationSortFunction with get(): Equation = jsNative and set(v: Equation): unit = jsNative
    member __.solve(dy: float, world: World): unit = jsNative
    member __.solveIsland(dy: float, island: Island): unit = jsNative
    member __.sortEquations(): unit = jsNative
    member __.addEquation(eq: Equation): unit = jsNative
    member __.addEquations(eqs: array<Equation>): unit = jsNative
    member __.removeEquation(eq: Equation): unit = jsNative
    member __.removeAllEquations(): unit = jsNative

and [<AllowNullLiteral>] [<Import("GSSolver","p2")>] GSSolver(?options: obj) =
    inherit Solver()
    member __.iterations with get(): float = jsNative and set(v: float): unit = jsNative
    member __.tolerance with get(): float = jsNative and set(v: float): unit = jsNative
    member __.useZeroRHS with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.frictionIterations with get(): float = jsNative and set(v: float): unit = jsNative
    member __.usedIterations with get(): float = jsNative and set(v: float): unit = jsNative
    member __.solve(h: float, world: World): unit = jsNative

and [<AllowNullLiteral>] [<Import("OverlapKeeper","p2")>] OverlapKeeper(bodyA: Body, shapeA: Shape, bodyB: Body, shapeB: Shape) =
    member __.shapeA with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.shapeB with get(): Shape = jsNative and set(v: Shape): unit = jsNative
    member __.bodyA with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.bodyB with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.tick(): unit = jsNative
    member __.setOverlapping(bodyA: Body, shapeA: Shape, bodyB: Body, shapeB: Body): unit = jsNative
    member __.bodiesAreOverlapping(bodyA: Body, bodyB: Body): bool = jsNative
    member __.set(bodyA: Body, shapeA: Shape, bodyB: Body, shapeB: Shape): unit = jsNative

and [<AllowNullLiteral>] [<Import("TupleDictionary","p2")>] TupleDictionary() =
    member __.data with get(): array<float> = jsNative and set(v: array<float>): unit = jsNative
    member __.keys with get(): array<float> = jsNative and set(v: array<float>): unit = jsNative
    member __.getKey(id1: float, id2: float): string = jsNative
    member __.getByKey(key: float): float = jsNative
    member __.get(i: float, j: float): float = jsNative
    member __.set(i: float, j: float, value: float): float = jsNative
    member __.reset(): unit = jsNative
    member __.copy(dict: TupleDictionary): unit = jsNative

and [<AllowNullLiteral>] [<Import("Utils","p2")>] Utils() =
    static member appendArray(a: array<'T>, b: array<'T>): array<'T> = jsNative
    static member splice(array: array<'T>, index: float, howMany: float): unit = jsNative
    static member extend(a: obj, b: obj): unit = jsNative
    static member defaults(options: obj, defaults: obj): obj = jsNative

and [<AllowNullLiteral>] [<Import("Island","p2")>] Island() =
    member __.equations with get(): array<Equation> = jsNative and set(v: array<Equation>): unit = jsNative
    member __.bodies with get(): array<Body> = jsNative and set(v: array<Body>): unit = jsNative
    member __.reset(): unit = jsNative
    member __.getBodies(result: obj): array<Body> = jsNative
    member __.wantsToSleep(): bool = jsNative
    member __.sleep(): bool = jsNative

and [<AllowNullLiteral>] [<Import("IslandManager","p2")>] IslandManager() =
    inherit Solver()
    member __.equations with get(): array<Equation> = jsNative and set(v: array<Equation>): unit = jsNative
    member __.islands with get(): array<Island> = jsNative and set(v: array<Island>): unit = jsNative
    member __.nodes with get(): array<IslandNode> = jsNative and set(v: array<IslandNode>): unit = jsNative
    static member getUnvisitedNode(nodes: array<IslandNode>): IslandNode = jsNative
    member __.visit(node: IslandNode, bds: array<Body>, eqs: array<Equation>): unit = jsNative
    member __.bfs(root: IslandNode, bds: array<Body>, eqs: array<Equation>): unit = jsNative
    member __.split(world: World): array<Island> = jsNative

and [<AllowNullLiteral>] [<Import("IslandNode","p2")>] IslandNode(body: Body) =
    member __.body with get(): Body = jsNative and set(v: Body): unit = jsNative
    member __.neighbors with get(): array<IslandNode> = jsNative and set(v: array<IslandNode>): unit = jsNative
    member __.equations with get(): array<Equation> = jsNative and set(v: array<Equation>): unit = jsNative
    member __.visited with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.reset(): unit = jsNative

and [<AllowNullLiteral>] [<Import("World","p2")>] World(?options: obj) =
    inherit EventEmitter()
    member __.postStepEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.addBodyEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.removeBodyEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.addSpringEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.impactEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.postBroadphaseEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.beginContactEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.endContactEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.preSolveEvent with get(): obj = jsNative and set(v: obj): unit = jsNative
    member __.NO_SLEEPING with get(): float = jsNative and set(v: float): unit = jsNative
    member __.BODY_SLEEPING with get(): float = jsNative and set(v: float): unit = jsNative
    member __.ISLAND_SLEEPING with get(): float = jsNative and set(v: float): unit = jsNative
    member __.springs with get(): array<Spring> = jsNative and set(v: array<Spring>): unit = jsNative
    member __.bodies with get(): array<Body> = jsNative and set(v: array<Body>): unit = jsNative
    member __.solver with get(): Solver = jsNative and set(v: Solver): unit = jsNative
    member __.narrowphase with get(): Narrowphase = jsNative and set(v: Narrowphase): unit = jsNative
    member __.islandManager with get(): IslandManager = jsNative and set(v: IslandManager): unit = jsNative
    member __.gravity with get(): (float*float) = jsNative and set(v: (float*float)): unit = jsNative
    member __.frictionGravity with get(): float = jsNative and set(v: float): unit = jsNative
    member __.useWorldGravityAsFrictionGravity with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.useFrictionGravityOnZeroGravity with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.doProfiling with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.lastStepTime with get(): float = jsNative and set(v: float): unit = jsNative
    member __.broadphase with get(): Broadphase = jsNative and set(v: Broadphase): unit = jsNative
    member __.constraints with get(): array<Constraint> = jsNative and set(v: array<Constraint>): unit = jsNative
    member __.defaultMaterial with get(): Material = jsNative and set(v: Material): unit = jsNative
    member __.defaultContactMaterial with get(): ContactMaterial = jsNative and set(v: ContactMaterial): unit = jsNative
    member __.lastTimeStep with get(): float = jsNative and set(v: float): unit = jsNative
    member __.applySpringForces with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.applyDamping with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.applyGravity with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.solveConstraints with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.contactMaterials with get(): array<ContactMaterial> = jsNative and set(v: array<ContactMaterial>): unit = jsNative
    member __.time with get(): float = jsNative and set(v: float): unit = jsNative
    member __.stepping with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.islandSplit with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.emitImpactEvent with get(): bool = jsNative and set(v: bool): unit = jsNative
    member __.sleepMode with get(): float = jsNative and set(v: float): unit = jsNative
    static member integrateBody(body: Body, dy: float): unit = jsNative
    member __.addConstraint(c: Constraint): unit = jsNative
    member __.addContactMaterial(contactMaterial: ContactMaterial): unit = jsNative
    member __.removeContactMaterial(cm: ContactMaterial): unit = jsNative
    member __.getContactMaterial(materialA: Material, materialB: Material): ContactMaterial = jsNative
    member __.removeConstraint(c: Constraint): unit = jsNative
    member __.step(dy: float, ?timeSinceLastCalled: float, ?maxSubSteps: float): unit = jsNative
    member __.runNarrowphase(np: Narrowphase, bi: Body, si: Shape, xi: array<obj>, ai: float, bj: Body, sj: Shape, xj: array<obj>, aj: float, cm: float, glen: float): unit = jsNative
    member __.addSpring(s: Spring): unit = jsNative
    member __.removeSpring(s: Spring): unit = jsNative
    member __.addBody(body: Body): unit = jsNative
    member __.removeBody(body: Body): unit = jsNative
    member __.getBodyByID(id: float): Body = jsNative
    member __.disableBodyCollision(bodyA: Body, bodyB: Body): unit = jsNative
    member __.enableBodyCollision(bodyA: Body, bodyB: Body): unit = jsNative
    member __.clear(): unit = jsNative
    member __.clone(): World = jsNative
    member __.hitTest(worldPoint: (float*float), bodies: array<Body>, precision: float): array<Body> = jsNative
    member __.setGlobalEquationParameters(parameters: obj): unit = jsNative
    member __.setGlobalStiffness(stiffness: float): unit = jsNative
    member __.setGlobalRelaxation(relaxation: float): unit = jsNative
