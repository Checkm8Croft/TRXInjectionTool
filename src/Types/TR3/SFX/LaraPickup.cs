using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.SFX;

public class TR3PickupSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        // Leggiamo un livello TR3 ricco di sound — Jungle ne ha tanti
        TR3Level jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");

        // Copiamo i metadata da TR3SFX.LaraKey (ID 39, caratteristiche simili al pickup)
        TR3SoundEffect existing = jungle.SoundEffects[TR3SFX.LaraKey];

        // Costruiamo il TRSFXData manualmente con il WAV embedded,
        // esattamente come fa TR1UziSFXBuilder — bypassando LoadSFX/main.sfx
        TRSFXData sfxData = new()
        {
            ID       = 62,
            Volume   = (ushort)(existing.Volume << 7),
            Chance   = existing.Chance,
            // Characteristics: mode=NORMAL(0), num_samples=1 → flags = (1 << 2) = 0x0004
            Characteristics = (ushort)((existing.GetFlags() & ~0x3C) | 0x04),
            Pitch    = 0,
            Range    = existing.Range,
            Data     = new() { File.ReadAllBytes("Resources/TR3/SFX/pickup.wav") },
        };

        // NON resettiamo il livello — il SoundMap di Jungle rimane intatto
        // in modo che InjectionData.Create trovi l'entry 62 serializzabile
        jungle.SoundEffects[(TR3SFX)62] = new TR3SoundEffect
        {
            Volume   = existing.Volume,
            Chance   = existing.Chance,
            Pitch    = 0,
            Range    = existing.Range,
        };

        InjectionData data = InjectionData.Create(jungle, InjectionType.General, "lara_pickup_sfx", true);

        // Puliamo tutto tranne SFX
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.AnimCommands.Clear();
        data.Models.Clear();
        data.Images.Clear();
        data.ObjectTextures.Clear();
        data.SpriteSequences.Clear();
        data.SpriteTextures.Clear();
        data.StaticObjects.Clear();
        data.CinematicFrames.Clear();

        // Sostituiamo l'entry SFX generato automaticamente con il nostro
        // che ha i byte WAV embedded invece del SampleOffset
        data.SFX.Clear();
        data.SFX.Add(sfxData);

        return new() { data };
    }
}
