using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

[GlobalClass]
public partial class Character : Resource
{
    [Export]public CharacterBase Base {get; private set;}
    [Export]public Array<Moves> Moves;
    [Export]public Array<Items> items;
    [Export]public Array<Equipment> Equipment;

    [Export] public Stats stats;
    [Export]public bool isControlledByPlayer;
    [Export]public bool Active=true;


    [Signal]
	public delegate void _GetHitEventHandler();
    [Signal]
	public delegate void _DieEventHandler();

    public enum Status{
        Normal,
        KO,
    };
    [Export]public Status status;
    public virtual void DamageCalc(BattleCharacter Character,BattleCharacter TargetCharacter, Moves move){
        /*for (int i=0;i<Equipment.Count;i++){
            Equipment[i].Base.GetHitEffect(TargetCharacter,move);
        }
        switch (move.Base.type){
            case MoveBase.Type.Physical:
                ChangeHP(Mathf.RoundToInt(Character.Character.stats.Atk*move.Base.Power*Character.MultiplyAtk()/stats.Def*TargetCharacter.MultiplyDef())*-1);
                EmitSignal("_GetHit");
            break;
            case MoveBase.Type.Special:
                ChangeHP(Mathf.RoundToInt(Character.Character.stats.SpAtk*move.Base.Power*Character.MultiplySpAtk()/stats.SpDef*TargetCharacter.MultiplySpDef())*-1);
                EmitSignal("_GetHit");
            break;
            case MoveBase.Type.Recovery:
                ChangeHP(Mathf.RoundToInt(stats.SpAtk*move.Base.Power*Character.MultiplySpAtk()));
            break;
        }*/
    }
    public virtual void ChangeHP(int hp,Node2D character){
        stats.HP+=hp;

        Node2D HPLabelParent=GameManager.Instance.TextEffectPrefabs[0].Instantiate<Node2D>();
        HPLabelParent.Scale=character.GlobalScale;
        HPLabelParent.Rotation=character.GlobalRotation;
        RichTextLabel HPLabel=HPLabelParent.GetChild<RichTextLabel>(0);
        HPLabel.Text="[center]"+hp.ToString()+"[/center]";
        HPLabel.AddThemeColorOverride("default_color",Base.TextEffectColor);
        Debug.WriteLine("Color: "+Base.TextEffectColor);
        character.AddChild(HPLabelParent);
        //HPLabel.SetGlobalPosition(character.GlobalPosition);
        if(hp<0){
            EmitSignal("_GetHit");
        }
        else if (hp==0){
            HPLabel.Text="[center]BLOCKED[/center]";
        }
        if(stats.HP<=0){
            Debug.WriteLine("Dying");
            stats.HP=0;
            status=Status.KO;
            EmitSignal("_Die");
        }
    }
}
