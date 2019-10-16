using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Dusts.FurnitureDusts
{
    public class BloomTileLeaves : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.noGravity = false;
            dust.scale = 1.2f;
            dust.alpha = 100;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y += 0.05f;
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X;
            dust.scale -= 0.03f;
            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
