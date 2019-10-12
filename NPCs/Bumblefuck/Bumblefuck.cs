using CalamityMod.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Bumblefuck
{
    [AutoloadBossHead]
    public class Bumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bumblebirb");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 32f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 160;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 40;
            npc.lifeMax = CalamityWorld.revenge ? 252500 : 227500;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 302500;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1000000 : 900000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[mod.BuffType("ExoFreeze")] = false;
            npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
            npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
            npc.buffImmune[mod.BuffType("DemonFlames")] = false;
            npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
            npc.buffImmune[mod.BuffType("Nightwither")] = false;
            npc.buffImmune[mod.BuffType("Shred")] = false;
            npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
            npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            npc.boss = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Murderswarm");
            else
                music = MusicID.Boss4;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.value = Item.buyPrice(0, 30, 0, 0);
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath46;
            bossBag = mod.ItemType("BumblebirbBag");
        }

        public override void AI()
        {
            // Variables
            Player player = Main.player[npc.target];
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            Vector2 vector = npc.Center;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Phases
            float mult = 1f +
                (revenge ? 0.25f : 0f) +
                ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.25f : 0f);
            bool phase2 = lifeRatio < 0.5f * mult;
            bool phase3 = lifeRatio < 0.1f * mult;

            // Max spawn amount
            int num1305 = revenge ? 4 : 3;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
                num1305 = 5;
            if (phase2)
                num1305 = 2;

            // Don't collide with tiles, disable gravity
            npc.noTileCollide = false;
            npc.noGravity = true;

            // Reset damage
            npc.damage = npc.defDamage;

            // Despawn
            if (Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            // Fly to target if target is too far away and not in idle or switch phase
            Vector2 vector205 = player.Center - npc.Center;
            if (npc.ai[0] > 1f && vector205.Length() > 3600f)
                npc.ai[0] = 1f;

            // Phase switch
            if (npc.ai[0] == 0f)
            {
                // Target
                npc.TargetClosest(true);

                if (npc.Center.X < player.Center.X - 2f)
                    npc.direction = 1;
                if (npc.Center.X > player.Center.X + 2f)
                    npc.direction = -1;

                // Direction and rotation
                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

                // Slow down if colliding with tiles
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
                    if (npc.velocity.X > 4f)
                        npc.velocity.X = 4f;
                    if (npc.velocity.X < -4f)
                        npc.velocity.X = -4f;
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
                    if (npc.velocity.Y > 4f)
                        npc.velocity.Y = 4f;
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }

                // Fly to target if target is too far away, otherwise get close to target and then slow down
                Vector2 value51 = player.Center - npc.Center;
                value51.Y -= 200f;
                if (value51.Length() > 2800f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (value51.Length() > 240f)
                {
                    float scaleFactor15 = 12f;
                    float num1306 = 30f;
                    value51.Normalize();
                    value51 *= scaleFactor15;
                    npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
                }
                else if (npc.velocity.Length() > 2f)
                    npc.velocity *= 0.95f;
                else if (npc.velocity.Length() < 1f)
                    npc.velocity *= 1.05f;

                // Phase switch
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                    while (npc.ai[0] == 0f)
                    {
                        int damage = Main.expertMode ? 50 : 60;

                        int num1307 = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
                        if (phase3)
                            num1307 = 1;

                        if (num1307 == 0 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                            npc.ai[0] = 2f;
                        else if (num1307 == 1)
                        {
                            npc.ai[0] = 3f;
                            if (phase2)
                            {
                                Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 102);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-2, 3), -4f, mod.ProjectileType("RedLightningFeather"), damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        else if (NPC.CountNPCS(mod.NPCType("Bumblefuck2")) < num1305)
                        {
                            npc.ai[0] = 4f;
                            Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 102);
                            int featherAmt = phase2 ? 3 : 6;
                            for (int num186 = 0; num186 < featherAmt; num186++)
                            {
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-4, 5), -3f, mod.ProjectileType("RedLightningFeather"), damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }
            else
            {
                // Fly to target
                if (npc.ai[0] == 1f)
                {
                    npc.collideX = false;
                    npc.collideY = false;
                    npc.noTileCollide = true;

                    if (npc.target < 0 || !player.active || player.dead)
                        npc.TargetClosest(true);

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else if (npc.velocity.X > 0f)
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.04f) / 10f;

                    Vector2 value52 = player.Center - npc.Center;
                    if (value52.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }

                    float scaleFactor16 = 14f + value52.Length() / 100f; //7
                    float num1308 = 25f;
                    value52.Normalize();
                    value52 *= scaleFactor16;
                    npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
                    return;
                }

                // Fly towards target quickly
                if (npc.ai[0] == 2f)
                {
                    if (npc.target < 0 || !player.active || player.dead)
                    {
                        npc.TargetClosest(true);
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }

                    if (player.Center.X - 10f < npc.Center.X)
                        npc.direction = -1;
                    else if (player.Center.X + 10f > npc.Center.X)
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.05f) / 5f;

                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
                        if (npc.velocity.X > 4f)
                            npc.velocity.X = 4f;
                        if (npc.velocity.X < -4f)
                            npc.velocity.X = -4f;
                    }
                    if (npc.collideY)
                    {
                        npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
                        if (npc.velocity.Y > 4f)
                            npc.velocity.Y = 4f;
                        if (npc.velocity.Y < -4f)
                            npc.velocity.Y = -4f;
                    }

                    Vector2 value53 = player.Center - npc.Center;
                    value53.Y -= 20f;
                    npc.ai[2] += 0.0222222228f;
                    if (Main.expertMode)
                        npc.ai[2] += 0.0166666675f;

                    float scaleFactor17 = 8f + npc.ai[2] + value53.Length() / 120f; //4
                    float num1309 = 20f;
                    value53.Normalize();
                    value53 *= scaleFactor17;
                    npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 120f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    // Variable for charging
                    float chargeDistance = 600f;
                    if (phase2)
                        chargeDistance -= 50f;
                    if (phase3)
                        chargeDistance -= 50f;

                    // Line up charge
                    if (npc.ai[0] == 3f)
                    {
                        npc.noTileCollide = true;

                        if (npc.velocity.X < 0f)
                            npc.direction = -1;
                        else
                            npc.direction = 1;

                        npc.spriteDirection = npc.direction;
                        npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;

                        Vector2 value54 = player.Center - npc.Center;
                        value54.Y -= 12f;
                        if (npc.Center.X > player.Center.X)
                            value54.X += chargeDistance;
                        else
                            value54.X -= chargeDistance;

                        if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(npc.Center.Y - player.Center.Y) < (phase3 ? 100f : 20f))
                        {
                            npc.ai[0] = 3.1f;
                            npc.ai[1] = 0f;
                        }

                        npc.ai[1] += 0.0333333351f;
                        float scaleFactor18 = 18f + npc.ai[1];
                        float num1310 = 4f;
                        value54.Normalize();
                        value54 *= scaleFactor18;
                        npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
                        return;
                    }

                    // Prepare to charge
                    if (npc.ai[0] == 3.1f)
                    {
                        npc.noTileCollide = true;

                        npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;

                        Vector2 vector206 = player.Center - npc.Center;
                        vector206.Y -= 12f;
                        float scaleFactor19 = 32f; //16
                        float num1311 = 8f;
                        vector206.Normalize();
                        vector206 *= scaleFactor19;
                        npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;

                        if (npc.velocity.X < 0f)
                            npc.direction = -1;
                        else
                            npc.direction = 1;

                        npc.spriteDirection = npc.direction;

                        npc.ai[1] += 1f;
                        if (npc.ai[1] > 10f)
                        {
                            npc.velocity = vector206;

                            if (npc.velocity.X < 0f)
                                npc.direction = -1;
                            else
                                npc.direction = 1;

                            npc.ai[0] = 3.2f;
                            npc.ai[1] = 0f;
                            npc.ai[1] = (float)npc.direction;
                        }
                    }
                    else
                    {
                        // Charge
                        if (npc.ai[0] == 3.2f)
                        {
                            npc.damage = (int)((double)npc.defDamage * 1.5);

                            npc.collideX = false;
                            npc.collideY = false;
                            npc.noTileCollide = true;

                            npc.ai[2] += 0.0333333351f;
                            npc.velocity.X = (32f + npc.ai[2]) * npc.ai[1];

                            if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + (chargeDistance - 140f)) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - (chargeDistance - 140f)))
                            {
                                if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                                {
                                    npc.ai[0] = 0f;
                                    npc.ai[1] = 0f;
                                    npc.ai[2] = 0f;
                                    npc.ai[3] = 0f;
                                }
                                else if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance + 200f)
                                {
                                    npc.ai[0] = 1f;
                                    npc.ai[1] = 0f;
                                    npc.ai[2] = 0f;
                                    npc.ai[3] = 0f;
                                }
                            }

                            npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
                            return;
                        }

                        // Find tile coordinates for birb spawn
                        if (npc.ai[0] == 4f)
                        {
                            npc.ai[0] = 0f;

                            npc.TargetClosest(true);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                npc.ai[1] = -1f;
                                npc.ai[2] = -1f;

                                for (int num1312 = 0; num1312 < 1000; num1312++)
                                {
                                    int num1313 = (int)player.Center.X / 16;
                                    int num1314 = (int)player.Center.Y / 16;

                                    int num1315 = 30 + num1312 / 50;
                                    int num1316 = 20 + num1312 / 75;

                                    num1313 += Main.rand.Next(-num1315, num1315 + 1);
                                    num1314 += Main.rand.Next(-num1316, num1316 + 1);

                                    if (!WorldGen.SolidTile(num1313, num1314))
                                    {
                                        while (!WorldGen.SolidTile(num1313, num1314) && (double)num1314 < Main.worldSurface)
                                            num1314++;

                                        if ((new Vector2((float)(num1313 * 16 + 8), (float)(num1314 * 16 + 8)) - player.Center).Length() < 3600f)
                                        {
                                            npc.ai[0] = 4.1f;
                                            npc.ai[1] = (float)num1313;
                                            npc.ai[2] = (float)num1314;
                                            break;
                                        }
                                    }
                                }
                            }

                            npc.netUpdate = true;
                            return;
                        }

                        // Move to birb spawn location
                        if (npc.ai[0] == 4.1f)
                        {
                            if (npc.velocity.X < -2f)
                                npc.direction = -1;
                            else if (npc.velocity.X > 2f)
                                npc.direction = 1;

                            npc.spriteDirection = npc.direction;
                            npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

                            npc.noTileCollide = true;

                            int num1317 = (int)npc.ai[1];
                            int num1318 = (int)npc.ai[2];

                            float x2 = (float)(num1317 * 16 + 8);
                            float y2 = (float)(num1318 * 16 - 20);

                            Vector2 vector207 = new Vector2(x2, y2);
                            vector207 -= npc.Center;
                            float num1319 = 12f + vector207.Length() / 150f;
                            if (num1319 > 20f)
                                num1319 = 20f;

                            float num1320 = 10f;
                            if (vector207.Length() < 10f)
                                npc.ai[0] = 4.2f;

                            vector207.Normalize();
                            vector207 *= num1319;
                            npc.velocity = (npc.velocity * (num1320 - 1f) + vector207) / num1320;
                            return;
                        }

                        // Spawn birbs
                        if (npc.ai[0] == 4.2f)
                        {
                            npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

                            npc.noTileCollide = true;

                            int num1321 = (int)npc.ai[1];
                            int num1322 = (int)npc.ai[2];

                            float x3 = (float)(num1321 * 16 + 8);
                            float y3 = (float)(num1322 * 16 - 20);

                            Vector2 vector208 = new Vector2(x3, y3);
                            vector208 -= npc.Center;

                            float num1323 = 4f; //4
                            float num1324 = 2f; //2

                            if (Main.netMode != NetmodeID.MultiplayerClient && vector208.Length() < 4f)
                            {
                                int num1325 = 10;
                                if (Main.expertMode)
                                    num1325 = (int)((double)num1325 * 0.75);

                                npc.ai[3] += 1f;
                                if (npc.ai[3] == (float)num1325)
                                    NPC.NewNPC(num1321 * 16 + 8, num1322 * 16, mod.NPCType("Bumblefuck2"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                                else if (npc.ai[3] == (float)(num1325 * 2))
                                {
                                    npc.ai[0] = 0f;
                                    npc.ai[1] = 0f;
                                    npc.ai[2] = 0f;
                                    npc.ai[3] = 0f;

                                    if (NPC.CountNPCS(mod.NPCType("Bumblefuck2")) < num1305 && Main.rand.Next(5) != 0)
                                        npc.ai[0] = 4f;
                                    else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                                        npc.ai[0] = 1f;
                                }
                            }

                            if (vector208.Length() > num1323)
                            {
                                vector208.Normalize();
                                vector208 *= num1323;
                            }

                            npc.velocity = (npc.velocity * (num1324 - 1f) + vector208) / num1324;
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += (double)(npc.velocity.Length() / 4f);
            npc.frameCounter += 1.0;
            if (npc.ai[0] < 4f)
            {
                if (npc.frameCounter >= 6.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y / frameHeight > 4)
                {
                    npc.frame.Y = 0;
                }
            }
            else
            {
                if (npc.frameCounter >= 6.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y / frameHeight > 9)
                {
                    npc.frame.Y = frameHeight * 5;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Bumblebirb";
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, mod.ItemType("BumblebirbTrophy"), 10);
            DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeBumblebirb"), true, !CalamityWorld.downedBumble);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBumble, 5, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, mod.ItemType("EffulgentFeather"), 6, 11);

                // Weapons
                DropHelper.DropItemFromSet(npc, mod.ItemType("GildedProboscis"), mod.ItemType("GoldenEagle"), mod.ItemType("RougeSlash"));
                DropHelper.DropItemChance(npc, mod.ItemType("Swordsplosion"), DropHelper.RareVariantDropRateInt);
            }

            // Mark Bumblebirb as dead
            CalamityWorld.downedBumble = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleHead"), 1f);
                for (int wing = 0; wing < 2; wing++)
                {
                    randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleWing"), 1f);
                }
                for (int leg = 0; leg < 4; leg++)
                {
                    randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleLeg"), 1f);
                }
            }
        }
    }
}
