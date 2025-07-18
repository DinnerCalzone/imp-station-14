using Content.Client.UserInterface.Controls;
using Content.Shared._Impstation.Pleebnar;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client._Impstation.Pleebnar;

[GenerateTypedNameReferences]
public sealed partial class PleebnarTelepathyWindow : FancyWindow
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    public Action<string?>? OnVisionSelect;
    private ProtoId<PleebnarVisionPrototype> _vision;
    private readonly List<PleebnarVisionPrototype> _visions = [];
    private readonly RadioOptions<string> _visionsbuttons;
    // init after opening
    public PleebnarTelepathyWindow()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);
        _visionsbuttons = new RadioOptions<string>(RadioOptionsLayout.Vertical);
        visionsVbox.AddChild(_visionsbuttons);
        _visionsbuttons.OnItemSelected += args =>
        {
            OnVisionSelect?.Invoke((string?)args.Button.GetItemMetadata(args.Id));
            _visionsbuttons.Select(args.Id);//need to actually tell the ui element to select the relevant radio button or else it won't stick
        };
    }
    //reload visions and add to vision list
    public void ReloadVisions()
    {
        foreach (var vision in _proto.EnumeratePrototypes<PleebnarVisionPrototype>())
        {
            _visions.Add(vision);
        }
        _visions.Sort((a, b) => a.Name.CompareTo((b.Name)));
    }
    //add visions to the radiobutton element
    public void AddVisions()
    {
        _visionsbuttons.Clear();
        foreach (var entry in _visions)
        {
            AddVision(entry.Name, entry.ID);
        }

    }
    //add a vision (ruddygreat made this one because the radio button is evil, thank you)
    private void AddVision(string name, ProtoId<PleebnarVisionPrototype> vision)
    {
        var id = _visionsbuttons.AddItem(Loc.GetString(name), vision);
        //gets child 0 because that's the container that the radio button options are put in
        _visionsbuttons.GetChild(0).GetChild(id).ToolTip = Loc.GetString(_proto.Index(vision).VisionString);
        _visionsbuttons.SetItemMetadata(id, vision.Id);
        if (vision == _vision)// if the currently added button corresponds to the selected id then select it
            _visionsbuttons.Select(id);
    }
    //update the state
    public void UpdateState(ProtoId<PleebnarVisionPrototype> vision)
    {
        _vision = vision;//set new vision
        for (var id = 0; id < _visionsbuttons.ItemCount; id++)//get vision id
        {
            if (string.Equals(vision.Id, _visionsbuttons.GetItemMetadata(id)))//if ids match then select that id
            {
                _visionsbuttons.Select(id);
                break;
            }
        }
    }
}
