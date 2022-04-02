using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlaguePrincess : ModProjectile
    {
        private bool dust = true;
        private int mode = 0; //0 missiles, 1 mini bees, 2 charging
        private int modeCounter = 0;
        private int AIint = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Virili");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 96;
            projectile.height = 116;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            projectile.minionSlots = projectile.ai[0];

            //bools and crap
            bool correctMinion = projectile.type == ModContent.ProjectileType<PlaguePrincess>();
            player.AddBuff(ModContent.BuffType<PlaguePrincessBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.virili = false;
                }
                if (modPlayer.virili)
                {
                    projectile.timeLeft = 2;
                }
            }

            //dust and flexible damage
            if (dust)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 89, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust = false;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            //framing
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //direction
            if ((double)Math.Abs(projectile.velocity.X) > 0.2)
            {
                projectile.spriteDirection = -projectile.direction;
            }

            //Lighting
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight(projectile.Center, 0f * num, 1.25f * num, 0f * num);

            //change modes every 10 seconds
            modeCounter++;
            if (modeCounter >= 600)
            {
                modeCounter = 0;
                mode++;
                if (mode > 2)
                    mode = 0;
            }

            //anti sticking movement
            projectile.MinionAntiClump();
            //anti-sticking also applies to the player
            float antiStickFloat = 0.05f;
            if (projectile.position.X < player.position.X)
            {
                projectile.velocity.X -= antiStickFloat;
            }
            else
            {
                projectile.velocity.X += antiStickFloat;
            }
            if (projectile.position.Y < player.position.Y)
            {
                projectile.velocity.Y -= antiStickFloat;
            }
            else
            {
                projectile.velocity.Y += antiStickFloat;
            }

            bool cancelAttack = false;
            if (mode == 2)
            {
                if (AIint == 2)
                {
                    projectile.ai[1] += 1f;
                    projectile.extraUpdates = 2;
                    if (projectile.ai[1] > 30f)
                    {
                        projectile.ai[1] = 1f;
                        AIint = 0;
                        projectile.extraUpdates = 1;
                        projectile.numUpdates = 0;
                        projectile.netUpdate = true;
                    }
                    else
                    {
                        cancelAttack = true;
                    }
                }
                else
                {
                    projectile.extraUpdates = 1;
                }
                if (cancelAttack)
                {
                    return;
                }
            }
            if (mode == 0 || mode == 1)
            {
                projectile.extraUpdates = 1;
                if (AIint == 2)
                    AIint = 0;
            }

            float num633 = 1040f;
            float num636 = 400f; //150
            Vector2 targetLocation = projectile.position;
            bool targetFound = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if ((!targetFound && num646 < num633) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        num633 = num646;
                        targetLocation = npc.Center;
                        targetFound = true;
                    }
                }
            }
            if (!targetFound)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if ((!targetFound && num646 < num633) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            num633 = num646;
                            targetLocation = nPC2.Center;
                            targetFound = true;
                        }
                    }
                }
            }

            //head back to player if too far
            if (Vector2.Distance(player.Center, projectile.Center) > 1200f)
            {
                AIint = 1;
                projectile.netUpdate = true;
            }

            if (targetFound && AIint == 0)
            {
                Vector2 targetVector = targetLocation - projectile.Center;
                float targetDist = targetVector.Length();
                targetVector.Normalize();
                if (targetDist > 200f)
                {
                    float scaleFactor2 = 8f;
                    targetVector *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + targetVector) / 41f;
                }
                else if (mode == 2) //charging
                {
                    float scaleFactor3 = 4f;
                    targetVector *= -scaleFactor3;
                    projectile.velocity = (projectile.velocity * 40f + targetVector) / 41f;
                }
                else if (projectile.velocity.Y > -1f)
                    projectile.velocity.Y -= 0.1f;
            }
            else //idle movement
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = AIint == 1;
                }

                //set minion speed
                float speedFloat = 5f; //6
                if (returningToPlayer)
                {
                    speedFloat = 12f; //15
                }
                Vector2 projVector = projectile.Center;
                Vector2 playerVector = player.Center - projVector + new Vector2(0, -60f); //-60
                float playerDist = playerVector.Length();
                if (playerDist > 200f && speedFloat < 6.5f) //200 and 8
                {
                    speedFloat = 6.5f; //8
                }
                if (playerDist < num636 && returningToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    AIint = 0;
                    projectile.netUpdate = true;
                }
                if (playerDist > 2000f) //if too far, teleport to player
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    playerVector.Normalize();
                    playerVector *= speedFloat;
                    projectile.velocity = (projectile.velocity * 40f + playerVector) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.18f;
                    projectile.velocity.Y = -0.08f;
                }
            }

            //increment attack cooldown
            float cooldown = 100f;
            if (mode == 0)
                cooldown = 200f;
            else if (mode == 1)
                cooldown = 110f;
            else if (mode == 2)
                cooldown = 80f;

            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > cooldown)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (AIint == 0)
            {
                if (mode == 0)
                {
                    if (targetFound && projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] += 1f;
                        float scaleFactor4 = 14f;
                        int projType = ModContent.ProjectileType<PrincessMissile>();
                        if (Main.myPlayer == projectile.owner && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetLocation, 0, 0))
                        {
                            Vector2 projVect = targetLocation - projectile.Center;
                            projVect.Normalize();
                            projVect *= scaleFactor4;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projVect.X, projVect.Y, projType, (int)(projectile.damage * 0.6f), 0f, Main.myPlayer, 0f, 0f);
                            projectile.netUpdate = true;
                        }
                    }
                }
                else if (mode == 1)
                {
                    if (targetFound && projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] += 1f;
                        int smallBee = ModContent.ProjectileType<PlagueBeeSmall>();
                        int bigBee = ModContent.ProjectileType<BabyPlaguebringer>();
                        int projType = smallBee;
                        if (player.ownedProjectileCounts[bigBee] < 1 && Main.rand.NextBool(3))
                            projType = bigBee;
                        if (Main.myPlayer == projectile.owner && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetLocation, 0, 0))
                        {
                            for (int beeIndex = 0; beeIndex < (projType == bigBee ? 1 : Main.rand.Next(1,5)); beeIndex++)
                            {
                                Vector2 projVect2 = targetLocation - projectile.Center;
                                projVect2.Normalize();
                                float SpeedX = projVect2.X + (float)Main.rand.Next(-30, 31) * 0.05f;
                                float SpeedY = projVect2.Y + (float)Main.rand.Next(-30, 31) * 0.05f;
                                int bee = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY, projType, (int)(projectile.damage * 0.8f), 0f, Main.myPlayer, 0f, 0f);
                                if (projType == bigBee)
                                {
                                    Main.projectile[bee].frame = 2;
                                }
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
                else if (mode == 2)
                {
                    if (projectile.ai[1] == 0f && targetFound && num633 < 500f)
                    {
                        projectile.ai[1] += 1f;
                        if (Main.myPlayer == projectile.owner)
                        {
                            AIint = 2;
                            Vector2 targetVect = targetLocation - projectile.Center;
                            targetVect.Normalize();
                            projectile.velocity = targetVect * 8f;
                            projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override bool CanDamage()
        {
            return mode == 2;
        }
    }
}
