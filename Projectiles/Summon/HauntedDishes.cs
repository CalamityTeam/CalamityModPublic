using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HauntedDishes : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HauntedDishes");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 19;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = projectile.Calamity();
            if (dust == 0f)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int num227 = 0; num227 < dustAmt; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, 7, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            bool projTypeCheck = projectile.type == ModContent.ProjectileType<HauntedDishes>();
            player.AddBuff(ModContent.BuffType<HauntedDishesBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    modPlayer.hauntedDishes = false;
                }
                if (modPlayer.hauntedDishes)
                {
                    projectile.timeLeft = 2;
                }
            }
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int num1 = 60 + 30 * projectile.minionPos;
            if (projectile.lavaWet)
            {
                projectile.ai[0] = 1f;
                projectile.ai[1] = 0f;
            }
            int num2 = 10;
            int num3 = 40 * (projectile.minionPos + 1) * player.direction;
            if (player.position.X + (float) (player.width / 2) < projectile.position.X + (float) (projectile.width / 2) - (float) num2 + (float) num3)
                flag1 = true;
            else if (player.position.X + (float) (player.width / 2) > projectile.position.X + (float) (projectile.width / 2) + (float) num2 + (float) num3)
                flag2 = true;

            if (projectile.ai[1] == 0f)
            {
                int conflict1 = 500;
                conflict1 += 40 * projectile.minionPos;
                if (projectile.localAI[0] > 0f)
                    conflict1 += 500;
                Vector2 vector2 = new Vector2(projectile.position.X + (float) projectile.width * 0.5f, projectile.position.Y + (float) projectile.height * 0.5f);
                float playerX = player.position.X + (float) (player.width / 2) - vector2.X;
                float playerY = player.position.Y + (float) (player.height / 2) - vector2.Y;
                float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                if (playerDist > 1500f)
                {
                    projectile.ai[0] = 1f;
                }
                if (playerDist > 2000f) //teleport to player if too far
                {
                    projectile.position.X = player.position.X + (float) (player.width / 2) - (float) (projectile.width / 2);
                    projectile.position.Y = player.position.Y + (float) (player.height / 2) - (float) (projectile.height / 2);
                }
            }
            if (projectile.ai[0] != 0f) //flying back to the player
            {
                projectile.tileCollide = false;
                float npcDetectRange = 1200f;
                bool npcFound = false;
                int num6 = -1;
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc2 = Main.npc[index];
                    if (npc2.CanBeChasedBy((object) projectile, false))
                    {
                        float npcX = npc2.position.X + (float) (npc2.width / 2);
                        float npcY = npc2.position.Y + (float) (npc2.height / 2);
                        float npcDist = (float) Math.Abs(player.position.X + (float) (player.width / 2) - npcX) + (float) Math.Abs(player.position.Y + (float) (player.height / 2) - npcY);
                        if (npcDist < npcDetectRange)
                        {
                            if (Collision.CanHit(projectile.position, projectile.width, projectile.height, npc2.position, npc2.width, npc2.height))
                                num6 = index;
                            npcFound = true;
                            break;
                        }
                    }
                }

                //return to normal if npc found
                if (npcFound && num6 >= 0)
                    projectile.ai[0] = 0f;

                Vector2 vector2 = new Vector2(projectile.position.X + (float) projectile.width * 0.5f, projectile.position.Y + (float) projectile.height * 0.5f);
                float xDist = player.position.X + (float) (player.width / 2) - vector2.X;
                xDist -= (float) (40 * player.direction);
                if (!npcFound)
                    xDist -= (float) (40 * projectile.minionPos * player.direction);
                float yDist = player.position.Y + (float) (player.height / 2) - vector2.Y;
                yDist -= 60f;
                float playerDist2 = (float) Math.Sqrt(xDist * xDist + yDist * yDist);
                float num11 = 12f;
                float num12 = playerDist2;
                float conflict2 = 0.4f;
                if (num11 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                    num11 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

                //if close enough to the player and has tile to stand on, return to normal
                if (playerDist2 < 100f && player.velocity.Y == 0f && (projectile.position.Y + (float) projectile.height <= player.position.Y + (float) player.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height)))
                {
                    projectile.ai[0] = 0f;
                    if (projectile.velocity.Y < -6f)
                        projectile.velocity.Y = -6f;
                }
                if (playerDist2 > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (playerDist2 < 50f)
                {
                    if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                    {
                        projectile.velocity *= 0.99f;
                    }
                    conflict2 = 0.01f;
                }
                else
                {
                    if (playerDist2 < 100f)
                    {
                        conflict2 = 0.1f;
                    }
                    if (playerDist2 > 300f)
                    {
                        conflict2 = 1f;
                    }
                    playerDist2 = num12 / playerDist2;
                    xDist *= playerDist2;
                    yDist *= playerDist2;
                }
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X = projectile.velocity.X + conflict2;
                    if (conflict2 > 0.05f && projectile.velocity.X < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + conflict2;
                    }
                }
                if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X = projectile.velocity.X - conflict2;
                    if (conflict2 > 0.05f && projectile.velocity.X > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - conflict2;
                    }
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y + conflict2;
                    if (conflict2> 0.05f && projectile.velocity.Y < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + conflict2 * 2f;
                    }
                }
                if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y - conflict2;
                    if (conflict2 > 0.05f && projectile.velocity.Y > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - conflict2 * 2f;
                    }
                }
                if (projectile.frame < 15)
                {
                    projectile.frame = 15;
                }
                else
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 3)
                    {
                        projectile.frame++;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 19)
                    {
                        projectile.frame = 15;
                    }
                }
                if (projectile.velocity.X > 0.5f)
                    projectile.spriteDirection = 1;
                else if (projectile.velocity.X < -0.5f)
                    projectile.spriteDirection = -1;
                projectile.rotation = projectile.spriteDirection != 1 ? (float) Math.Atan2((double) projectile.velocity.Y, (double) projectile.velocity.X) + 3.14f : (float) Math.Atan2((double) projectile.velocity.Y, (double) projectile.velocity.X);
            }
            else
            {
                float conflict3 = (float) (40 * projectile.minionPos);
                float attackCooldown = 30f;
                int num4 = 60;
                --projectile.localAI[0];
                if (projectile.localAI[0] < 0f)
                    projectile.localAI[0] = 0f;
                if (projectile.ai[1] > 0f)
                {
                    --projectile.ai[1];
                }
                else
                {
                    float num5 = projectile.position.X;
                    float num6 = projectile.position.Y;
                    float num7 = 100000f;
                    float num8 = num7;
                    int num9 = -1;
                    NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                    if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object) projectile, false))
                    {
                        float num10 = minionAttackTargetNpc.position.X + (float) (minionAttackTargetNpc.width / 2);
                        float num11 = minionAttackTargetNpc.position.Y + (float) (minionAttackTargetNpc.height / 2);
                        float num12 = Math.Abs(projectile.position.X + (float) (projectile.width / 2) - num10) + Math.Abs(projectile.position.Y + (float) (projectile.height / 2) - num11);
                        if (num12 < num7)
                        {
                            if (num9 == -1 && num12 <= num8)
                            {
                                num8 = num12;
                                num5 = num10;
                                num6 = num11;
                            }
                            if (Collision.CanHit(projectile.position, projectile.width, projectile.height, minionAttackTargetNpc.position, minionAttackTargetNpc.width, minionAttackTargetNpc.height))
                            {
                                num7 = num12;
                                num5 = num10;
                                num6 = num11;
                                num9 = minionAttackTargetNpc.whoAmI;
                            }
                        }
                    }
                    if (num9 == -1)
                    {
                        for (int index = 0; index < Main.maxNPCs; ++index)
                        {
                            if (Main.npc[index].CanBeChasedBy((object) projectile, false))
                            {
                                float num10 = Main.npc[index].position.X + (float) (Main.npc[index].width / 2);
                                float num11 = Main.npc[index].position.Y + (float) (Main.npc[index].height / 2);
                                float num12 = Math.Abs(projectile.position.X + (float) (projectile.width / 2) - num10) + Math.Abs(projectile.position.Y + (float) (projectile.height / 2) - num11);
                                if (num12 < num7)
                                {
                                    if (num9 == -1 && num12 <= num8)
                                    {
                                        num8 = num12;
                                        num5 = num10;
                                        num6 = num11;
                                    }
                                    if (Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height))
                                    {
                                        num7 = num12;
                                        num5 = num10;
                                        num6 = num11;
                                        num9 = index;
                                    }
                                }
                            }
                        }
                    }
                    if (num9 == -1 && num8 < num7)
                        num7 = num8;
                    float num13 = 400f;
                    if ((double) projectile.position.Y > Main.worldSurface * 16.0)
                        num13 = 200f;
                    if (num7 < num13 + conflict3 && num9 == -1)
                    {
                        float num10 = num5 - (projectile.position.X + (float) (projectile.width / 2));
                        if (num10 < -5f)
                        {
                            flag1 = true;
                            flag2 = false;
                        }
                        else if (num10 > 5f)
                        {
                            flag2 = true;
                            flag1 = false;
                        }
                    }
                    else if (num9 >= 0 && num7 < 800f + conflict3)
                    {
                        projectile.localAI[0] = (float) num4;
                        float num10 = num5 - (projectile.position.X + (float) (projectile.width / 2));
                        if (num10 > 300f || num10 < -300f)
                        {
                            if (num10 < -50f)
                            {
                                flag1 = true;
                                flag2 = false;
                            }
                            else if (num10 > 50f)
                            {
                                flag2 = true;
                                flag1 = false;
                            }
                        }
                        else if (projectile.owner == Main.myPlayer)
                        {
                            projectile.ai[1] = attackCooldown;
                            double num11 = 12.0;
                            Vector2 vector2 = new Vector2(projectile.Center.X, projectile.Center.Y - 8f);
                            float num12 = num5 - vector2.X + Main.rand.NextFloat(-6f, 6f);
                            float num14 = (float) ((double) (Math.Abs(num12) * 0.1f) * (double) Main.rand.Next(0, 100) * (1.0 / 1000.0));
                            float num15 = num6 - vector2.Y + Main.rand.NextFloat(-6f, 6f) - num14;
                            double num16 = Math.Sqrt((double) num12 * (double) num12 + (double) num15 * (double) num15);
                            float num17 = (float) (num11 / num16);
                            float SpeedX = num12 * num17;
                            float SpeedY = num15 * num17;
                            int damage = projectile.damage;
                            int Type = ModContent.ProjectileType<PlateProjectile>();
                            int index = Projectile.NewProjectile(vector2.X, vector2.Y, SpeedX * 2f, SpeedY * 2f, Type, damage, projectile.knockBack, projectile.owner, 0f, 0f);
                            if (SpeedX < 0f)
                                projectile.direction = -1;
                            if (SpeedX > 0f)
                                projectile.direction = 1;
                            projectile.netUpdate = true;
                        }
                    }
                }
                Vector2 vector2_1 = Vector2.Zero;
                bool flag7 = false;
                if (projectile.ai[1] != 0f)
                {
                    flag1 = false;
                    flag2 = false;
                }
                else if (projectile.localAI[0] == 0f)
                    projectile.direction = player.direction;
                if (!flag7)
                    projectile.rotation = 0f;
                projectile.tileCollide = true;
                float num18 = 0.08f;
                float num19 = 6.5f;
                num19 = 6f;
                num18 = 0.2f;
                if (num19 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    num19 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    num18 = 0.3f;
                }
                if (flag1)
                {
                    if (projectile.velocity.X > -3.5f)
                        projectile.velocity.X -= num18;
                    else
                        projectile.velocity.X -= num18 * 0.25f;
                }
                else if (flag2)
                {
                    if (projectile.velocity.X < 3.5f)
                        projectile.velocity.X += num18;
                    else
                        projectile.velocity.X += num18 * 0.25f;
                }
                else
                {
                    projectile.velocity.X *= 0.9f;
                    if (projectile.velocity.X >= -num18 && projectile.velocity.X <= num18)
                        projectile.velocity.X = 0f;
                }
                if (flag1 | flag2)
                {
                    int conflict4 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
                    int j = (int) ((double) projectile.position.Y + (double) (projectile.height / 2)) / 16;
                    if (flag1)
                        --conflict4;
                    if (flag2)
                        ++conflict4;
                    if (WorldGen.SolidTile(conflict4 + (int) projectile.velocity.X, j))
                        flag4 = true;
                }
                if (player.position.Y + player.height - 8f > projectile.position.Y + projectile.height)
                    flag3 = true;
                Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
                if (projectile.velocity.Y == 0f)
                {
                    if (!flag3 && ((double) projectile.velocity.X < 0.0 || (double) projectile.velocity.X > 0.0))
                    {
                        int i = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
                        int j = (int) ((double) projectile.position.Y + (double) (projectile.height / 2)) / 16 + 1;
                        if (flag1)
                            --i;
                        if (flag2)
                            ++i;
                        WorldGen.SolidTile(i, j);
                    }
                    if (flag4)
                    {
                        int i1 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
                        int j = (int) ((double) projectile.position.Y + (double) projectile.height) / 16 + 1;
                        if (WorldGen.SolidTile(i1, j) || Main.tile[i1, j].halfBrick() || ((int) Main.tile[i1, j].slope() > 0 || projectile.type == 200))
                        {
                            if (projectile.type == 200)
                            {
                                projectile.velocity.Y = -3.1f;
                            }
                            else
                            {
                                try
                                {
                                    int conflict5 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
                                    int conflict7 = (int) ((double) projectile.position.Y + (double) (projectile.height / 2)) / 16;
                                    if (flag1)
                                        --conflict5;
                                    if (flag2)
                                        ++conflict5;
                                    int i2 = conflict5 + (int) projectile.velocity.X;
                                    if (!WorldGen.SolidTile(i2, conflict7 - 1) && !WorldGen.SolidTile(i2, conflict7 - 2))
                                        projectile.velocity.Y = -5.1f;
                                    else if (!WorldGen.SolidTile(i2, conflict7 - 2))
                                        projectile.velocity.Y = -7.1f;
                                    else if (WorldGen.SolidTile(i2, conflict7 - 5))
                                        projectile.velocity.Y = -11.1f;
                                    else if (WorldGen.SolidTile(i2, conflict7 - 4))
                                        projectile.velocity.Y = -10.1f;
                                    else
                                        projectile.velocity.Y = -9.1f;
                                }
                                catch
                                {
                                    projectile.velocity.Y = -9.1f;
                                }
                            }
                        }
                    }
                    else if (projectile.type == 266 && flag1 | flag2)
                        projectile.velocity.Y -= 6f;
                }
                if (projectile.velocity.X > num19)
                    projectile.velocity.X = num19;
                if (projectile.velocity.X < -num19)
                    projectile.velocity.X = -num19;
                if (projectile.velocity.X < 0f)
                    projectile.direction = -1;
                if (projectile.velocity.X > 0f)
                    projectile.direction = 1;
                if (projectile.velocity.X > num18 & flag2)
                    projectile.direction = 1;
                if (projectile.velocity.X < -num18 & flag1)
                    projectile.direction = -1;
                if (projectile.direction == -1)
                    projectile.spriteDirection = -1;
                if (projectile.direction == 1)
                    projectile.spriteDirection = 1;
                if (projectile.ai[1] > 0f)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.localAI[1] = 1f;
                        projectile.frame = 9;
                    }
                    if (projectile.frame >= 9 && projectile.frame <= 14)
                    {
                        projectile.frameCounter++;
                        if (projectile.frameCounter > 8)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame == 14)
                            projectile.frame = 9;
                    }
                }
                else if (projectile.velocity.Y == 0f)
                {
                    projectile.localAI[1] = 0f;
                    if (projectile.velocity.X == 0f)
                    {
                        projectile.frameCounter++;
                        if (projectile.frameCounter > 4)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame >= 3)
                        {
                            projectile.frame = 0;
                        }
                    }
                    else if (projectile.velocity.X < -0.8f || projectile.velocity.X > 0.8f)
                    {
                        projectile.frameCounter = projectile.frameCounter + (int) Math.Abs(projectile.velocity.X);
                        projectile.frameCounter++;
                        if (projectile.frameCounter > 20)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame < 4)
                            projectile.frame = 3;
                        if (projectile.frame >= 9)
                            projectile.frame = 3;
                    }
                    else
                    {
                        projectile.frameCounter++;
                        if (projectile.frameCounter > 4)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame >= 3)
                        {
                            projectile.frame = 0;
                        }
                    }
                }
                else if (projectile.velocity.Y < 0f)
                {
                    projectile.frameCounter = 0;
                    projectile.frame = 3;
                }
                else if (projectile.velocity.Y > 0f)
                {
                    projectile.frameCounter = 0;
                    projectile.frame = 3;
                }
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 10f)
                    projectile.velocity.Y = 10f;
                Vector2 velocity = projectile.velocity;
            }
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void Kill(int timeLeft)
        {
            int index = Gore.NewGore(new Vector2(projectile.position.X - (float) (projectile.width / 2), projectile.position.Y - (float) (projectile.height / 2)), new Vector2(0.0f, 0.0f), Main.rand.Next(61, 64), projectile.scale);
            Main.gore[index].velocity *= 0.1f;
        }
    }
}
