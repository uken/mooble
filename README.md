# Mooble

## Static Analysis

Mooble provides tools to perform static analysis on prefabs and scene files -
often unchecked in code review and hard to test without someone running the
scene and going through every possible flow. Mooble's goal is to help catch
errors ahead of time by checking for common errors in scene and prefab files,
and allowing you to customize the static analysis by writing your own rules.

### Built-in Rules

#### NoDuplicateComponents

This rule makes sure that GameObjects do not have two of the same type of
Component, e.g.  no GameObject has two Text components, etc.

#### NoInactiveBehaviours

This rule ensures that no
[Behaviour](https://docs.unity3d.com/ScriptReference/Behaviour.html) is set to
inactive. You can exclude certain types from this rule (i.e., allowing them to
be inactive in the scene or prefab) by specifying an `Exclusions` array in the
configuration. See [config](# Configuration Options) for more details.

#### NoMissingComponents

This rule ensures that all script components are not referencing missing
scripts.  If your object has a component with a missing script reference,
something has likely gone wrong!

#### NoMissingObjectReferences

This rule outputs a warning if you have a
[MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)
with unassigned `public` or `SerializeField` fields - i.e. those with `null`
values. This can be a smell in that your script either has some unused code
in it or it may be that your `MonoBehaviour` is overly general.

### Writing Your Own Rules

This feature is currently in progress.

### Configuration Options

Add a `moobleconfig.json` file to your root project directory. This is the file
that the menu items look for when trying to determine what rules you want to
run over your scenes and prefabs. Take a look at the `moobleconfig.json` file
provided in this repository for an example.

Any rule that is excluded from the list will not be run. Not all rules accept
`Exclusions` lists; take a look at the code for the specific rule to check
whether it accepts exclusions.

### Build Integration

This feature is currently in progress.
