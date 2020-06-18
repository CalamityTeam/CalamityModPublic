using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FantasyTalismanStealth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Talisman");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (Main.rand.Next(3) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
			if (projectile.ai[0] != 1f)
			{
				if (projectile.timeLeft % 10 == 0)
				{
					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<LostSoulFriendly>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					}
				}
			}
            projectile.StickyProjAI(15);
            if (projectile.ai[0] == 1f)
            {
				if (projectile.timeLeft % 4 == 0)
				{
					if (Main.rand.NextBool(2))
					{
						int spiritDamage = projectile.damage / 2;
						int[] numArray1 = new int[Main.maxNPCs];
						int maxValue1 = 0;
						int maxValue2 = 0;
						for (int index = 0; index < Main.maxNPCs; ++index)
						{
							if (Main.npc[index].CanBeChasedBy((object) projectile, false))
							{
								float num2 = Math.Abs(Main.npc[index].Center.X - projectile.Center.X) + Math.Abs(Main.npc[index].Center.Y - projectile.Center.Y);
								if (num2 < 800f)
								{
									if (Collision.CanHit(projectile.position, 1, 1, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height) && num2 > 50f)
									{
										numArray1[maxValue2] = index;
										++maxValue2;
									}
									else if (maxValue2 == 0)
									{
										numArray1[maxValue1] = index;
										++maxValue1;
									}
								}
							}
						}
						if (maxValue1 == 0 && maxValue2 == 0)
							return;
						int num3 = maxValue2 <= 0 ? numArray1[Main.rand.Next(maxValue1)] : numArray1[Main.rand.Next(maxValue2)];
						double num4 = 4.0;
						float num5 = (float) Main.rand.Next(-100, 101);
						float num6 = (float) Main.rand.Next(-100, 101);
						double num7 = Math.Sqrt((double) num5 * (double) num5 + (double) num6 * (double) num6);
						float num8 = (float) (num4 / num7);
						float SpeedX = num5 * num8;
						float SpeedY = num6 * num8;
						int ghost = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY, ProjectileID.SpectreWrath, spiritDamage, 0.0f, projectile.owner, (float)num3, 0f);
						Main.projectile[ghost].Calamity().forceRogue = true;
						Main.projectile[ghost].penetrate = 1;
					}
				}
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(1, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.timeLeft > 240)
			{
				projectile.timeLeft = 240;
			}
        }
    }
}
