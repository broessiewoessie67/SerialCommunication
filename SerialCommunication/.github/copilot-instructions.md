# Copilot instructions for SerialCommunication

## Build
- Visual Studio: Open the solution/project (SerialCommunication.csproj) in Visual Studio 2017 or later and build (F6) or run (F5).
- MSBuild (command line):
  - Debug: msbuild SerialCommunication.csproj /p:Configuration=Debug
  - Release: msbuild SerialCommunication.csproj /p:Configuration=Release
- Built output: bin\Debug\SerialCommunication.exe or bin\Release\SerialCommunication.exe

Note: This is a .NET Framework 4.7.2 WinForms project (ToolsVersion 15.0). Use the Visual Studio Developer Command Prompt or MSBuild from an environment that has the .NET Framework targeting packs installed.

## Test and Lint
- There are no unit tests or linting configured in this repository.
- How to run a single test: Not applicable; add a test project (NUnit/xUnit/MSTest) if you need tests and reference the project.

## Run
- From Visual Studio: Run with F5.
- From the file system: execute bin\Debug\SerialCommunication.exe after building.

## High-level architecture
- Single-project Windows Forms application targeting .NET Framework 4.7.2.
- Entry point: Program.Main (Program.cs) which calls Application.Run(new Form1()).
- UI surface: Form1 is the main form, implemented as a partial class split across Form1.cs (logic / event handlers) and Form1.Designer.cs (generated UI layout and InitializeComponent).
- Resources: Images and other UI assets live in the Resources\ folder and are embedded as project resources (Properties.Resources / Resources.resx).
- Serial I/O: Uses System.IO.Ports.SerialPort to enumerate and (presumably) communicate with serial ports. Form1.Load and related event handlers handle port enumeration and UI wiring.

## Key conventions and patterns
- UI/logic split: Keep layout in Form1.Designer.cs and behavior in Form1.cs. Do not edit the Designer file except through the Windows Forms Designer.
- Resource usage: Images are included in Resources and referenced from the designer. Prefer Properties.Resources for programmatic access.
- Naming: Some control names use Dutch terms (e.g., comboBoxPoort). Maintain existing naming when extending to avoid confusion.
- Error handling: Current code swallows exceptions in some UI handlers (empty catch blocks). When adding functionality, prefer logging or at least leaving TODOs to handle errors explicitly.
- Project file: SerialCommunication.csproj is an MSBuild-style csproj (not SDK-style). Use MSBuild or Visual Studio to build reliably.

## Places to look first when modifying behavior
- Form1.cs: event handlers and serial port logic.
- Form1.Designer.cs: UI element names and layout.
- Resources\*: images used by the UI.
- SerialCommunication.csproj: references, target framework, and build configuration.

## Notes for Copilot / automated assistants
- Focus on Form1 for feature changes since this is the single UI entry point.
- Preserve Designer-generated code; prefer adding helpers or partial-class methods in Form1.cs.
- Avoid adding SDK-style project changes unless migrating the project and updating contributors; that is a breaking change for current tooling expectations.

---

If you want this file expanded with suggested tests, linting setup, or a recommended project migration plan, say which area to cover next.
