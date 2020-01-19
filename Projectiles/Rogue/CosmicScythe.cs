using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    class CosmicScythe : ModProjectile
    {
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
            projectile.timeLeft = 300;
            projectile.alpha = 100;
            projectile.penetrate = 5;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f * (float)projectile.direction;
            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            projectile.velocity *= 0.95f;
            if (projectile.timeLeft == 300)
            {
                originalDamage = projectile.damage;
                projectile.damage = 0;
            }
            if (projectile.timeLeft <= 275)
            {
                if (projectile.timeLeft > 250)
                    projectile.velocity *= 1.06f;
                projectile.damage = (int) ((double)originalDamage * 1.25);
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 1500f;
                bool flag17 = false;
                for (int num475 = 0; num475 < 200; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(projectile, false)) 
                    {
                        float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                        if (num478 < num474)
                        {
                            num474 = num478;
                            num472 = num476;
                            num473 = num477;
                            flag17 = true;
                        }
                    }
                }
                if (flag17)
                {
                    float num483 = 20f;
                    Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num484 = num472 - vector35.X;
                    float num485 = num473 - vector35.Y;
                    float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                    num486 = num483 / num486;
                    num484 *= num486;
                    num485 *= num486;
                    projectile.velocity.X = (projectile.velocity.X * 21f + num484) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 21f + num485) / 21f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60, false);
            projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 27, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 27, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 27, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
