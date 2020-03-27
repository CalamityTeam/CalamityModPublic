using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicUmbrella : ModProjectile
    {
		private int counter = 0;
		private bool canHome = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbrella");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 180;
            projectile.penetrate = 5;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.rotation += 0.075f;
			projectile.alpha -= 50;
            float num633 = 1000f;
            float num634 = 1000f;
            float num635 = 2200f;
            float num636 = 150f;
            float num637 = 0.05f;
            for (int num638 = 0; num638 < Main.projectile.Length; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<MagicUmbrella>();
                if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner &&
                    flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
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
            bool flag24 = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                projectile.extraUpdates = 1;
                if (projectile.ai[1] > 40f)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = 0;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    flag24 = true;
                }
            }
            if (flag24)
            {
                return;
            }
            if (!canHome)
            {
                return;
            }
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false) && canHome)
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        num633 = num646;
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
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
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
                    float scaleFactor2 = 9f; //8
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f;
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
                float num650 = 6f;
                if (flag26)
                {
                    num650 = 15f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 8f)
                {
                    num650 = 8f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
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
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 40f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && flag25 && num633 < 500f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 value20 = vector46 - projectile.Center;
                        value20.Normalize();
                        projectile.velocity = value20 * 9f; //8
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(75, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (Main.rand.NextBool(4))
			{
				for (int n = 0; n < Main.rand.Next(1, 3); n++) //1 to 2 baseball bats
				{
					float x = target.position.X + (float)Main.rand.Next(-400, 400);
					float y = target.position.Y - (float)Main.rand.Next(500, 800);
					Vector2 vector = new Vector2(x, y);
					float num13 = target.position.X + (float)(target.width / 2) - vector.X;
					float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
					num13 += (float)Main.rand.Next(-100, 101);
					int num15 = 29;
					float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					num16 = (float)num15 / num16;
					num13 *= num16;
					num14 *= num16;
					Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<MagicBat>(), (int)((float)projectile.damage * Main.rand.NextFloat(0.3f, 0.6f)), projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), projectile.owner, 0f, 0f);
				}
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (Main.rand.NextBool(4))
			{
				for (int n = 0; n < Main.rand.Next(1, 3); n++) //1 to 2 baseball bats
				{
					float x = target.position.X + (float)Main.rand.Next(-400, 400);
					float y = target.position.Y - (float)Main.rand.Next(500, 800);
					Vector2 vector = new Vector2(x, y);
					float num13 = target.position.X + (float)(target.width / 2) - vector.X;
					float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
					num13 += (float)Main.rand.Next(-100, 101);
					int num15 = 29;
					float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					num16 = (float)num15 / num16;
					num13 *= num16;
					num14 *= num16;
					Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<MagicBat>(), (int)((float)projectile.damage * Main.rand.NextFloat(0.3f, 0.6f)), projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), projectile.owner, 0f, 0f);
				}
			}
        }
    }
}
