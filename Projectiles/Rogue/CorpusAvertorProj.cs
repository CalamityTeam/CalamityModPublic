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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f;

            if (Projectile.ai[0] < 120f)
                Projectile.ai[0] += 1f;

            if (Projectile.ai[0] < 61f)
            {
                if (Projectile.ai[0] % 20f == 0f)
                {
                    Vector2 velocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y);
                    float mult = Projectile.ai[0] / 80f; // Ranges from 0.25 to 0.5 to 0.75
                    velocity *= mult;

                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<CorpusAvertorClone>(),
                        (int)(Projectile.damage * mult), Projectile.knockBack * mult, Projectile.owner, Projectile.ai[0]);
                }
            }
            else
            {
                Projectile.velocity.X *= 1.01f;
                Projectile.velocity.Y *= 1.01f;

                int scale = (int)((Projectile.ai[0] - 60f) * 4.25f);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, 0f, 0f, 100, new Color(scale, 0, 0, 50), 2f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[0] >= 61f)
            {
                int scale = (int)((Projectile.ai[0] - 60f) * 4.25f);
                return new Color(scale, 0, 0, 50);
            }
            return new Color(0, 0, 0, 50);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
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

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 3f);
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

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 3f);
        }
    }
}
