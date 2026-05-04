using Godot;
using System;
using System.Diagnostics;

public partial class VolumeSlider : Control
{
    [Export] ProgressBar Slider;
    [Export] string AudioBus;
    [Export] RichTextLabel VolumeLevel;
    [Export] public BaseButton SelectButton;
    int BusIndex;

    public override void _EnterTree()
    {
        base._EnterTree();
        BusIndex = AudioServer.GetBusIndex(AudioBus);
        Debug.WriteLine(AudioBus+":"+BusIndex);
        Debug.WriteLine("Max index: "+GameManager.Instance.Settings.Volume.Count);


        AudioServer.SetBusVolumeDb(BusIndex,Mathf.LinearToDb((float)Mathf.Clamp(GameManager.Instance.Settings.Volume[BusIndex],0,1))); 
        Slider.ValueChanged += ChangeVolume;
        ShowValue();
        SelectButton.Pressed+=Enable; 
    }

    void ChangeVolume(double Value)
    {
        //VolumeLevel.Text = $"[center]{Value}[/center]";
        AudioServer.SetBusVolumeDb(BusIndex,Mathf.LinearToDb((float)Mathf.Clamp(Value,0,1)));
        GameManager.Instance.Settings.Volume[BusIndex] = Value;
    }
    public void ShowValue()
    {
        Slider.Value = GameManager.Instance.Settings.Volume[BusIndex];
    }
    public void Enable()
    {
        Debug.WriteLine("Eenabling");
        ProcessMode = ProcessModeEnum.Inherit;
    }
    public void Disable()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsActionPressed("MoveRight"))
        {
            Slider.Value += 0.01;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            Slider.Value -= 0.01;
        }
    }
}
