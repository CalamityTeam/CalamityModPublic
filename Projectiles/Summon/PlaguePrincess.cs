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
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 116;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            Projectile.minionSlots = Projectile.ai[0];

            //bools and crap
            bool correctMinion = Projectile.type == ModContent.ProjectileType<PlaguePrincess>();
            player.AddBuff(ModContent.BuffType<ViriliBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.virili = false;
                }
                if (modPlayer.virili)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //dust and flexible damage
            if (dust)
            {
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 89, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust = false;
            }

            //framing
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //direction
            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }

            //Lighting
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 0f * num, 1.25f * num, 0f * num);

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
            Projectile.MinionAntiClump();
            //anti-sticking also applies to the player
            float antiStickFloat = 0.05f;
            if (Projectile.position.X < player.position.X)
            {
                Projectile.velocity.X -= antiStickFloat;
            }
            else
            {
                Projectile.velocity.X += antiStickFloat;
            }
            if (Projectile.position.Y < player.position.Y)
            {
                Projectile.velocity.Y -= antiStickFloat;
            }
            else
            {
                Projectile.velocity.Y += antiStickFloat;
            }

            bool cancelAttack = false;
            if (mode == 2)
            {
                if (AIint == 2)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.extraUpdates = 2;
                    if (Projectile.ai[1] > 30f)
                    {
                        Projectile.ai[1] = 1f;
                        AIint = 0;
                        Projectile.extraUpdates = 1;
                        Projectile.numUpdates = 0;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        cancelAttack = true;
                    }
                }
                else
                {
                    Projectile.extraUpdates = 1;
                }
                if (cancelAttack)
                {
                    return;
                }
            }
            if (mode == 0 || mode == 1)
            {
                Projectile.extraUpdates = 1;
                if (AIint == 2)
                    AIint = 0;
            }

            float num633 = 1040f;
            float num636 = 400f; //150
            Vector2 targetLocation = Projectile.position;
            bool targetFound = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
                    if ((!targetFound && num646 < num633) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
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
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if ((!targetFound && num646 < num633) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            num633 = num646;
                            targetLocation = nPC2.Center;
                            targetFound = true;
                        }
                    }
                }
            }

            //head back to player if too far
            if (Vector2.Distance(player.Center, Projectile.Center) > 1200f)
            {
                AIint = 1;
                Projectile.netUpdate = true;
            }

            if (targetFound && AIint == 0)
            {
                Vector2 targetVector = targetLocation - Projectile.Center;
                float targetDist = targetVector.Length();
                targetVector.Normalize();
                if (targetDist > 200f)
                {
                    float scaleFactor2 = 8f;
                    targetVector *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + targetVector) / 41f;
                }
                else if (mode == 2) //charging
                {
                    float scaleFactor3 = 4f;
                    targetVector *= -scaleFactor3;
                    Projectile.velocity = (Projectile.velocity * 40f + targetVector) / 41f;
                }
                else if (Projectile.velocity.Y > -1f)
                    Projectile.velocity.Y -= 0.1f;
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
                Vector2 projVector = Projectile.Center;
                Vector2 playerVector = player.Center - projVector + new Vector2(0, -60f); //-60
                float playerDist = playerVector.Length();
                if (playerDist > 200f && speedFloat < 6.5f) //200 and 8
                {
                    speedFloat = 6.5f; //8
                }
                if (playerDist < num636 && returningToPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    AIint = 0;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f) //if too far, teleport to player
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    playerVector.Normalize();
                    playerVector *= speedFloat;
                    Projectile.velocity = (Projectile.velocity * 40f + playerVector) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.18f;
                    Projectile.velocity.Y = -0.08f;
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

            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > cooldown)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (AIint == 0)
            {
                if (mode == 0)
                {
                    if (targetFound && Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        float scaleFactor4 = 14f;
                        int projType = ModContent.ProjectileType<PrincessMissile>();
                        if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetLocation, 0, 0))
                        {
                            Vector2 projVect = targetLocation - Projectile.Center;
                            projVect.Normalize();
                            projVect *= scaleFactor4;
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projVect, projType, (int)(Projectile.damage * 0.6f), 0f, Main.myPlayer, 0f, 0f);
                            if (Main.projectile.IndexInRange(p))
                                Main.projectile[p].originalDamage = (int)(Projectile.originalDamage * 0.6);
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else if (mode == 1)
                {
                    if (targetFound && Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        int smallBee = ModContent.ProjectileType<PlagueBeeSmall>();
                        int bigBee = ModContent.ProjectileType<BabyPlaguebringer>();
                        int projType = smallBee;
                        if (player.ownedProjectileCounts[bigBee] < 1 && Main.rand.NextBool(3))
                            projType = bigBee;
                        if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetLocation, 0, 0))
                        {
                            for (int beeIndex = 0; beeIndex < (projType == bigBee ? 1 : Main.rand.Next(1,5)); beeIndex++)
                            {
                                Vector2 projVect2 = targetLocation - Projectile.Center;
                                projVect2.Normalize();
                                float SpeedX = projVect2.X + (float)Main.rand.Next(-30, 31) * 0.05f;
                                float SpeedY = projVect2.Y + (float)Main.rand.Next(-30, 31) * 0.05f;
                                int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, SpeedX, SpeedY, projType, (int)(Projectile.damage * 0.8f), 0f, Main.myPlayer, 0f, 0f);
                                if (projType == bigBee)
                                {
                                    Main.projectile[bee].frame = 2;
                                }
                                if (Main.projectile.IndexInRange(bee))
                                    Main.projectile[bee].originalDamage = (int)(Projectile.originalDamage * 0.8f);
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
                else if (mode == 2)
                {
                    if (Projectile.ai[1] == 0f && targetFound && num633 < 500f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            AIint = 2;
                            Vector2 targetVect = targetLocation - Projectile.Center;
                            targetVect.Normalize();
                            Projectile.velocity = targetVect * 8f;
                            Projectile.netUpdate = true;
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

        public override bool? CanDamage() => mode == 2;
    }
}
