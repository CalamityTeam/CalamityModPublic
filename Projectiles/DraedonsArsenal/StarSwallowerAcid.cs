using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class StarSwallowerAcid : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";

        public const float Gravity = 0.25f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.minion = true;
            projectile.minionSlots = 0f;
        }
        public override void AI()
        {
            if (projectile.velocity.Y <= 10f)
            {
                projectile.velocity.Y += Gravity;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] < 10)
            {
                projectile.alpha = (int)MathHelper.Lerp(255, 0, projectile.ai[0] / 10f);
            }
            projectile.tileCollide = projectile.timeLeft <= 260;
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
                Main.dust[idx].scale = 2f;
            }
            return true;
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], new Color(255, 255, 255, 127) * projectile.Opacity, 2);
            return false;
        }
    }
}
