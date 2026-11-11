using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

// Lara's "Aha!" pickup cue. TR1 never shipped it, so the WAV travels inline in
// the injection under sample ID 62: the slot TR2/TR3 already use for the
// pickup sound, which keeps the ID consistent across games.
public class TR1PickupSFXBuilder : InjectionBuilder
{
    private const short _sampleID = 62;
    private const string _wavPath = "Resources/TR1/SFX/pickup.wav";
    private static readonly string _levelPath = $"Resources/{TR1LevelNames.VILCABAMBA}";

    public override string ID => "tr1_lara_pickup_sfx";

    public override IEnumerable<string> RequiredResources => [_wavPath, _levelPath];

    public override List<InjectionData> Build()
    {
        // Only used as a donor for the metadata (volume, chance) of a similar
        // one-shot Lara sound.
        TR1Level level = _control1.Read(_levelPath);
        TR1SoundEffect existing = level.SoundEffects[TR1SFX.LaraKey];

        TR1SoundEffect fx = new()
        {
            Chance = existing.Chance,
            Volume = existing.Volume,
            Samples = [File.ReadAllBytes(_wavPath)],
        };

        // An empty injection carrying only the SFX record, so nothing from the
        // donor level (textures, palette, models) leaks into the file.
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "lara_pickup_sfx");
        data.SFX.Add(TRSFXData.Create(_sampleID, fx));

        return [data];
    }
}
