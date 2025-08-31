# .NET 10 Support

This file documents the .NET 10 support in the Ketupa Predicates library.

## Current Status
.NET 10 support is **enabled by default** in all projects. The library now targets .NET 10.0 (Preview) alongside other supported frameworks.

## Build Requirements

### Prerequisites
1. .NET 10 SDK (Preview 7 or later)
2. Development environment that supports .NET 10

### Standard Build Commands

All standard .NET commands now include .NET 10 support:

#### Build all projects:
```bash
dotnet build
```

#### Run tests:
```bash
dotnet test
```

#### Restore packages:
```bash
dotnet restore
```

### What Changes When .NET 10 is Enabled

1. **Main Library** (`src/KetupaPredicates/KetupaPredicates.csproj`):
   - Adds `net10.0` to target frameworks
   - Uses C# 12.0 language version for .NET 10
   - Enables nullable reference types

2. **CLI Tool** (`src/KetupaPredicatesCli/KetupaPredicatesCli.csproj`):
   - Adds `net10.0` to target frameworks

3. **WPF Tester** (`src/KetupaPredicatesTester/KetupaPredicatesTester.csproj`):
   - Adds `net10.0-windows` to target frameworks

4. **Test Project**:
   - Includes the `KetupaPredicates.Tests.MSTest10.0` project in builds

### Making .NET 10 Default (Future)

Once .NET 10 is stable and you want to make it the default, update each project file to:
1. Move `net10.0` to the default `TargetFrameworks` property
2. Remove the conditional `IncludeNet10` properties
3. Add the test project back to the solution file

### Conditional Compilation

The code already uses `NET5_0_OR_GREATER` conditional compilation symbols which will automatically work with .NET 10, providing:
- Switch expressions (C# 8.0+ feature)
- Nullable reference types 
- Modern C# language features

No additional conditional compilation changes are needed.