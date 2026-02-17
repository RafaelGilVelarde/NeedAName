using Godot;
using MonoCustomResourceRegistry;
using System;

public partial class LanguageButtons : BaseButton
{
    [Export] public string Language;

    public void Setup()
    {
        Pressed+=SetLocale;
    }
    void SetLocale()
    {
        TranslationServer.SetLocale(Language);
        GameManager.Instance.Settings.Language = Language;
        GameManager.Instance.SaveSettings();
    }
    public override void _ExitTree()
    {
        base._ExitTree();
        Pressed-=SetLocale;
    }
}
