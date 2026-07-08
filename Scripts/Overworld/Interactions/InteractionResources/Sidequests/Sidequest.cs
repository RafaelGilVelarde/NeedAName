using Godot;
using System;

[GlobalClass]
public partial class Sidequest : Resource
{
    [Export] int IndexBoolStarted, IndexBoolFinished;
    [Export] string Title, Description;
}
