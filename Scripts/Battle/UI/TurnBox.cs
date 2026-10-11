using Godot;
using System;

public partial class TurnBox : Control
{
    [Export] public AnimationPlayer Animator;
    [Export] public BattleCharacter CurrentCharacter;
    [Export] public TextureRect CharacterSprite;
    [Export] public RichTextLabel ActiveTime;
}
