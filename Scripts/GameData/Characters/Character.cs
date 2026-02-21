using Godot;
using Godot.Collections;
using System.Diagnostics;


[GlobalClass]
public partial class Character : Resource
{
    public Node2D NodeCharacter;
    [Export] public string Name;
    [Export] public CharacterBase Base { get; private set; }
    [Export] public Array<Moves> Moves;
    [Export] public Array<Items> items;
    [Export] public Array<EquipmentBase> Equipment;

    [Export] public Stats stats, EquipStats = new Stats(), TotalStats = new Stats();
    [Export] public bool isControlledByPlayer;
    [Export] public bool Active = true;
    [Export] public Key Key { get; private set; }
    [Export] public InputEventKey EventKey { get; private set; }


    [Signal]
    public delegate void _GetHitEventHandler();
    [Signal]
    public delegate void _ChangeHPEventHandler(int HP);
    [Signal]
    public delegate void _ChangeWPEventHandler(int WP, bool Hide);
    [Signal]
    public delegate void _DieEventHandler();


    public enum Status
    {
        Normal,
        KO,
    };
    [Export] public Status status;
    public bool CheckWP(int WP)
    {
        if (stats.WP >= WP)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ChangeWP(int WP)
    {
        stats.WP += WP;
        stats.WP = Mathf.Clamp(stats.WP, 0, 100);
        EmitSignal("_ChangeWP", stats.WP, true);
    }
    public virtual void DamageCalc(BattleCharacter Character, BattleCharacter TargetCharacter, Moves move)
    {

    }
    public virtual void ChangeHP(int hp, bool ShowText = true)
    {
        stats.HP += hp;
        stats.HP = Mathf.Clamp(stats.HP, 0, TotalStats.MaxHP);
        if (ShowText)
        {
            EmitSignal("_ChangeHP", hp);
            if (hp != 0)
            {
                ShowTextLabel($"[center]{hp}[/center]", Base.TextEffectColor);
            }
            else
            {
                ShowTextLabel("[center]"+Tr("BLOCKED")+"[/center]", Base.TextEffectColor);
            }            
        }
        
        if (hp < 0)
        {
            EmitSignal("_GetHit");
        }

        if (stats.HP <= 0)
        {
            stats.HP = 0;
            status = Status.KO;
            EmitSignal("_Die");
        }
    }
    public virtual void SetStats()
    {
        Stats BaseStats = Base.BaseStats;
        if (stats.Lv == 0)
        {
            stats.Lv = 1;
        }
        stats.MaxHP = (int)(BaseStats.MaxHP * Mathf.Log(2 * stats.Lv));
        stats.Atk = (int)(BaseStats.Atk * Mathf.Log(2 * stats.Lv));
        stats.Def = (int)(BaseStats.Def * Mathf.Log(2 * stats.Lv));
        stats.SpAtk = (int)(BaseStats.SpAtk * Mathf.Log(2 * stats.Lv));
        stats.SpDef = (int)(BaseStats.SpDef * Mathf.Log(2 * stats.Lv));
        stats.Speed = (int)(BaseStats.Speed * Mathf.Log(2 * stats.Lv));
        SetTotalStats();
    }
    public void SetTotalStats()
    {
        TotalStats.MaxHP = stats.MaxHP + EquipStats.MaxHP;
        stats.HP = Mathf.Clamp(stats.HP, 0, TotalStats.MaxHP);
        TotalStats.Atk = stats.Atk + EquipStats.Atk;
        TotalStats.Def = stats.Def + EquipStats.Def;
        TotalStats.SpAtk = stats.SpAtk + EquipStats.SpAtk;
        TotalStats.SpDef = stats.SpDef + EquipStats.SpDef;
        TotalStats.Speed = stats.Speed + EquipStats.Speed;
    }
    public void ChangeKey(InputEventKey newKey)
    {
        if (newKey != null)
        {
            PartyCharacterBase aux = (PartyCharacterBase)Base;
            if (InputMap.HasAction("SelectedKey" + aux.PartyId))
            {
                InputMap.ActionEraseEvents("SelectedKey" + aux.PartyId);
                InputMap.ActionAddEvent("SelectedKey" + aux.PartyId, newKey);
            }
            else
            {
                InputMap.AddAction("SelectedKey" + aux.PartyId);
                InputMap.ActionAddEvent("SelectedKey" + aux.PartyId, newKey);
            }
            EventKey = newKey;
            Key = newKey.Keycode;            
        }
    }

    public virtual void Equip(Equipment equipment)
    {
        EquipmentBase equipmentBase = (EquipmentBase)equipment.Base;
        UnEquip(equipmentBase.EquipType);
        Equipment[(int)equipmentBase.EquipType] = equipmentBase;
        //equipment.Base.Effect(new Array<Character> { this });
        equipment.Use(new Array<Character> { this });
        SetTotalStats();
    }
    public virtual void UnEquip(EquipmentType type)
    {
        EquipmentBase Aux = Equipment[(int)type];
        Array<Items> GameItems = GameManager.Instance.Data.items[1].items;
        bool ExistsInInventory = false;
        int Index = 0;
        if (Aux != null)
        {
            for (int i = 0; i < GameItems.Count; i++)
            {
                if (GameItems[i].Base.ID == Aux.ID)
                {
                    ExistsInInventory = true;
                    Index = i;
                }
            }
            if (ExistsInInventory)
            {
                GameItems[Index].Amount++;
            }
            else
            {
                Equipment equipment = new Equipment(Aux, 1);
                GameItems.Add(equipment);
            }
            Aux.UnEquipEffect(this);
        }
        Equipment[(int)type] = null;
        SetTotalStats();
    }
    public void ShowTextLabel(string Text, Color color)
    {
        Node2D HPLabelParent = GameManager.Instance.TextEffectPrefabs[0].Instantiate<Node2D>();
        Debug.WriteLine("Name: "+Base.Name);
        Debug.WriteLine("Text: "+Text);
        Debug.WriteLine("Node: "+NodeCharacter);
        RichTextLabel HPLabel = HPLabelParent.GetChild(0).GetChild<RichTextLabel>(0);
        HPLabel.Text = "[center]" + Text + "[/center]";
        HPLabel.AddThemeColorOverride("default_color", color);
        NodeCharacter.GetTree().CurrentScene.AddChild(HPLabelParent);
        HPLabelParent.Position = NodeCharacter.GlobalPosition;
        HPLabelParent.ZIndex = 20;
    }

    public virtual void ResetCharacter()
    {
        stats.HP = TotalStats.MaxHP;
        stats.WP = 0;
    }
    public void LearnMove(Moves Move)
    {
        Debug.WriteLine("Move Learned: "+Move.Base.Name);
        Moves.Add((Moves)Move.Duplicate());
    }
}
