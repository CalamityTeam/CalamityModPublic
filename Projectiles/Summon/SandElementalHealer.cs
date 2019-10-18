using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Summon
{
    public class SandElementalHealer : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oxy's Waifu");
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 98;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20 -
                (NPC.downedGolemBoss ? 5 : 0) -
                (NPC.downedMoonlord ? 5 : 0) -
                (CalamityWorld.downedDoG ? 4 : 0) -
                (CalamityWorld.downedYharon ? 3 : 0);
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<SandElementalHealer>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.sandBoobWaifu && !modPlayer.allWaifus)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.dWaifu = false;
                }
                if (modPlayer.dWaifu)
                {
                    projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 32, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
            }
            if (Main.player[projectile.owner].minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Main.player[projectile.owner].minionDamage);
                projectile.damage = damage2;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 16)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if ((double)Math.Abs(projectile.velocity.X) > 0.2)
            {
                projectile.spriteDirection = -projectile.direction;
            }
            float num636 = 100f; //150
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight(projectile.Center, 0.7f * num, 0.6f * num, 0f * num);
            float num637 = 0.05f;
            for (int num638 = 0; num638 < 1000; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<SandElementalHealer>();
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
            if (Vector2.Distance(player.Center, projectile.Center) > 400f)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
            bool flag26 = false;
            if (!flag26)
            {
                flag26 = projectile.ai[0] == 1f;
            }
            float num650 = 7f; //6
            if (flag26)
            {
                num650 = 18f; //15
            }
            Vector2 center2 = projectile.Center;
            Vector2 vector48 = player.Center - center2 + new Vector2(-250f, -60f); //-60
            float num651 = vector48.Length();
            if (num651 > 200f && num650 < 10f) //200 and 8
            {
                num650 = 10f; //8
            }
            if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                projectile.netUpdate = true;
            }
            if (num651 > 2000f)
            {
                projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
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
                projectile.velocity.X = -0.22f;
                projectile.velocity.Y = -0.12f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 16)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 220f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.localAI[0] < 120f)
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                int num658 = ModContent.ProjectileType<CactusHealOrb>();
                if (projectile.ai[1] == 0f && projectile.localAI[0] >= 120f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner && Main.player[projectile.owner].statLife < Main.player[projectile.owner].statLifeMax2)
                    {
                        Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 6);
                        int num226 = 36;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                            vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                            Vector2 vector7 = vector6 - projectile.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 107, vector7.X * 1.5f, vector7.Y * 1.5f, 100, new Color(0, 200, 0), 1f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].noLight = true;
                            Main.dust[num228].velocity = vector7;
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -6f, num658, 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }
    }
}
