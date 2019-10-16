using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Waters
{
    public class SunkenSeaWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.LocalPlayer.Calamity().ZoneSunkenSea;
        }

        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("SunkenSeaWaterflow");
        }

        public override int GetSplashDust()
        {
            return 33;
        }

        public override int GetDropletGore()
        {
            return 713;
        }

        public override Color BiomeHairColor()
        {
            return Color.Blue;
        }
    }
}
