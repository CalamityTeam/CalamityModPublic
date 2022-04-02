using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CorpusAvertorProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CorpusAvertor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpus Avertor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;

            if (projectile.ai[0] < 120f)
                projectile.ai[0] += 1f;

            if (projectile.ai[0] < 61f)
            {
                if (projectile.ai[0] % 20f == 0f)
                {
                    Vector2 velocity = new Vector2(projectile.velocity.X, projectile.velocity.Y);
                    float mult = projectile.ai[0] / 80f; // Ranges from 0.25 to 0.5 to 0.75
                    velocity *= mult;

                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<CorpusAvertorClone>(),
                        (int)(projectile.damage * mult), projectile.knockBack * mult, projectile.owner, projectile.ai[0]);
                }
            }
            else
            {
                projectile.velocity.X *= 1.01f;
                projectile.velocity.Y *= 1.01f;

                int scale = (int)((projectile.ai[0] - 60f) * 4.25f);
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, 0f, 0f, 100, new Color(scale, 0, 0, 50), 2f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.ai[0] >= 61f)
            {
                int scale = (int)((projectile.ai[0] - 60f) * 4.25f);
                return new Color(scale, 0, 0, 50);
            }
            return new Color(0, 0, 0, 50);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float heal = damage * 0.05f;
            if ((int)heal == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (heal > CalamityMod.lifeStealCap)
                heal = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 3f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            float heal = damage * 0.05f;
            if ((int)heal == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (heal > CalamityMod.lifeStealCap)
                heal = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 3f);
        }
    }
}
