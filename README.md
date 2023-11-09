# Mitochondria

An Among Us mod API.

> This mod is not affiliated with Among Us or Innersloth LLC, and the content
> contained therein is not endorsed or otherwise sponsored by Innersloth LLC.
> Portions of the materials contained herein are property of Innersloth LLC.
> © Innersloth LLC.

## All Modules
- [Mitochondria.Core](README.md)
    <p>The core functionality and abstractions of Mitochondria</p>
- [Mitochondria.ExtensionPack](Mitochondria.ExtensionPack/README.md)
    <p>An extension pack including extra functionality like tooltips</p>
- [Mitochondria.Options.Settings](Mitochondria.Options.Settings/README.md)
    <p>Custom settings menu options</p>

## Usage

### Binding

Used by inheriting `Binding`, adding the `[Binding]` attribute and calling
`BindingManager#TryBind()`.

Allows for binding 2 objects together with the use of
`BindingExtensions.Equalize()` in the update method of a binding to
propagate changes from one object to another.

### Configuration

Used by inheriting `IConfig` and then calling `PluginConfig#Load()` and
`IStorageHandle#Save()`, with a `PluginConfig` being obtained via
`Config#Of()`, `[PluginInfo]#GetConfig()` or `Config<TPlugin>.Instance` (where
`TPlugin` inherits `BasePlugin`).

Allows you to have multiple configuration files for your plugin.

### GUI

#### Custom action buttons

Used by inheriting `CustomActionButton` and adding it to a container such as
`CustomHudManager#MainActionButtonsContainer`.

Allows for creating custom action buttons.

#### Manipulating existing GUI elements

Used by creating elements or containers using `[GameObject]#SetGuiElement()` or
`[GameObject]#SetGuiContainer()` and then using `[GameObject]#GetGuiElement()`
or `[GameObject]#GetGuiContainer()` elsewhere. Elements and containers can
implement various interfaces, such as `IOrderableContainer`, to provide
functionality.

Allows for creating abstractions to manipulate other existing GUI elements.

### Modifiers

Used by inheriting various modifiers, such as `GameplayModifier`, and calling `ModifierManager#Add()` and
`ModifierManager#Remove()`.

Provides convenient ways to modify gameplay.

### Overrides

Used by creating various overrides, such as `IntroCutsceneTeamOverride`, and
calling `OverrideManager#Add()` and `OverrideManager#Remove()`.

Allows for overriding specific things in the game.

### Prototypes

Used by calling `PrototypeManager#CloneAndSet()` and
`PrototypeManager#TryCloneAndGet()`.

Allows for saving game objects as templates so that they can be created
elsewhere.

### Resources

#### Providers

Used by creating a provider and calling `ResourceProvider#Load()`.

Allows for creating various resource types using various methods, such as a
sprite from an embedded resource.

### Roles

Used by inheriting `CustomRole` and adding the `[CustomRole]` attribute.

Allows for the definition of custom roles.

### Services

Used by inheriting `IService` and adding the `[Service]` attribute.

Provides convenient hooks.