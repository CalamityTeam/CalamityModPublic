using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CosmicScythe : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SignusScythe";

        private int originalDamage;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 400;
            projectile.alpha = 100;
            projectile.penetrate = 5;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f * (float)projectile.direction;
            int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1f);
            Main.dust[shadow].noGravity = true;
            Main.dust[shadow].velocity *= 0f;
            projectile.velocity *= 0.95f;
            if (projectile.timeLeft == 400)
            {
                originalDamage = projectile.damage;
                projectile.damage = 0;
            }
            if (projectile.timeLeft <= 375)
            {
                if (projectile.timeLeft > 350)
                    projectile.velocity *= 1.06f;
                projectile.damage = (int)(originalDamage * 1.25);
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 12f, 20f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int buffType = projectile.ai[0] == 1f ? BuffID.ShadowFlame : ModContent.BuffType<GodSlayerInferno>();
            target.AddBuff(buffType, 60, false);
            projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 50);
            for (int d = 0; d < 4; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, default, 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 12; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, default, 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, default, 2f);
                Main.dust[shadow].velocity *= 2f;
            }
        }
    }
}
