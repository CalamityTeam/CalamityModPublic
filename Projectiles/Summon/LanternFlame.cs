using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.Projectiles.Summon
{
    public class LanternFlame : ModProjectile
    {
		private bool playSound = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
			projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 1f / 255f, 0f, 0f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }

			if (projectile.alpha > 0)
			{
				projectile.alpha -= 10;
			}
			if (projectile.alpha <= 25)
			{
				float num472 = projectile.Center.X;
				float num473 = projectile.Center.Y;
				float num474 = 700f;
				bool flag17 = false;
				if (Main.player[projectile.owner].HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float num950 = npc.position.X + (float)(npc.width / 2);
						float num951 = npc.position.Y + (float)(npc.height / 2);
						float num952 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num950) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num951);
						if (num952 < num474)
						{
							num474 = num952;
							num472 = num950;
							num473 = num951;
							flag17 = true;
						}
					}
				}
				if (!flag17)
				{
					for (int num475 = 0; num475 < Main.npc.Length; num475++)
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
				}
				if (flag17)
				{
					float num483 = 16f;
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

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 48;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
			if (playSound)
            {
                Main.PlaySound(SoundID.Item74, projectile.position);
            }
			playSound = false;
		}
    }
}
