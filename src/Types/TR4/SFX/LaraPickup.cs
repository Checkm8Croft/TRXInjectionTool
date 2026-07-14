using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.SFX;

public class TR4PickupSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR4Level karnak = _control4.Read($"Resources/TR4/{TR4LevelNames.KARNAK}");
        TR4SoundEffect existing = karnak.SoundEffects[TR4SFX.LaraKey];

        // Costruiamo InjectionData direttamente senza passare per Create(TR4Level),
        // perché TR4SoundEffect richiede Samples non-null per essere serializzato.
        InjectionData data = InjectionData.Create(TRGameVersion.TR4, InjectionType.General, "lara_pickup_sfx");

        data.SFX.Add(new TRSFXData
        {
            ID              = 62,
            Volume          = (ushort)(existing.Volume << 7),
            Chance          = existing.Chance,
            Characteristics = (ushort)((existing.GetFlags() & ~0x3C) | 0x04),
            Pitch           = 0,
            Range           = existing.Range,
            Data            = new() { File.ReadAllBytes("Resources/TR4/SFX/pickup.wav") },
        });

        return new() { data };
    }
}
