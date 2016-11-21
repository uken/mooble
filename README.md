# Mooble

## Static Analysis

Mooble provides tools to perform static analysis on prefabs and scene files -
often unchecked in code review and hard to test without someone running the
scene and going through every possible flow. Mooble's goal is to help catch
errors ahead of time by checking for common errors in scene and prefab files,
and allowing you to customize the static analysis by writing your own rules.

### Built-in Rules

#### `NoDuplicateComponents`

This rule makes sure that `GameObjects` do not have two of the same type of
`Component`, e.g.  no `GameObject` has two `Text` components, etc.

#### `NoInactiveBehaviours`

This rule ensures that no
[`Behaviour`](https://docs.unity3d.com/ScriptReference/Behaviour.html) is set to
inactive. You can exclude certain types from this rule (i.e., allowing them to
be inactive in the scene or prefab) by specifying an `Exclusions` array in the
configuration. See [config](# Configuration Options) for more details.

#### `NoMissingComponents`

This rule ensures that all script components are not referencing missing
scripts.  If your object has a component with a missing script reference,
something has likely gone wrong!

#### `NoMissingObjectReferences`

This rule outputs a warning if you have a
[`MonoBehaviour`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)
with unassigned `public` or `SerializeField` fields - i.e. those with `null`
values. This can be a smell in that your script either has some unused code
in it or it may be that your `MonoBehaviour` is overly general.

### Writing Your Own Rules

In order to write your own rule, you need to implement the following classes:

1. A class that implements `Rule<T>`, where `T` is the type of component you'd
   like to validate (otherwise, `T` must be `GameObject`). Take a look at the
   `Rule` file to see which abstract methods must be implemented.

2. A class that implements `IViolation`. This is the type of violation that
   your rule generates. Take a look at the interface in this repository to see
   which methods it must implement.

After you have implemented the class, add an entry to `moobleconfig.json` for
that class. The class name must be fully namespaced. You must also provide the
assembly name which you can find out by running the following code locally:

```csharp
using System;
using System.Reflection;

public class FindAssembly : MonoBehaviour {
  private void Start() {
    // SomeClass is a class in your project
    Debug.Log(typeof(SomeClass).Assembly.GetName().Name);
  }
}
```

In most cases it will be `"Assembly-CSharp"`.

### Configuration Options

Add a `moobleconfig.json` file to your root project directory. This is the file
that the menu items look for when trying to determine what rules you want to
run over your scenes and prefabs. Take a look at the `moobleconfig.json` file
provided in this repository for an example.

Only rules listed in the `Rules` list will be run.

#### Exclusions

The `Exclusions` list is to allow users to provide various types that the
rule in question should _not_ run on. This type name must be qualified
with the full namespace and the assembly it belongs to. For example, if you
don't want to run the `NoInactiveBehaviours` rule on `Animator` behaviours,
add it to the exclusion list:

```json
"Exclusions": [ "UnityEngine.Animator, UnityEngine" ]
```

Other common assemblies include:
* `Assembly-CSharp` for code in your project
* `Assembly-CSharp-firstpass` for code in your plugins directory

Not all rules accept `Exclusions` lists; take a look at the code for the
specific rule to check whether it accepts exclusions.

### Build Integration

You can integrate Mooble with your Jenkins build by using the command line tools provided (see [CLI.cs](https://github.com/uken/mooble/blob/master/Assets/Plugins/Mooble/StaticAnalysis/CLI.cs)). You can run the command line static analysis tools by using Unity's batch mode. Make sure to close any open instances of Unity before you run the following:

```bash
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath ${WORKSPACE} -executeMethod Mooble.StaticAnalysis.CLI.RunPrefabAnalysis ${MOOBLE_PREFABS}
```

`${WORKSPACE}` is the location of the project that has integrated the Mooble plugin, and `${MOOBLE_PREFABS}` in this case is a space-delimited list of prefabs you want to run the prefab analysis on, e.g `Assets/Prefabs/A.prefab Assets/Prefabs/B.prefab Assets/Prefabs/MorePrefabs/C.prefab`.
