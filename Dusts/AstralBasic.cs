
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Dusts
{
    public class AstralBasic : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = Main.rand.NextFloat(0.9f, 1f);
        }

        public override bool Update(Dust dust)
        {
            //update position
            dust.position += dust.velocity;

            dust.scale -= 0.02f;
            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
