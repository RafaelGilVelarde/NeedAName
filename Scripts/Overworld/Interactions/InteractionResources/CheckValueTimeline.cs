using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;

[GlobalClass]
public partial class CheckValueTimeline : Resource
{
    [Export] int TimelineIndex; 
    [Export] Array<string> KeyIndexes;
    [Export] Array<Variant> DictValues;
    [Export] Dictionary<string, Variant> Values;

    public int GetTimeline(int Index){
        int aux = 0;
        Flags flags = GameManager.Instance.Data.Flags;
        for(int i =0;i<Values.Keys.Count;i++){
            string IndexAux = Values.Keys.ElementAt(i);
            if (flags.Dict[IndexAux].Equals(Values[IndexAux]))
            {
                aux++;
            }

        }
        Debug.WriteLine("CheckTimeline: "+this +": "+ aux);
        if (aux >= Values.Keys.Count)
        {
            return TimelineIndex;
        }
        else
        {
            return Index;
        }
    }
    
}
