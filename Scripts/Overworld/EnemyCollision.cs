using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class EnemyCollision : Area2D
{
    [Export] BattleStart battleStart;
    public override void _Ready()
    {
        BodyEntered+=OnCollisionEntered;
    }

    private void OnCollisionEntered(Node2D body)
    {
        Debug.WriteLine(body);
        Debug.WriteLine(body.GetGroups());
        if(body.IsInGroup("PlayerOverworldController")){
            battleStart.CallDeferred("StartBattle");
        }
    }


    /*BattleCharacter AddCharacters(Character Enemy){
            OverworldController Overworld=new OverworldController();
            Node Prefab=CharacterPrefab.Instantiate<Node>();
            GetTree().CurrentScene.AddChild(Prefab);
            Overworld=Prefab.GetNode<OverworldController>("./OverworldController");
            Overworld.BattleCharacter.Character=Enemy;

            AnimationPlayer OverworldAnimator=Enemy.Base.OverworldAnimator.Instantiate<AnimationPlayer>();
            AnimationPlayer BattleAnimator=Enemy.Base.BattleAnimator.Instantiate<AnimationPlayer>();

            Overworld.AddChild(OverworldAnimator);
            Overworld.AnimatorTree=OverworldAnimator.GetChild<AnimationTree>(0);
            Overworld.BattleCharacter.AddChild(BattleAnimator);
            Overworld.BattleCharacter.AnimatorTree=BattleAnimator.GetChild<AnimationTree>(0);


            return Overworld.BattleCharacter;
    }*/
}
