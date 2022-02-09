using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blast");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 180;
            projectile.height = 180;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            cooldownSlot = 1;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if (projectile.wet || projectile.lavaWet)
            {
                projectile.Kill();
            }
            if (projectile.ai[1] == 0f)
            {
				int dustType = (Main.dayTime && !CalamityWorld.malice) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
				for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }
            if (Math.Abs(projectile.velocity.X) > 0.2)
            {
                projectile.spriteDirection = -projectile.direction;
            }
            if (projectile.velocity.X < 0f)
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            }
            else
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (Main.dayTime && !CalamityWorld.malice) ? new Color(250, 150, 0, projectile.alpha) : new Color(100, 200, 250, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = (Main.dayTime && !CalamityWorld.malice) ? Main.projectileTexture[projectile.type] : ModContent.GetTexture("CalamityMod/Projectiles/Boss/HolyBlastNight");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 shootFromVector = new Vector2(projectile.Center.X, projectile.Center.Y);
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
				float velocity = 5f;
				int totalProjectiles = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 5 : 4;
				int type = ModContent.ProjectileType<HolyFire2>();
				int damage = (int)Math.Round(projectile.damage * 0.75);
				for (int i = 0; i < totalProjectiles; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), type, damage, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyBlastImpact"), projectile.Center);
            int dustType = (Main.dayTime && !CalamityWorld.malice) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int num193 = 0; num193 < 4; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
            }
            for (int num194 = 0; num194 < 40; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			int buffType = (Main.dayTime && !CalamityWorld.malice) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
			target.AddBuff(buffType, 240);
		}

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
