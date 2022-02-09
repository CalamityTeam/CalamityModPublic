using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    public class DesertScourgeHead : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool TailSpawned = false;
        public bool playRoarSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.GetNPCDamage();
            npc.defense = 4;
            npc.npcSlots = 12f;
            npc.width = 32;
            npc.height = 80;
            npc.LifeMaxNERB(2500, 3000, 1650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 2, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = ModContent.ItemType<DesertScourgeBag>();
            music = CalamityMod.Instance.GetMusicFromMusicMod("DesertScourge") ?? MusicID.Boss1;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                npc.scale = 1.25f;
            else if (CalamityWorld.death)
                npc.scale = 1.2f;
            else if (CalamityWorld.revenge)
                npc.scale = 1.15f;
            else if (Main.expertMode)
                npc.scale = 1.1f;

            npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToWater = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(playRoarSound);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            playRoarSound = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            // Enrage
            if (!player.ZoneDesert && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

            float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
            if (biomeEnraged)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 2f;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            if (revenge || lifeRatio < (expertMode ? 0.75f : 0.5f))
                npc.Calamity().newAI[0] += 1f;

            float burrowTimeGateValue = death ? 420f : 540f;
            bool burrow = npc.Calamity().newAI[0] >= burrowTimeGateValue;
            bool resetTime = npc.Calamity().newAI[0] >= burrowTimeGateValue + 600f;
            bool lungeUpward = burrow && npc.Calamity().newAI[1] == 1f;
            bool quickFall = npc.Calamity().newAI[1] == 2f;

            float speed = 0.09f;
            float turnSpeed = 0.06f;

            if (expertMode)
            {
                float velocityScale = death ? 0.12f : 0.06f;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = death ? 0.075f : 0.05f;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            speed += 0.12f * enrageScale;
            turnSpeed += 0.06f * enrageScale;

            if (lungeUpward)
            {
                speed *= 1.25f;
                turnSpeed *= 1.5f;
            }

            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            npc.alpha -= 42;
            if (npc.alpha < 0)
                npc.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    int minLength = death ? 40 : revenge ? 35 : expertMode ? 30 : 25;
                    for (int num36 = 0; num36 < minLength + 1; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DesertScourgeBody>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DesertScourgeTail>(), npc.whoAmI);
                        }
                        Main.npc[lol].ai[2] = npc.whoAmI;
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }

            int num12 = (int)(npc.position.X / 16f) - 1;
            int num13 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num14 = (int)(npc.position.Y / 16f) - 1;
            int num15 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num12 < 0)
            {
                num12 = 0;
            }
            if (num13 > Main.maxTilesX)
            {
                num13 = Main.maxTilesX;
            }
            if (num14 < 0)
            {
                num14 = 0;
            }
            if (num15 > Main.maxTilesY)
            {
                num15 = Main.maxTilesY;
            }
            bool flag2 = lungeUpward;
            if (!flag2)
            {
                for (int k = num12; k < num13; k++)
                {
                    for (int l = num14; l < num15; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[(int)Main.tile[k, l].type] || (Main.tileSolidTop[(int)Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                npc.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int num954 = 1000;
                if (enrageScale > 0f)
                    num954 = 100;

                bool flag3 = true;
                if (npc.position.Y > player.position.Y)
                {
                    int rectWidth = num954 * 2;
                    int rectHeight = num954 * 2;
                    for (int m = 0; m < 255; m++)
                    {
                        if (Main.player[m].active)
                        {
                            int rectX = (int)Main.player[m].position.X - num954;
                            int rectY = (int)Main.player[m].position.Y - num954;
                            Rectangle rectangle13 = new Rectangle(rectX, rectY, rectWidth, rectHeight);
                            if (rectangle.Intersects(rectangle13))
                            {
                                flag3 = false;
                                break;
                            }
                        }
                    }
                    if (flag3)
                    {
                        flag2 = true;
                    }
                }
            }
            else
            {
                npc.localAI[1] = 0f;
            }

            float num17 = 16f;
            if (player.dead)
            {
                flag2 = false;
                npc.velocity.Y = npc.velocity.Y + 1f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 1f;
                    num17 = 32f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertScourgeHead>() || Main.npc[a].type == ModContent.NPCType<DesertScourgeBody>() ||
                            Main.npc[a].type == ModContent.NPCType<DesertScourgeTail>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }

            float num18 = speed;
            float num19 = turnSpeed;
            float burrowDistance = malice ? 500f : 800f;
            float burrowTarget = player.Center.Y + burrowDistance;
            float lungeTarget = player.Center.Y - 600f;
            Vector2 vector3 = npc.Center;
            float num20 = player.Center.X;
            float num21 = lungeUpward ? lungeTarget : burrow ? burrowTarget : player.Center.Y;
            num20 = (float)((int)(num20 / 16f) * 16);
            num21 = (float)((int)(num21 / 16f) * 16);
            vector3.X = (float)((int)(vector3.X / 16f) * 16);
            vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
            num20 -= vector3.X;
            num21 -= vector3.Y;
            float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));

            // Lunge up towards target
            if (burrow && npc.Center.Y >= burrowTarget - 16f)
            {
                npc.Calamity().newAI[1] = 1f;
                if (!playRoarSound)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DesertScourgeRoar"), player.Center);
                    playRoarSound = true;
                }
            }

            // Quickly fall back down once above target
            if (lungeUpward && npc.Center.Y <= player.Center.Y - 420f)
            {
                npc.TargetClosest();
                npc.Calamity().newAI[1] = 2f;
                playRoarSound = false;
            }

            // Quickly fall and reset variables once at target's Y position
            if (quickFall)
            {
                npc.velocity.Y += 1f;
                if (npc.Center.Y >= player.Center.Y)
                {
                    npc.Calamity().newAI[0] = 0f;
                    npc.Calamity().newAI[1] = 0f;
                    playRoarSound = false;
                }
            }

            // Reset variables if the burrow and lunge attack is taking too long
            if (resetTime)
            {
                npc.Calamity().newAI[0] = 0f;
                npc.Calamity().newAI[1] = 0f;
            }

            if (!flag2)
            {
                npc.TargetClosest(true);
                npc.velocity.Y = npc.velocity.Y + 0.15f;
                if (npc.velocity.Y > num17)
                {
                    npc.velocity.Y = num17;
                }
                if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == num17)
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 0.9f;
                    }
                }
            }
            else
            {
                if (npc.soundDelay == 0)
                {
                    float num24 = num22 / 40f;
                    if (num24 < 10f)
                    {
                        num24 = 10f;
                    }
                    if (num24 > 20f)
                    {
                        num24 = 20f;
                    }
                    npc.soundDelay = (int)num24;
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
                }
                num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                float num25 = Math.Abs(num20);
                float num26 = Math.Abs(num21);
                float num27 = num17 / num22;
                num20 *= num27;
                num21 *= num27;
                if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num19;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num19;
                    }
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num19;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num19;
                    }
                }
                if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18;
                    }
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num18;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num18;
                    }
                    if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num18 * 2f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num18 * 2f;
                        }
                    }
                    if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num18 * 2f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num18 * 2f;
                        }
                    }
                }
                else if (num25 > num26)
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 1.1f;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num18;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num18;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num18 * 1.1f;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num18;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num18;
                        }
                    }
                }
            }
            npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
            if (flag2)
            {
                if (npc.localAI[0] != 1f)
                {
                    npc.netUpdate = true;
                }
                npc.localAI[0] = 1f;
            }
            else
            {
                if (npc.localAI[0] != 0f)
                {
                    npc.netUpdate = true;
                }
                npc.localAI[0] = 0f;
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        #region Loot
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SandBlock;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<DesertScourgeHead>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.LesserHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<DesertScourgeTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDesertScourge>(), true, !CalamityWorld.downedDesertScourge);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<VictoryShard>(), 7, 14);
                DropHelper.DropItem(npc, ItemID.Coral, 5, 9);
                DropHelper.DropItem(npc, ItemID.Seashell, 5, 9);
                DropHelper.DropItem(npc, ItemID.Starfish, 5, 9);

                // Weapons
                // Set up the base drop set, which includes Scourge of the Desert at its normal drop chance.
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<AquaticDischarge>(w),
                    DropHelper.WeightStack<Barinade>(w),
                    DropHelper.WeightStack<StormSpray>(w),
                    DropHelper.WeightStack<SeaboundStaff>(w),
                    DropHelper.WeightStack<ScourgeoftheDesert>(w),
					DropHelper.WeightStack<AeroStone>(w),
					DropHelper.WeightStack<SandCloak>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<OceanCrest>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<DesertScourgeMask>(), 7);

                // Fishing
                DropHelper.DropItem(npc, ModContent.ItemType<SandyAnglingKit>());
            }

            // If Desert Scourge has not been killed yet, notify players that the Sunken Sea is open and Sandstorms can happen
            if (!CalamityWorld.downedDesertScourge)
            {
                string key = "Mods.CalamityMod.OpenSunkenSea";
                Color messageColor = Color.Aquamarine;
                string key2 = "Mods.CalamityMod.SandstormTrigger";
                Color messageColor2 = Color.PaleGoldenrod;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor2);

                if (!Terraria.GameContent.Events.Sandstorm.Happening)
                    typeof(Terraria.GameContent.Events.Sandstorm).GetMethod("StartSandstorm", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }

            // Mark Desert Scourge as dead
            CalamityWorld.downedDesertScourge = true;
            CalamityNetcode.SyncWorld();
        }
        #endregion

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead2"), 1f);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 300, true);
        }
    }
}
