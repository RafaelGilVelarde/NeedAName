using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class BattleStart : Node2D
{
    
    [Export]BattleScene scene;
    [Export] Array<Character> EnemyCharacters;
    [Export] int CharacterPrefab;
    [Export] Vector2 BorderOffset;
    [Export]Array<Vector2>PartyPos,EnemyPos;

    public void Constructor(Array<Vector2> Party, Array<Vector2> Enemy, int Prefab, Array<Character> Characters, BattleScene Battle){
        PartyPos = Party;
        EnemyPos = Enemy;
        CharacterPrefab = Prefab;
        EnemyCharacters.Clear();
        int Index = 0;
        if(EnemyPos.Count>1){
            while(EnemyCharacters.Count<EnemyPos.Count-1){
                //int Aux=Index%Characters.Count;
                RandomNumberGenerator RNG = new RandomNumberGenerator();
                int chance=RNG.RandiRange(0,Characters.Count-1);
                //if(chance == Index%Characters.Count){
                    EnemyCharacters.Add((Character)Characters[chance].Duplicate(true));
                //}
                Index++;
            }
        }
        scene = Battle;
    }
    public virtual void StartBattle(){
        if(BattleManager.instance.Scene==null){
            Array<PlayerController> Characters=GameManager.Instance.Characters;
            Array<BattleCharacter> Party=new Array<BattleCharacter>();
            Array<BattleCharacter> EnemyBattle=new Array<BattleCharacter>(); 
            for(int i=0;i<Characters.Count;i++){
                Character aux= Characters[i].BattleCharacter.Character;
                if (aux.Active && aux.status!=Character.Status.KO)
                {
                    Party.Add(Characters[i].BattleCharacter);
                    if (scene.Horizontal)
                    {
                        Party[Party.Count - 1].BattleOffset = Vector2.Zero;
                    }
                    else
                    {
                        Party[Party.Count - 1].BattleOffset = Party[Party.Count - 1].Character.Base.BattleOffset;
                    }
                }
            }

            EnemyBattle.Add(this.GetParent<OverworldController>().BattleCharacter);
            if (scene.Horizontal)
            {
                EnemyBattle[0].BattleOffset = Vector2.Zero;
            }
            else
            {
                EnemyBattle[0].BattleOffset = EnemyBattle[0].Character.Base.BattleOffset;
            }
            for (int i = 0; i < EnemyCharacters.Count; i++)
            {
                EnemyBattle.Add(GameManager.Instance.AddCharacters((Character)EnemyCharacters[i].Duplicate(true), CharacterPrefab).BattleCharacter);
                EnemyBattle[EnemyBattle.Count - 1].GetParent<Node2D>().Position = GlobalPosition;
                if (scene.Horizontal)
                {
                    EnemyBattle[EnemyBattle.Count - 1].BattleOffset = Vector2.Zero;
                }
                else
                {
                    EnemyBattle[EnemyBattle.Count - 1].BattleOffset = EnemyBattle[EnemyBattle.Count - 1].Character.Base.BattleOffset;
                }
            }
            for(int i=0;i<EnemyBattle.Count;i++){
                ((EnemyCharacter)EnemyBattle[i].Character).AI = (AISettings)((EnemyCharacterBase)EnemyBattle[i].Character.Base).AI.Duplicate();
                EnemyBattle[i].Character.SetStats();
                EnemyBattle[i].Character.ResetCharacter();
                EnemyBattle[i].Overworld.BattleStart();
            }


            Array<Vector2> PartyPosGlobal=new Array<Vector2>();
            Array<Vector2> EnemyPosGlobal=new Array<Vector2>();
            for (int i=0;i<Party.Count;i++){
                Vector2 AuxPos = GlobalPosition + PartyPos[i] - Party[i].BattleOffset;
                if (scene.Horizontal)
                {
                    AuxPos = new Vector2(AuxPos.X, scene.PartyFloorY[i] - Party[i].FloorNode.Position.Y);
                }
                Vector2 Aux=CheckPosition(AuxPos);
                PartyPosGlobal.Add(Aux);
            }
            for (int i=0;i<EnemyBattle.Count;i++){
                Debug.WriteLine("ThisPos: "+GlobalPosition);
                Vector2 AuxPos = GlobalPosition + EnemyPos[i] - EnemyBattle[i].BattleOffset;
                if (scene.Horizontal)
                {
                    AuxPos = new Vector2(AuxPos.X, scene.EnemyFloorY[i] - EnemyBattle[i].FloorNode.Position.Y);
                }
                Vector2 Aux=CheckPosition(AuxPos);
                Debug.WriteLine("FinalPos: "+Aux);
                EnemyPosGlobal.Add(Aux);
            }
            BattleManager.instance.ProcessMode=ProcessModeEnum.Inherit;
            BattleManager.instance.StartBattle(scene,Party,EnemyBattle,PartyPosGlobal,EnemyPosGlobal,EnemyBattle[0].GlobalPosition,EnemyBattle[0].GlobalPosition,this);
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
                    if(!aux.IsInGroup("EnemyOverworldController")&&!aux.IsInGroup("PlayerOverworldController")&&!aux.IsInGroup("IgnoreOnStartBattle")){
                        Debug.WriteLine("Collider: "+aux);
                        FinalPos=result["position"].As<Vector2>();
                        if(!aux.IsInGroup("Borders")){
                            Vector2 Normalized = new Vector2(FinalPos.X/Mathf.Abs(FinalPos.X),FinalPos.Y/Mathf.Abs(FinalPos.Y));
                            FinalPos-=Normalized*BorderOffset;
                        }
                }
            }
            return FinalPos;
    }
}
