using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

// Lara's "Aha!" pickup cue for TR3. The sample is not reachable through the
// game's main.sfx, so the WAV travels inline in the injection under sample
// ID 62, matching the slot TR2 uses for the pickup sound.
public class TR3PickupSFXBuilder : InjectionBuilder
{
    private const short _sampleID = 62;
    private const string _wavPath = "Resources/TR3/SFX/pickup.wav";
    private static readonly string _levelPath = $"Resources/TR3/{TR3LevelNames.JUNGLE}";

    public override string ID => "tr3_lara_pickup_sfx";

    public override IEnumerable<string> RequiredResources => [_wavPath, _levelPath];

    public override List<InjectionData> Build()
    {
        // Donor for the metadata of a similar one-shot Lara sound.
        TR3Level level = _control3.Read(_levelPath);
        TR3SoundEffect existing = level.SoundEffects[TR3SFX.LaraKey];

        // An empty injection carrying only the SFX record. Building it from the
        // donor level instead would drag its textures, palette and models along.
        InjectionData data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, "lara_pickup_sfx");
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
