using Godot;
using System;

public partial class TextPrompt : Node2D
{
    [Export] public RichTextLabel Text;
    [Export] public AnimationPlayer Animator;
}
