using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.RegularEnemies
{
    public static class SlimeAI
    {
        public static void ChooseRandomItem(out int dropItem)
        {
            // Use a fallback of -1.
            dropItem = -1;

            switch (Main.rand.Next(4))
            {
                // Potions.
                case 0:
                    int rand = Main.rand.Next(7);
                    if (rand == 0)
                    {
                        dropItem = ItemID.SwiftnessPotion;
                    }
                    else if (rand == 1)
                    {
                        dropItem = ItemID.IronskinPotion;
                    }
                    else if (rand == 2)
                    {
                        dropItem = ItemID.SpelunkerPotion;
                    }
                    else if (rand == 3)
                    {
                        dropItem = ItemID.MiningPotion;
                    }
                    else if (Main.netMode != NetmodeID.SinglePlayer && Main.rand.NextBool())
                    {
                        dropItem = ItemID.WormholePotion;
                    }
                    else
                    {
                        dropItem = ItemID.RecallPotion;
                    }
                    break;

                // Misc Items.
                case 1:
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            dropItem = ItemID.Torch;
                            break;
                        case 1:
                            dropItem = ItemID.Bomb;
                            break;
                        case 2:
                            dropItem = ItemID.Rope;
                            break;
                        case 3:
                            dropItem = ItemID.Heart;
                            break;
                    }
                    break;

                // Ores.
                case 2:
                    if (Main.rand.NextBool())
                    {
                        dropItem = Main.rand.Next(ItemID.IronOre, ItemID.SilverOre + 1);
                    }
                    else
                    {
                        dropItem = Main.rand.Next(ItemID.TinOre, ItemID.PlatinumOre + 1);
                    }
                    break;

                // Coins.
                case 3:
                    dropItem = Main.rand.Next(ItemID.CopperCoin, ItemID.GoldCoin + 1);
                    break;
            }
        }

        public static bool BuffedSlimeAI(NPC npc, Mod mod)
        {
            bool isLavaSlime = npc.type == NPCID.LavaSlime || npc.type == ModContent.NPCType<InfernalCongealment>();
            bool canShootProjectile = npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.SlimeSpiked || npc.type == NPCID.SpikedJungleSlime;
            int projectileShootType = -1;
            float projectileShootSpeedFactor = 1f;

            if (npc.type == NPCID.SpikedIceSlime)
                projectileShootType = ProjectileID.IceSpike;
            if (npc.type == NPCID.SlimeSpiked)
                projectileShootType = ProjectileID.SpikedSlimeSpike;
            if (npc.type == NPCID.SpikedJungleSlime)
            {
                projectileShootType = ProjectileID.JungleSpike;
                projectileShootSpeedFactor *= 0.6f;
            }

            ref float jumpDelay = ref npc.ai[0];
            ref float dropItemID = ref npc.ai[1];
            ref float targetResetCountdown = ref npc.ai[2];
            ref float projectileShootCountdown = ref npc.localAI[0];

            if (npc.type == NPCID.BlueSlime && (dropItemID == 1f || dropItemID == 2f || dropItemID == 3f))
                dropItemID = -1f;

            // Determine what the slime holds, if anything. This does not apply to slimes that are have no money tp drop.
            if (npc.type == NPCID.BlueSlime && dropItemID == 0f && Main.netMode != NetmodeID.MultiplayerClient && npc.value > 0f)
            {
                dropItemID = -1f;

                if (Main.rand.NextBool(20))
                {
                    ChooseRandomItem(out int dropItem);
                    dropItemID = dropItem;
                    npc.netUpdate = true;
                }
            }

            // Decide colors for rainbow slimes.
            if (npc.type == NPCID.RainbowSlime)
            {
                Lighting.AddLight(npc.Center / 16, Main.DiscoColor.ToVector3() * -1f);
                npc.color.R = (byte)Main.DiscoR;
                npc.color.G = (byte)Main.DiscoG;
                npc.color.B = (byte)Main.DiscoB;
                npc.color.A = 100;
                npc.alpha = 175;
            }

            // Have corrupt slimes emit demonite dust.
            if (npc.type == NPCID.CorruptSlime && Main.rand.NextBool(30))
            {
                Dust demonite = Dust.NewDustDirect(npc.position, npc.width, npc.height, 14, 0f, 0f, npc.alpha, npc.color, 1f);
                demonite.velocity *= 0.3f;
            }

            // Have ice slimes emit snow dust.
            if ((npc.type == NPCID.IceSlime || npc.type == NPCID.SpikedIceSlime) && Main.rand.NextBool(10))
            {
                Dust snow = Dust.NewDustDirect(npc.position, npc.width, npc.height, 76, 0f, 0f, 0, default, 1f);
                snow.noGravity = true;
                snow.velocity *= 0.1f;
            }

            if (isLavaSlime)
            {
                // Emit orange light.
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 1f, 0.3f, 0.1f);

                // And fire. dust.
                int idx = Dust.NewDust(npc.position, npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default, 1.7f);
                Main.dust[idx].noGravity = true;
            }

            // Handle projectile shoot logic, if applicable.
            if (canShootProjectile)
            {
                // Decrement the projectile shoot countdown until it's ready.
                if (projectileShootCountdown > 0f)
                    projectileShootCountdown--;

                float distanceFromTarget = npc.Distance(Main.player[npc.target].Center);
                bool noTilesInWayOfTarget = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);

                // If not in water, the target can be hit by this NPC, and they're close and in a line of sight, release projectiles.
                if (!npc.wet && !Main.player[npc.target].npcTypeNoAggro[npc.type] && noTilesInWayOfTarget && distanceFromTarget < 200f && npc.velocity.Y == 0f)
                {
                    jumpDelay = -40f;
                    npc.velocity.X *= 0.9f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && projectileShootCountdown <= 0f)
                    {
                        var source = npc.GetSource_FromAI();
                        if (distanceFromTarget < 120f)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 spikeShootVelocity = new Vector2(i - 2, -4f);
                                spikeShootVelocity *= Main.rand.NextVector2Square(0.75f, 1.25f);
                                spikeShootVelocity.Normalize();
                                spikeShootVelocity *= Main.rand.NextFloat(3.5f, 4.5f) * projectileShootSpeedFactor;
                                int proj = Projectile.NewProjectile(source, npc.Center, spikeShootVelocity, projectileShootType, 9, 0f, Main.myPlayer, 0f, 0f);
                                if (CalamityWorld.death)
                                    Main.projectile[proj].extraUpdates += 1;
                                projectileShootCountdown = 30f;
                            }
                        }
                        else
                        {
                            Vector2 velocity = npc.SafeDirectionTo(Main.player[npc.target].Center - Vector2.UnitY * Main.rand.NextFloat(200f)) * projectileShootSpeedFactor * (CalamityWorld.death ? 4f : 6f);
                            int proj = Projectile.NewProjectile(source, npc.Center, velocity, projectileShootType, 9, 0f, Main.myPlayer, 0f, 0f);
                            if (CalamityWorld.death)
                            {
                                Main.projectile[proj].extraUpdates += 1;
                                Main.projectile[proj].timeLeft = 1200;
                            }
                            projectileShootCountdown = 50f;
                        }
                    }
                }
            }

            // Decrement the target reset counter.
            if (targetResetCountdown > 1f)
                targetResetCountdown--;

            // Rise to the top of water.
            if (npc.wet)
                DoWaterHoverBehavior(npc, isLavaSlime, ref targetResetCountdown);

            npc.aiAction = 0;

            // Initialize with short jumps.
            if (targetResetCountdown == 0f)
            {
                jumpDelay = -100f;
                targetResetCountdown = 1f;
                npc.TargetClosest();
            }

            if (npc.velocity.Y == 0f)
            {
                // Slide out of blocks if stuck.
                if (npc.collideY && npc.oldVelocity.Y != 0f && Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.position.X -= npc.velocity.X + npc.direction;

                if (npc.ai[3] == npc.position.X)
                {
                    npc.direction *= -1;
                    targetResetCountdown = 200f;
                }

                npc.ai[3] = 0f;

                // Slow down horizontally until stopping.
                npc.velocity.X *= 0.8f;
                if (Math.Abs(npc.velocity.X) < 0.1f)
                    npc.velocity.X = 0f;

                // Slimes jump more quickly overall when the slime rain event is ongoing.
                jumpDelay += (Main.slimeRain ? 4f : 3f) * (CalamityWorld.death ? 2f : 1f);

                if (npc.type == NPCID.HoppinJack || npc.type == NPCID.GoldenSlime)
                    jumpDelay += 10f;

                if (isLavaSlime)
                    jumpDelay += 2f;

                if (npc.type == NPCID.DungeonSlime || npc.type == ModContent.NPCType<CryoSlime>() || npc.type == ModContent.NPCType<CrimulanBlightSlime>() ||
                    npc.type == ModContent.NPCType<EbonianBlightSlime>())
                {
                    jumpDelay += 3f;
                }

                if (npc.type == NPCID.RainbowSlime)
                    jumpDelay += 2f;

                if (npc.type == NPCID.IlluminantSlime)
                    jumpDelay += 2f;

                if (npc.type == NPCID.Crimslime)
                    jumpDelay += 1f;

                // The fuck? This is from vanilla, presumably. I'll leave it alone in the event that it's some dumb spaghetti.
                if (npc.type == NPCID.CorruptSlime)
                    jumpDelay += npc.scale >= 0f ? 4f : 1f;

                int jumpType = 0;
                if (jumpDelay >= 0f)
                    jumpType = 1;

                if (jumpDelay >= -1000f && jumpDelay <= -500f)
                    jumpType = 2;

                if (jumpDelay >= -2000f && jumpDelay <= -1500f)
                    jumpType = 3;

                if (jumpType > 0)
                    DoJump(npc, jumpType, isLavaSlime, ref targetResetCountdown, out jumpDelay);

                else if (jumpDelay >= -30f)
                {
                    npc.aiAction = 1;
                    return false;
                }
            }
            else if (npc.target < Main.maxPlayers && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
            {
                if (npc.collideX && Math.Abs(npc.velocity.X) == 0.2f)
                {
                    npc.position.X -= 1.4f * npc.direction;
                }
                if (npc.collideY && npc.oldVelocity.Y != 0f && Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.position.X -= npc.velocity.X + npc.direction;
                }
                if ((npc.direction == -1 && npc.velocity.X < 0.01) || (npc.direction == 1 && npc.velocity.X > -0.01))
                {
                    npc.velocity.X += 0.2f * (float)npc.direction;
                    return false;
                }
                npc.velocity.X *= 0.93f;
            }
            return false;
        }

        public static void DoJump(NPC npc, int jumpType, bool isLavaSlime, ref float targetResetCountdown, out float jumpDelay)
        {
            if (targetResetCountdown == 1f)
                npc.TargetClosest();

            float verticalJumpSpeed = 4f;
            float horizontalJumpSpeed = 4f;
            if (Main.slimeRain)
            {
                verticalJumpSpeed = 5f;
                horizontalJumpSpeed = 5f;
            }

            // Long jumps go further into the air.
            if (jumpType == 3)
            {
                verticalJumpSpeed *= 2.5f;
                horizontalJumpSpeed++;
                if (isLavaSlime)
                    verticalJumpSpeed += 2f;
            }

            // Perform the jump.
            npc.velocity.Y = -verticalJumpSpeed;
            npc.velocity.X += horizontalJumpSpeed * npc.direction;

            // Cycle between jump type 1, 2, and 3.
            if (jumpType == 3)
            {
                jumpDelay = -200f;
                npc.ai[3] = npc.position.X;
            }
            else if (jumpType == 1)
                jumpDelay = -1120f;
            else
                jumpDelay = -2120f;

            // Certain slimes have overall larger jumps.
            if (npc.type == NPCID.ToxicSludge || npc.type == ModContent.NPCType<PerennialSlime>() || npc.type == ModContent.NPCType<BloomSlime>() || npc.type == ModContent.NPCType<IrradiatedSlime>())
            {
                npc.velocity.X *= 1.2f;
                npc.velocity.Y *= 1.3f;
            }
            npc.netUpdate = true;
        }

        public static void DoWaterHoverBehavior(NPC npc, bool isLavaSlime, ref float targetResetCountdown)
        {
            // Move up if tiles are hit on the Y axis.
            if (npc.collideY)
                npc.velocity.Y = -4f;

            if (npc.velocity.Y < 0f && npc.ai[3] == npc.position.X)
            {
                npc.direction *= -1;
                targetResetCountdown = 200f;
            }

            if (npc.velocity.Y > 0f)
                npc.ai[3] = npc.position.X;

            float riseSpeed = 0.6f;
            float maxRiseSpeed = 6f;
            if (isLavaSlime)
            {
                riseSpeed += 0.2f;
                maxRiseSpeed += 10f;
            }

            // Grind downward vertical movement to a halt if present.
            if (npc.velocity.Y > 2f)
                npc.velocity.Y *= 0.9f;

            // Move upwards more quickly if rising upward and the slime is lava in nature.
            else if (npc.directionY < 0 && isLavaSlime)
                npc.velocity.Y -= 1.2f;

            // Do typical rise movement.
            npc.velocity.Y -= riseSpeed;
            if (npc.velocity.Y < -maxRiseSpeed)
                npc.velocity.Y = -maxRiseSpeed;

            if (targetResetCountdown == 1f)
                npc.TargetClosest();
        }
    }
}
