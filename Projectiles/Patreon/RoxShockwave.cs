using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class RoxShockwave : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.penetrate = -1;
            projectile.width = 450;
            projectile.height = 450;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 40;
        }
    }
}
