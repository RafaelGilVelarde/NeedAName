using Godot;
using System;

[GlobalClass]
public partial class InteractEffectResource : Resource
{
    public Callable StartInteract = new Callable();
    public Callable EndInteract = new Callable();

    public Node2D BaseNode;

    public virtual void Ready()
    {
        
    }
    public virtual void ConnectCall()
    {
        EndInteract = new Callable(this,MethodName.DisconnectCall);
    }
    public virtual void DisconnectCall()
    {

    }
}
