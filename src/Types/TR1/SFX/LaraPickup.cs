using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX
{
    public class TR1PickupSFXBuilder : InjectionBuilder
    {
        public override List<InjectionData> Build()
        {
            // Leggiamo un livello TR1 che ha molti sound — Vilcabamba ne ha tanti
            TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");

            // Copiamo i metadata da un sample esistente simile (LaraKey = ID 39, SAMPLE_MODE_NORMAL)
            TR1SoundEffect existing = caves.SoundEffects[TR1SFX.LaraKey];
            TR1SoundEffect fx = new()
            {
                Chance = existing.Chance,
                Volume = existing.Volume,
                // Sostituiamo solo il WAV con il nostro "Aha"
                Samples = new() { File.ReadAllBytes("Resources/TR1/SFX/pickup.wav") },
            };

            // Assegniamo al nostro ID target nel SoundMap del livello
            // NON resettiamo il livello — lasciamo il SoundMap intatto
            caves.SoundEffects[(TR1SFX)62] = fx;

            // Creiamo il bin con removeMeshData=true per tenere solo i sample
            InjectionData data = InjectionData.Create(caves, InjectionType.General, "lara_pickup_sfx", true);

            // Teniamo solo i dati SFX, puliamo tutto il resto
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

            return new() { data };
        }
    }
}
