using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class HadopelagicEcho2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Echo");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.scale = 0.85f;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.timeLeft = 30;
			projectile.extraUpdates = 1;
			projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.width = (int)(36f * projectile.scale);
			projectile.height = (int)(36f * projectile.scale);
            if (projectile.alpha > 100)
            {
                projectile.alpha -= 25;
            }
			if (projectile.alpha < 100)
				projectile.alpha = 100;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);

			if (projectile.ai[0] == 1f)
			{
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 500f;
                bool flag17 = false;

                for (int num475 = 0; num475 < Main.projectile.Length; num475++)
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
                    float num483 = 8f;
                    Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num484 = num472 - vector35.X;
                    float num485 = num473 - vector35.Y;
                    float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                    num486 = num483 / num486;
                    num484 *= num486;
                    num485 *= num486;
                    projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                }
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
