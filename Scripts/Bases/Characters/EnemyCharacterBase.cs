using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
[GlobalClass]
public partial class EnemyCharacterBase : CharacterBase
{
    [Export] public float ExpYield;
    [Export] public AISettings AI;    
}
