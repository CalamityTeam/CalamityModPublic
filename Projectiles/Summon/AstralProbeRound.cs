using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
			projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0.5f, 0.1f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
				{
					ModContent.DustType<AstralOrange>(),
					ModContent.DustType<AstralBlue>()
                });
                int astral = Dust.NewDust(projectile.position, 1, 1, randomDust, 0f, 0f, 0, default, 0.5f);
                Main.dust[astral].alpha = projectile.alpha;
                Main.dust[astral].velocity *= 0f;
                Main.dust[astral].noGravity = true;
            }
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 1200f;
            bool flag17 = false;
            if (Main.player[projectile.owner].HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                {
					float num476 = npc.Center.X;
					float num477 = npc.Center.Y;
					float num478 = Math.Abs(projectile.Center.X - num476) + Math.Abs(projectile.Center.Y - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			else if (projectile.ai[0] != -1f && Main.npc[(int)projectile.ai[0]].active)
			{
				NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                {
					float num476 = npc.Center.X;
					float num477 = npc.Center.Y;
					float num478 = Math.Abs(projectile.Center.X - num476) + Math.Abs(projectile.Center.Y - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (!flag17)
			{
				for (int target = 0; target < Main.npc.Length; target++)
				{
					if (Main.npc[target].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[target].Center, 1, 1))
					{
						float num476 = Main.npc[target].position.X + (float)(Main.npc[target].width / 2);
						float num477 = Main.npc[target].position.Y + (float)(Main.npc[target].height / 2);
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
                float num483 = 17f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 5; k++)
            {
                int randomDust = Main.rand.Next(2);
                if (randomDust == 0)
                {
                    randomDust = ModContent.DustType<AstralOrange>();
                }
                else
                {
                    randomDust = ModContent.DustType<AstralBlue>();
                }
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
