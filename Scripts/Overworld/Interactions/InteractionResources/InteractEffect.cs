using Godot;
using System;

public partial class InteractEffect : Node
{
    public Callable StartInteract = new Callable();
    public Callable EndInteract = new Callable();
    public virtual void ConnectCall()
    {
        EndInteract = new Callable(this,MethodName.DisconnectCall);
    }
    public virtual void DisconnectCall()
    {

    }
}
