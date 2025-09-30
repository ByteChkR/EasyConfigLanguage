# ECL Language Documentation

## Overview

Easy Config Language (ECL) is a simple and intuitive configuration language designed to facilitate the setup and management of applications.

## Syntax

ECL uses a straightforward syntax that is easy to read and write. Here are some of the key elements:
- **Comments**: Use `//` for single-line comments and `/* ... */` for multi-line comments.
- **Key-Value Pairs**: Define settings using `key = value` format.
- **Objects**: Group related settings using curly braces `{}`.
- **Arrays**: Define lists using square brackets `[]`.
- **Data Types**: Supports strings, numbers, booleans, arrays, and objects.
- **Nested Key-Value Pairs**: You can nest objects within objects for hierarchical configurations by using `myObject.nestedKey = value` syntax.
- **Simple Built-In Functions**: Merge object with `@merge(obj1, obj2, ...)` and include external files with `@include("path/to/**.ecl")`.

## Simple Object Example
```hcl
{
    enabled = true
    name = "simple-object"
    description = "A simple object example"
    version = "1.0.0"
}
```
## Simple Implicit Object Example
```hcl
enabled = true
name = "simple-object"
description = "A simple object example"
version = "1.0.0"
```

## Nested Object Example
```hcl
module = {
    enabled = true
    name = "simple-object"
    meta = {
        description = "A simple object example"
        version = "1.0.0"
    }
}
```

## Implicit Nested Object Example
```hcl
module.enabled = true
module.name = "simple-object"
module.meta.description = "A simple object example"
module.meta["version"] = "1.0.0"
```