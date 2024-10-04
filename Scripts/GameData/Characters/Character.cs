using Godot;
using Godot.Collections;
using System.Diagnostics;


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

    }
    public virtual void ChangeHP(int hp,Node2D character){
        stats.HP+=hp;

        Node2D HPLabelParent=GameManager.Instance.TextEffectPrefabs[0].Instantiate<Node2D>();
        HPLabelParent.Scale=character.GlobalScale;
        HPLabelParent.Rotation=character.GlobalRotation;
        RichTextLabel HPLabel=HPLabelParent.GetChild<RichTextLabel>(0);
        HPLabel.Text="[center]"+hp.ToString()+"[/center]";
        HPLabel.AddThemeColorOverride("default_color",Base.TextEffectColor);
        Debug.WriteLine("HP Loss: "+hp);
        character.AddChild(HPLabelParent);
        //HPLabel.SetGlobalPosition(character.GlobalPosition);
        if(hp<0){
            EmitSignal("_GetHit");
        }
        else if (hp==0){
            HPLabel.Text="[center]BLOCKED[/center]";
        }
        if(stats.HP<=0){
            stats.HP=0;
            status=Status.KO;
            EmitSignal("_Die");
        }
    }
    public virtual void SetStats(){
        Stats BaseStats = Base.BaseStats;
        stats.MaxHP= (int)(BaseStats.MaxHP*Mathf.Log(2* stats.Lv));
        stats.Atk= (int)(BaseStats.Atk*Mathf.Log(2* stats.Lv));
        stats.Def= (int)(BaseStats.Def*Mathf.Log(2* stats.Lv));
        stats.SpAtk= (int)(BaseStats.SpAtk*Mathf.Log(2* stats.Lv));
        stats.SpDef= (int)(BaseStats.SpDef*Mathf.Log(2* stats.Lv));
        stats.Speed= (int)(BaseStats.Speed*Mathf.Log(2* stats.Lv));
        Debug.WriteLine("Max HP: "+stats.MaxHP+ " Atk: "+stats.Atk+" Def: "+stats.Def);
    }
}
