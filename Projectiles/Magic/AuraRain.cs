using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class AuraRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aura Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = 45;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.scale = 1.1f;
            projectile.magic = true;
            projectile.extraUpdates = 1;
            aiType = 239;
        }

        public override void AI()
        {
            Dust dust4 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
            dust4.velocity = Vector2.Zero;
            dust4.position -= projectile.velocity / 5f;
            dust4.noGravity = true;
            dust4.scale = 0.8f;
            dust4.noLight = true;
        }
    }
}
