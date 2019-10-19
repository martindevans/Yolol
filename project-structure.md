## Yolol

This library is for the basics of interacting with Yolol from C#.

### Execution

This namespace contains any classes to do with executing a Yolol AST. A `MachineState` stores the total state of a Yolol program (i.e. variable values) and can be passed from line to line as a script is executed. A `MachineState` references an `IDeviceNetwork` which provides a way of reading/writing external variables, there are some default implementations of this which just always return zero (`NullDeviceNetwork`) or throw an exception if an external is accessed (`ThrowDeviceNetwork`). The `ExecutionException` represents a runtime error in the Yolol program, these can be caught when executing as a signal to proceed to the next line.

### Grammar

This namespace contains classes for tokenising and parsing Yolol code. The `Tokenizer` splits a string into tokens, the `Parser` consumes the tokens and converts it into an AST.

## YololEmulator

This executable is a commandline emulator for Yolol, it consumes a files of Yolol and executes it line by line (every time `F5` is pressed), displaying the compete state of the program each time. Most of the logic is in the `Program` class.

### Network

This namespace contains some additional implementations of the `IDeviceNetwork`. `ConsoleInputDeviceNetwork` freezes the program until the user to provides a value (so the user can emulate an external device network "on paper"). The `Http` namespace provides an implementation of a device network which gets/sets values through HTTP, allowing multiple instances of the emulator to be used together to simulate a device network.

## Yolol.Cylon

This library provides serializers/deserializers to the Cylon JSON AST spec. Serializers take an AST and output JSON. Deserializers take JSON and output an AST.

## Yololc

This executable is a commandline optimiser for Yolol, it consumes a file of Yolol and outputs better (i.e. smaller) Yolol. The program itself is quite simple (just commandline configuration, logging etc) as the real logic of all optimisations is contained within `Yolol.Analysis`.

## Yolol.Analysis

This library provides methods for analysing and optimising Yolol ASTs. Optimisation or analysis work is done in a "pass" which does all of it's work and produces a set of results before the next pass can run. `OptimisationPipeline` is the core of the entire project, this chains together all the other analysis/optimisation passes into a useful pipeline.

### TreeVisitor

This namespace contains analysis/optimisation passes which work on the basis of "visiting" the AST, recursively, one node at a time. Tree visitor passes are generally very simple to write but are very limited in what they do achieve because they can only "see" one node at a time - many of these passes will be better if they were rewritten as CFG analysis passes (once CFG analysis is stable).

`BaseTreeVisitor` visits an entire AST (expression and statements) and returns another AST. `BaseStatementVisitor` visits just the statements of an AST and returns some other type. `BaseExpressionVisitor` visits just the expressions of an AST and returns some other type.

`ProgramDecomposition` is a pass which takes an AST and converts it into a much simpler form, removing many of the unnecesary complexity of Yolol (implicit ordering, compound operations, mid expression errors etc). For example:

    a = b * :c + :d

becomes:

    tmp1 = b
    tmp2 = :c
    tmp3 = tmp1 * tmp2     // This could throw, meaning `:d` is never read
    tmp4 = :d
    tmp5 = tmp3 + tmp4
    a = tmp5

This simpler form is required for CFG conversion.

### TreeVisitor.Inspection

This namespace contains AST visitor passes which analyse the tree but do not modify it.

### TreeVisitor.Reduction

This namespace contains AST visitor passes which modify the tree to reduce it.

## ControlFlowGraph

This contains classes for representing Yolol as a "control flow graph" (`IControlFlowGraph`) - blocks of code (`IBasicBlock`) connected by edges (`IEdge`) which represent different moves between blocks (e.g. fallthrough, goto, runtime error, conditionals etc). You can see an example of a CFG graph [here](https://tinyurl.com/y6dec6oq).

Things in this namespace are generally represented twice, as an _immutable_ interface and as a _mutable_ class. When building a new CFG you will use using the class, when consuming a CFG that's already been constructed you will use the interfaces. If you want to modify an already existing CFG you should create a _new_ CFG and copy across the old one, with modifications applied in the copy.

Before a program is converted into a CFG it must be simplified (using `ProgramDecomposition`) and it must have `Single Static Assignment` form, i.e. each variable must be assigned at most once. For example this program:

    a = 1
    a = b * 2
    a = a + 3
    a *= 3

Would convert into:

    a[0] = 1
    a[1] = b[0] * 2
    a[2] = a[1] + 3
    a[3] = a[2] * 3

### ControlFlowGraph.Extensions

This namespace contains a variety of extension methods to the basic control flow graph which add passes for analysing, modifying, reducing and serialising the graph.

### ControlFlowGraph.AST

This namespace contains additional AST nodes which are used within the CFG representation. These nodes are generally simpler alternatives of language constructs in Yolol and must all be removed before converting the program back into Yolol.

#### Conditional(expression)

Represents a branch in execution. This must be the last statements in a basic block and the block must have two exit edges labelled "conditional true" and "conditional false" - which branch is taken depends upon the expression.

#### Increment(VatiableName) & Decrement(VariableName)

Increments/Decrements a variable and returns the value, does _not_ modify the value of the variables.

#### ErrorExpression & ErrorStatement

Unconditionally causes a runtime error, either as an item in an expression of a statements. e.g.

    a = 1 * "b"

Could be replaced with:

    a = ErrorExpression()

Which could further be replaced with:

    ErrorStatement()

#### Phi

When a program is in SSA form a variable access may access multiple other variables simultaneously, for example:

    a = 1
    if :condition then a = 2 else a = a - 1 end
    :output = a

The final output value may be the value from either branch. In SSA form that looks like this:

    a[0] = 1
    if :condition then a[1] = 2 else a[2] = a[0] - 1 end
    :output = Phi(a[1], a[2])

#### TypedAssignment

This represents assigning a variable with a known type. For example

    a = 1

Can be annoted with types:

    a = (Number)1;

## DataFlowGraph

A Data Flow Graph represents the flow of data within a single basic block, you can see an example DFG [here](https://tinyurl.com/y5qu6wq8). The DFG has inputs (constants, variables assigned in a previous block, externals) operations (equality, multiplication, addition etc) and outputs (assigned variables, goto, conditional etc). The DFG follows the same pattern of immutability as the CFG - an `IDataFlowGraph` is immutable, a `DataFlowGraph` is mutable. If you want to modify a DFG you should create a new one and copy across the DFG, applying the modification as part of the copy.

### Types

This namespace contains classes to do with type inference of Yolol code. `ITypeAssignments` is a table which can be queried for the type of a variable. `FlowTyping` contains extension methods on the CFG for adding and removing type annotations. `ExpressionTypeInference` is an AST visitor which infers the type of expressions.

Types are a bitset, so a given expression can have multiple types. For example:

    a = 1
    b = "hello"
    if :condition then c = a else c = b end

If you queried the final type of each variable you would get:

    a = (Number)
    b = (String)
    c = (Number|String)