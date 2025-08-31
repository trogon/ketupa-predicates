# .NET 10 Support Instructions

This file documents how to enable .NET 10 support once .NET 10 is released.

## Current Status
The repository has been prepared for .NET 10 support with conditional MSBuild properties. All target frameworks are conditionally set based on the `IncludeNet10` property.

## Enabling .NET 10 Support

### Prerequisites
1. Install .NET 10 SDK when it becomes available
2. Ensure your development environment supports .NET 10

### Build Commands

#### Enable .NET 10 for all projects:
```bash
dotnet build -p:IncludeNet10=true
```

#### Test with .NET 10:
```bash
dotnet test -p:IncludeNet10=true
```

#### Restore packages for .NET 10:
```bash
dotnet restore -p:IncludeNet10=true
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