using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Patreon
{
    public class MiniGuardianDefense : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Defensive Guardian");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
			projectile.tileCollide = false;
            projectile.width = 62;
            projectile.height = 80;
			projectile.minionSlots = 0f;
			projectile.minion = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
			projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.gDefense = false;
            }
            if (modPlayer.gDefense)
            {
                projectile.timeLeft = 2;
            }
            if (!modPlayer.pArtifact || player.maxMinions < 8)
            {
                projectile.active = false;
                return;
            }
            float num535 = projectile.position.X;
            float num536 = projectile.position.Y;
            float num537 = 3000f;
            bool flag19 = false;
            NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
            {
                float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                if (num541 < num537)
                {
                    num537 = num541;
                    num535 = num539;
                    num536 = num540;
                    flag19 = true;
                }
            }
            if (!flag19)
            {
                int num3;
                for (int num542 = 0; num542 < 200; num542 = num3 + 1)
                {
                    if (Main.npc[num542].CanBeChasedBy(projectile, false))
                    {
                        float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                        float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                        float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                        if (num545 < num537)
                        {
                            num537 = num545;
                            num535 = num543;
                            num536 = num544;
                            flag19 = true;
                        }
                    }
                    num3 = num542;
                }
            }
            if (!flag19)
            {
                float num16 = 0.5f;
                projectile.tileCollide = false;
                int num17 = 100;
                Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num18 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector3.X;
                float num19 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector3.Y;
                num19 += (float)Main.rand.Next(-10, 21);
                num18 += (float)Main.rand.Next(-10, 21);
                num18 += (float)(60 * -(float)Main.player[projectile.owner].direction);
                num19 -= 60f;
                float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
                float num21 = 18f;

                if (num20 < (float)num17 && Main.player[projectile.owner].velocity.Y == 0f &&
                    projectile.position.Y + (float)projectile.height <= Main.player[projectile.owner].position.Y + (float)Main.player[projectile.owner].height &&
                    !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    if (projectile.velocity.Y < -6f)
                    {
                        projectile.velocity.Y = -6f;
                    }
                }
                if (num20 > 2000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num20 < 50f)
                {
                    if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                    {
                        projectile.velocity *= 0.90f;
                    }
                    num16 = 0.01f;
                }
                else
                {
                    if (num20 < 100f)
                    {
                        num16 = 0.1f;
                    }
                    if (num20 > 300f)
                    {
                        num16 = 1f;
                    }
                    num20 = num21 / num20;
                    num18 *= num20;
                    num19 *= num20;
                }

                if (projectile.velocity.X < num18)
                {
                    projectile.velocity.X = projectile.velocity.X + num16;
                    if (num16 > 0.05f && projectile.velocity.X < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num16;
                    }
                }
                if (projectile.velocity.X > num18)
                {
                    projectile.velocity.X = projectile.velocity.X - num16;
                    if (num16 > 0.05f && projectile.velocity.X > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - num16;
                    }
                }
                if (projectile.velocity.Y < num19)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num16;
                    if (num16 > 0.05f && projectile.velocity.Y < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
                    }
                }
                if (projectile.velocity.Y > num19)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num16;
                    if (num16 > 0.05f && projectile.velocity.Y > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
                    }
                }
            }
            else
            {
                if (projectile.ai[1] == -1f)
                {
                    projectile.ai[1] = 17f;
                }
                if (projectile.ai[1] > 0f)
                {
                    projectile.ai[1] -= 1f;
                }
                if (projectile.ai[1] == 0f)
                {
                    float num550 = 24f; //12
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = num535 - vector43.X;
                    float num552 = num536 - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    if (modPlayer.gOffense)
                    {
                        num550 *= 0.95f;
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                    projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 10f)
                    {
                        projectile.velocity *= 1.05f;
                    }
                }
            }
            if ((double)projectile.velocity.X > 0.25)
            {
                projectile.direction = -1;
            }
            else if ((double)projectile.velocity.X < -0.25)
            {
                projectile.direction = 1;
            }

            if ((double)Math.Abs(projectile.velocity.X) > 0.2)
            {
                projectile.spriteDirection = -projectile.direction;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }
    }
}
