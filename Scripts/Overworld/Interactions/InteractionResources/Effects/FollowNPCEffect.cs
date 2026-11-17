using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class FollowNPCEffect : InteractEffect
{
    Callable FollowNPC;
    //[Export] PlayerController Follower;
    [Export] CharacterType character;
    [Export] Array<CharacterType> CheckDeleteCharacters;
    [Export] int CharacterPrefab;
    [Export] Array<string> CheckKey,CheckUnflagKey;

    [Export] string KeyIndex;

   /* public override void _Ready()
    {
        base._Ready();
        FollowNPC = new Callable(this,MethodName.Follow);
        //Animator = Follower.Animator;
        //AnimatorTree = Follower.AnimatorTree;
        Flags flags= GameManager.Instance.Data.Flags;
        for(int i = 0;i<CheckKey.Count;i++){
            if((bool)flags.Dict[CheckKey[i]])
            {
                GetParent().QueueFree();
                break;
            }
        }

    }
    public override void ConnectCall()
    {
        StartInteract = new Callable(this,MethodName.Follow);
        base.ConnectCall();
    }

    void Follow(string argument){
        GameManager Game = GameManager.Instance;
        if(argument == "Follow"){
            AddFollow();
        }
        if(argument == "Delete"){
            Flags Flag = GameManager.Instance.Data.Flags;
            for(int i = 0;i<CheckDeleteCharacters.Count;i++){
                Character AuxChar = new Character();
                CharacterType AuxType = CheckDeleteCharacters[i]; 
                if(AuxType.Party){
                    AuxChar = Game.Data.Party[AuxType.CharacterIndex];
                    }
                else{
                    AuxChar = Game.Data.AllFollwers[AuxType.CharacterIndex];
                }
                Flag.ChangeDictFlag(CheckUnflagKey[i%CheckUnflagKey.Count],false);
                for(int j = 0;j<Game.Followers.Count;j++){
                    PlayerController Follower = Game.Followers[j];
                    Character Chara = Follower.BattleCharacter.Character;
                    if(AuxChar == Chara && Chara.Active){
                        if(AuxType.Party){
                            Game.RemoveFollowingCharacter(Follower,false);
                        }
                        else{
                            Game.RemoveFollowingCharacter(Follower,true);
                        }
                    }

                }
            }
            AddFollow();
        }
    }

    void AddFollow(){
            GameManager Game = GameManager.Instance;
            PlayerController Aux;
            Character AuxChar = new Character();
            if(character.Party){
                AuxChar = Game.Data.Party[character.CharacterIndex];
            }
            else{
                AuxChar = Game.Data.AllFollwers[character.CharacterIndex];
            }
            Aux = (PlayerController)Game.AddCharacters(AuxChar,CharacterPrefab);
            Aux.Parent.GlobalPosition = GetParent<Node2D>().GlobalPosition;
            switch (CharacterPrefab){
                case 0:
                    GameManager.Instance.AddFollowingCharacter(Aux,false);
                break;
                case 2:
                    GameManager.Instance.AddFollowingCharacter(Aux,true);
                break;
            }
            Game.Data.Flags.ChangeDictFlag(KeyIndex,true);
            GetParent().QueueFree();
    }*/
}
