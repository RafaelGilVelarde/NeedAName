using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class MainSettings : Resource
{
    [Export] public int CurrentSave;
    [Export] public string DisplayName, Language = "en";
    [Export] public Array<double> Volume;
}
