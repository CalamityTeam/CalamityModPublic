using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Environment
{
    public class AcidDrop : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.timeLeft = 360;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Water drip
            for (int i = 0; i < 4; i++)
            {
                int idx = Dust.NewDust(projectile.position - projectile.velocity, 2, 2, 154, 0f, 0f, 0, new Color(112, 150, 42, 127), 1f);
                Dust dust = Main.dust[idx];
                dust.position.X -= 2f;
                Main.dust[idx].alpha = 38;
                Main.dust[idx].velocity *= 0.1f;
                Main.dust[idx].velocity -= projectile.velocity * 0.025f;
                Main.dust[idx].scale = 0.75f;
            }
            return true;
        }

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            // 1 to 3 seconds of poisoned
            target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(1, 4));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, new Color(255, 255, 255, 127), ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
