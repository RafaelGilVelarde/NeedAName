using Godot;
using System;

public partial class MenuScreens : Control
{
    public override void _Ready()
    {
        Setup();
    }
    public void EnableButtons(Control Buttons, bool Enable){
        foreach(BaseButton button in Buttons.GetChildren()){
            button.Disabled = !Enable;
            }
    }
    public virtual void Select(){
        InitialDisplay();
    }
    public virtual void Confirm(){

    }
    public virtual void Deny(){
        
    }
    public virtual void Setup(){
        
    }
    public virtual void InitialDisplay(){

    }
}
