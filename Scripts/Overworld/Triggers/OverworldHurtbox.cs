using Godot;
using System;

public partial class OverworldHurtbox : Area2D
{
    [Signal]
	public delegate void _HurtEventHandler(Node2D node);
}
