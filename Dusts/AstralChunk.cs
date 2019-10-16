
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Dusts
{
    public class AstralChunk : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
        }

        public override bool Update(Dust dust)
        {
            //update position / rotation
            dust.velocity.Y += 0.1f;
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X;

            dust.alpha += 6;
            if (dust.alpha > 240)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
