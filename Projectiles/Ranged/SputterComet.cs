using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Ranged
{
    public class SputterComet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.1f, 0.3f, 0.8f);
            if (Main.rand.NextBool(3))
            {
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, ModContent.DustType<AstralBlue>(), 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, projectile.position);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            for (int num480 = 0; num480 < 3; num480++)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
