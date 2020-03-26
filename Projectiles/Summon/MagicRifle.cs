using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicRifle : ModProjectile
    {
		private int counter = 0;
		private bool canHome = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rifle");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float num637 = 0.05f;
            for (int num638 = 0; num638 < Main.projectile.Length; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<MagicRifle>();
                if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num638].position.X)
                    {
                        projectile.velocity.X = projectile.velocity.X - num637;
                    }
                    else
                    {
                        projectile.velocity.X = projectile.velocity.X + num637;
                    }
                    if (projectile.position.Y < Main.projectile[num638].position.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num637;
                    }
                    else
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num637;
                    }
                }
            }
			if (counter <= 30)
			{
				counter++;
				canHome = true;
			}
            float homingRange = 1000f;
            float num634 = 1300f;
            float num635 = 2600f;
            float num636 = 600f;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false) && canHome)
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < homingRange)
                    {
						homingRange = num646;
                        vector46 = npc.Center;
                        flag25 = true;
                    }
                }
            }
            else
            {
                for (int num645 = 0; num645 < Main.npc.Length; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false) && canHome)
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25 && num646 < homingRange)
                        {
                            homingRange = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 18f; //12
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 9f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[0] == 1f;
                }
                float num650 = 12f;
                if (flag26)
                {
                    num650 = 30f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -120f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 16f)
                {
                    num650 = 16f;
                }
                if (num651 < num636 && flag26)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num651 > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num651 > 70f)
                {
                    vector48.Normalize();
                    vector48 *= num650;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            if (flag25)
            {
				projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
				projectile.rotation = (vector46 - projectile.Center).ToRotation() + (projectile.spriteDirection == 1 ? MathHelper.ToRadians(315) : MathHelper.ToRadians(135));
            }
            else
            {
				projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
				projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? MathHelper.ToRadians(315) : MathHelper.ToRadians(135));
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float scaleFactor3 = 6f;
                int projType = ModContent.ProjectileType<MagicBullet>();
                if (flag25 && projectile.ai[1] == 0f)
                {
					if (Main.rand.NextBool(6))
					{
						Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20, 0.1f);
					}
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        Vector2 value19 = vector46 - projectile.Center;
                        value19.Normalize();
                        value19 *= scaleFactor3;
						Main.PlaySound(SoundID.Item40, projectile.position);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value19.X, value19.Y, projType, projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(148, 0, 211, projectile.alpha);
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
