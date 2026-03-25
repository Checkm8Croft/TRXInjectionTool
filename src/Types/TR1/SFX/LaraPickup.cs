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
            TR1Level caves = _control1.Read($"Resources/{TR1LevelNames.CAVES}");
            TR1SoundEffect fx = new()
            {
                Chance = 0,
                Volume = 0xFF,
                Samples = new()
            {
                File.ReadAllBytes("Resources/TR1/SFX/pickup.wav"),
            },
            };
            ResetLevel(caves);
            caves.SoundEffects[(TR1SFX)62] = fx;

            InjectionData data = InjectionData.Create(caves, InjectionType.General, "lara_pickup_sfx");
            return new() { data };
        }
    }
}
