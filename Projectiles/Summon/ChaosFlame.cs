using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ChaosFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/ChaosFlameSmall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Flame");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.minionSlots = 0f;
            projectile.minion = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.frameCounter++ % 4 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0f / 255f);
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale = 1f - Main.rand.NextFloat() * 0.5f;
                projectile.localAI[0] += 1f;
            }
            if (Main.rand.NextBool(4))
            {
                int flame = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.NextBool(3) ? 16 : 174, 0f, 0f);
                Main.dust[flame].noGravity = true;
                Main.dust[flame].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft == 300)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
