using Godot;
using Godot.Collections;
using System.Diagnostics;


[GlobalClass]
public partial class Character : Resource
{
    public Node2D NodeCharacter;
    [Export]public CharacterBase Base {get; private set;}
    [Export]public Array<Moves> Moves;
    [Export]public Array<Items> items;
    [Export]public Array<EquipmentBase> Equipment;

    [Export] public Stats stats, EquipStats = new Stats(), TotalStats = new Stats();
    [Export]public bool isControlledByPlayer;
    [Export]public bool Active=true;
    [Export] public Key Key {get; private set;}


    [Signal]
	public delegate void _GetHitEventHandler();
    [Signal]
	public delegate void _ChangeHPEventHandler(int HP);
    [Signal]
	public delegate void _DieEventHandler();

    public enum Status{
        Normal,
        KO,
    };
    [Export]public Status status;
    public virtual void DamageCalc(BattleCharacter Character,BattleCharacter TargetCharacter, Moves move){

    }
    public virtual void ChangeHP(int hp){
        stats.HP+=hp;
        stats.HP=Mathf.Clamp(stats.HP,0,TotalStats.MaxHP);

        Node2D HPLabelParent=GameManager.Instance.TextEffectPrefabs[0].Instantiate<Node2D>();
        HPLabelParent.Scale=NodeCharacter.GlobalScale;
        HPLabelParent.Rotation=NodeCharacter.GlobalRotation;
        RichTextLabel HPLabel=HPLabelParent.GetChild<RichTextLabel>(0);
        HPLabel.Text="[center]"+hp.ToString()+"[/center]";
        HPLabel.AddThemeColorOverride("default_color",Base.TextEffectColor);
        NodeCharacter.AddChild(HPLabelParent);
        EmitSignal("_ChangeHP",hp);
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
        /*Stats BaseStats = Base.BaseStats;
        stats.MaxHP= (int)(BaseStats.MaxHP*Mathf.Log(2* stats.Lv));
        stats.Atk= (int)(BaseStats.Atk*Mathf.Log(2* stats.Lv));
        stats.Def= (int)(BaseStats.Def*Mathf.Log(2* stats.Lv));
        stats.SpAtk= (int)(BaseStats.SpAtk*Mathf.Log(2* stats.Lv));
        stats.SpDef= (int)(BaseStats.SpDef*Mathf.Log(2* stats.Lv));
        stats.Speed= (int)(BaseStats.Speed*Mathf.Log(2* stats.Lv));
        Debug.WriteLine("Max HP: "+stats.MaxHP+ " Atk: "+stats.Atk+" Def: "+stats.Def);
        SetTotalStats();*/
    }
    public void SetTotalStats(){
        TotalStats.MaxHP = stats.MaxHP + EquipStats.MaxHP;
        TotalStats.Atk = stats.Atk + EquipStats.Atk;
        TotalStats.Def = stats.Def + EquipStats.Def;
        TotalStats.SpAtk = stats.SpAtk + EquipStats.SpAtk;
        TotalStats.SpDef = stats.SpDef + EquipStats.SpDef;
        TotalStats.Speed = stats.Speed + EquipStats.Speed;
    }
    public void ChangeKey(InputEventKey newKey){
        PartyCharacterBase aux=(PartyCharacterBase)Base;
        if(InputMap.HasAction("SelectedKey"+aux.PartyId)){
            InputMap.ActionEraseEvents("SelectedKey"+aux.PartyId);
            InputMap.ActionAddEvent("SelectedKey"+aux.PartyId,newKey);
        }
        else{
            InputMap.AddAction("SelectedKey"+aux.PartyId);
            InputMap.ActionAddEvent("SelectedKey"+aux.PartyId,newKey);            
        }
        Key = newKey.Keycode;
    }

    public virtual void Equip(Equipment equipment){
        EquipmentBase equipmentBase = (EquipmentBase)equipment.Base;
        Equipment[(int)equipmentBase.EquipType] = equipmentBase;
        equipment.Base.Effect(new Array<Character>{this});
        SetTotalStats();
    }
    public virtual void UnEquip(EquipmentType type){
        EquipmentBase Aux = Equipment[(int)type];
        Array<Items> GameItems = GameManager.Instance.Data.items[1].items;
        bool ExistsInInventory = false;
        int Index = 0;
        if(Aux!=null){
            for(int i =0;i<GameItems.Count;i++){
                if(GameItems[i].Base.ID == Aux.ID){
                    ExistsInInventory = true;
                    Index = i;
                }
            }
            if(ExistsInInventory){
                GameItems[Index].Amount++;
            }
            else{
                Equipment equipment = new Equipment(Aux,1);            
                GameItems.Add(equipment);
            }
            Aux.UnEquipEffect(this);
        }
        Equipment[(int)type] = null;
        SetTotalStats();
    }
}
