using Godot;
using Godot.Collections;
using System;

public partial class EnemySpawner : Node2D
{
    [Export] Array<Character> MainEnemies, SideEnemies; 
    [Export] int Prefab;
    [Export] Array<Vector2> PartyPos, EnemyPos;
    [Export] BattleScene Scene;
    [Export] DetectArea detectArea;
    public override void _Ready()
    {
        Callable.From(ActorSetup).CallDeferred();	


    }
    
	private async void ActorSetup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int Index=RNG.RandiRange(0,MainEnemies.Count-1);        
        EnemyOverworldController Overworld = (EnemyOverworldController)GameManager.Instance.AddCharacters(MainEnemies[Index],1);
        Overworld.Parent.Position=GlobalPosition;
        Overworld.CachedTargetPosition=GlobalPosition;
        Overworld.SetArea(detectArea);
        Overworld.Battle.Constructor(PartyPos,EnemyPos,Prefab,SideEnemies,Scene);
    }

}
