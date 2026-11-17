using Godot;
using Godot.Collections;
using System.Diagnostics;


[GlobalClass]
public partial class Flags : Resource
{
    [Export] public Dictionary<string,Variant> Dict = new Dictionary<string, Variant>();


    [Signal]
    public delegate void _DialogueFlagsBoolChangedEventHandler(int Change, bool Changed);

    [Signal]
    public delegate void _DictFlagsChangedEventHandler(string Key, bool Changed);

    
    public void ChangeDictFlag(string Key, Variant variant)
    {

        bool changed = !Dict[Key].Equals(variant);
        Dict[Key] = variant;
        EmitSignal_DictFlagsChanged(Key,changed);
    }

    public void ChangeDictArrayFlag(string Key, Array<int> Indexes, Array<Variant> Variants)
    {
        bool changed = false;
        for(int i = 0; i < Indexes.Count; i++)
        {
            if (i < ((Array)Dict[Key]).Count)
            {
                changed = !((Array)Dict[Key])[Indexes[i]].Equals(Variants[i]);
                ((Array)Dict[Key])[Indexes[i]] = Variants[i];                            
            }
            else
            {
                ((Array)Dict[Key]).Add(Variants[i]);
                changed = true;
            }
        }
        EmitSignal_DictFlagsChanged(Key,changed);

    }




}
