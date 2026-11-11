using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.SFX;

// Lara's "Aha!" pickup cue for TR4. TR4 keeps its samples inside the level
// file rather than in a main.sfx, so the WAV travels inline in the injection
// under sample ID 62, matching the slot TR2/TR3 use for the pickup sound.
public class TR4PickupSFXBuilder : InjectionBuilder
{
    private const short _sampleID = 62;
    private const string _wavPath = "Resources/TR4/SFX/pickup.wav";
    private static readonly string _levelPath = $"Resources/TR4/{TR4LevelNames.KARNAK}";

    public override string ID => "tr4_lara_pickup_sfx";

    public override IEnumerable<string> RequiredResources => [_wavPath, _levelPath];

    public override List<InjectionData> Build()
    {
        // Donor for the metadata of a similar one-shot Lara sound.
        TR4Level level = _control4.Read(_levelPath);
        TR4SoundEffect existing = level.SoundEffects[TR4SFX.LaraKey];

        // Do not go through InjectionData.Create(TR4Level): the donor's sound
        // effects must be written back out as a level, and a freshly built
        // TR4SoundEffect without samples crashes that writer.
        InjectionData data = InjectionData.Create(TRGameVersion.TR4, InjectionType.General, "lara_pickup_sfx");
        data.SFX.Add(new TRSFXData
        {
            ID = _sampleID,
            Volume = (ushort)(existing.Volume << 7),
            Chance = existing.Chance,
            // Mode NORMAL, exactly one sample.
            Characteristics = (ushort)((existing.GetFlags() & ~0x3C) | 0x04),
            Pitch = 0,
            Range = existing.Range,
            Data = [File.ReadAllBytes(_wavPath)],
        });

        return [data];
    }
}
