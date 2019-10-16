using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Backgrounds
{
    public class AstralUndergroundBGStyle : ModUgBgStyle
    {
        public override bool ChooseBgStyle() => Main.LocalPlayer.InAstral();

        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i <= 3; i++)
            {
                textureSlots[i] = mod.GetBackgroundSlot("Backgrounds/AstralUG" + i.ToString());
            }
        }
    }
}
