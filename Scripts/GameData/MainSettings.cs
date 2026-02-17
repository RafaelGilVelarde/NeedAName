using Godot;
using System;

[GlobalClass]
public partial class MainSettings : Resource
{
    [Export] public int CurrentSave;
    [Export] public string DisplayName, Language = "en";
}
