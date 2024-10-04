using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BattleStart : Node2D
{
    
    [Export]BattleScene scene;
    [Export]Array<Character> EnemyCharacters;
    [Export] int CharacterPrefab;
    [Export]Array<Vector2>PartyPos,EnemyPos;

    public void Constructor(Array<Vector2> Party, Array<Vector2> Enemy, int Prefab, Array<Character> Characters, BattleScene Battle){
        PartyPos = Party;
        EnemyPos = Enemy;
        CharacterPrefab = Prefab;
        EnemyCharacters.Clear();
        int Index = 0;
        if(EnemyPos.Count>0){
            while(EnemyCharacters.Count<EnemyPos.Count-1){
                int Aux=Index%Characters.Count;
                RandomNumberGenerator RNG = new RandomNumberGenerator();
                int chance=RNG.RandiRange(0,Characters.Count-1);
                if(chance == Index%Characters.Count){
                    EnemyCharacters.Add((Character)Characters[Aux].Duplicate(true));
                }
                Index++;
            }
        }
        scene = Battle;
    }
    public virtual void StartBattle(){
        if(BattleManager.instance.Scene==null){
            Array<OverworldController> Characters=GameManager.Instance.Characters;
            Array<BattleCharacter> Party=new Array<BattleCharacter>();
            Array<BattleCharacter> EnemyBattle=new Array<BattleCharacter>(); 
            for(int i=0;i<Characters.Count;i++){
                Character aux= Characters[i].BattleCharacter.Character;
                if(aux.Active){
                    Party.Add(Characters[i].BattleCharacter);
                }
            }
            EnemyBattle.Add(this.GetParent<OverworldController>().BattleCharacter);
            for(int i=0;i<EnemyCharacters.Count;i++){
                EnemyBattle.Add(GameManager.Instance.AddCharacters(EnemyCharacters[i],CharacterPrefab).BattleCharacter);
                EnemyBattle[EnemyBattle.Count-1].GetParent<Node2D>().Position=GlobalPosition;
            }
            for(int i=0;i<EnemyBattle.Count;i++){
                EnemyBattle[i].Overworld.BattleStart();
                EnemyBattle[i].Character.SetStats();
            }
            Array<Vector2> PartyPosGlobal=new Array<Vector2>();
            Array<Vector2> EnemyPosGlobal=new Array<Vector2>();
            for (int i=0;i<PartyPos.Count;i++){
                Vector2 Aux=CheckPosition(GlobalPosition+PartyPos[i]);
                PartyPosGlobal.Add(Aux);
            }
            for (int i=0;i<EnemyPos.Count;i++){
                Vector2 Aux=CheckPosition(GlobalPosition+EnemyPos[i]);
                EnemyPosGlobal.Add(Aux);
            }
            BattleManager.instance.ProcessMode=ProcessModeEnum.Inherit;
            BattleManager.instance.StartBattle(scene,Party,EnemyBattle,PartyPosGlobal,EnemyPosGlobal,EnemyBattle[0].GlobalPosition,EnemyBattle[0].GlobalPosition);
            GameManager.Instance.BattleStart();
        }
    }
    public Vector2 CheckPosition(Vector2 Pos){
            Vector2 FinalPos=Pos;

            CharacterBody2D character=GetParent<EnemyOverworldController>().Parent;
            var spaceState = GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, Pos,character.CollisionMask);
            query.Exclude=new Godot.Collections.Array<Rid> {character.GetRid(),GetParent<EnemyOverworldController>().CurrentTarget.GetNode<CollisionObject2D>(".").GetRid()};
            var result = spaceState.IntersectRay(query);
            if(result.Count>0){
                    Node2D aux=result["collider"].As<Node2D>();
                    if(!aux.IsInGroup("EnemyOverworldController")&&!aux.IsInGroup("PlayerOverworldController")){
                        FinalPos=result["position"].As<Vector2>();
                }
            }
            return FinalPos;
    }
}
