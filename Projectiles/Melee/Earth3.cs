using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Earth3 : ModProjectile
    {
        private int noTileHitCounter = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int randomToSubtract = Main.rand.Next(1, 4);
            noTileHitCounter -= randomToSubtract;
            if (noTileHitCounter == 0)
            {
                projectile.tileCollide = true;
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, (int)projectile.position.X, (int)projectile.position.Y);
                }
            }
            projectile.alpha -= 15;
            int num58 = 150;
            if (projectile.Center.Y >= projectile.ai[1])
            {
                num58 = 0;
            }
            if (projectile.alpha < num58)
            {
                projectile.alpha = num58;
            }
            projectile.localAI[0] += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;
            if (Main.rand.NextBool(16))
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[num59].velocity = value3 * 0.66f;
                Main.dust[num59].position = projectile.Center + value3 * 12f;
            }
            if (Main.rand.NextBool(48))
            {
                int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[num60].velocity *= 0.66f;
                Main.gore[num60].velocity += projectile.velocity * 0.3f;
            }
            if (projectile.ai[1] == 1f)
            {
                projectile.light = 0.9f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
                if (Main.rand.NextBool(20))
                {
                    Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
            Lighting.AddLight(projectile.Center, 1.5f, 0f, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 30;
            projectile.height = 30;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 35; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
        }
    }
}
