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
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
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
        private bool flies = false;
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
			npc.npcSlots = 12f;
            npc.width = 32;
            npc.height = 80;
            npc.LifeMaxNERB(2600, 3000, 1650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = 6;
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

			if (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice)
				npc.scale = 1.25f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
                npc.scale = 1.1f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(playRoarSound);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            playRoarSound = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || malice;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			float enrageScale = 0f;
			if (!player.ZoneDesert || malice)
				enrageScale += 2f;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			if (revenge || lifeRatio < (expertMode ? 0.75f : 0.5f))
				npc.Calamity().newAI[0] += 1f;

			float burrowTimeGateValue = death ? 420f : 540f;
			bool burrow = npc.Calamity().newAI[0] >= burrowTimeGateValue;
			bool resetTime = npc.Calamity().newAI[0] >= burrowTimeGateValue + 600f;
			bool lungeUpward = burrow && npc.Calamity().newAI[1] == 1f;
			bool quickFall = npc.Calamity().newAI[1] == 2f;

			float speed = 8f;
			float turnSpeed = 0.08f;

			if (expertMode)
			{
				float velocityScale = death ? 9f : 6f;
				speed += velocityScale * (1f - lifeRatio);
				float accelerationScale = death ? 0.09f : 0.06f;
				turnSpeed += accelerationScale * (1f - lifeRatio);
			}

			speed += 4f * enrageScale;
			turnSpeed += 0.04f * enrageScale;

			if (lungeUpward)
			{
				speed *= 1.5f;
				turnSpeed *= 1.5f;
			}

			if (npc.Calamity().enraged > 0)
			{
				speed *= 1.25f;
				turnSpeed *= 1.25f;
			}

			if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

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
						Main.npc[lol].ai[3] = npc.whoAmI;
						Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }

            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + npc.height) / 16f) + 2;
            if (num180 < 0)
                num180 = 0;
            if (num181 > Main.maxTilesX)
                num181 = Main.maxTilesX;
            if (num182 < 0)
                num182 = 0;
            if (num183 > Main.maxTilesY)
                num183 = Main.maxTilesY;

            bool flag94 = flies || lungeUpward;
			if (!flag94)
			{
				for (int num952 = num180; num952 < num181; num952++)
				{
					for (int num953 = num182; num953 < num183; num953++)
					{
						if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[Main.tile[num952, num953].type] || (Main.tileSolidTop[Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
						{
							Vector2 vector105;
							vector105.X = num952 * 16;
							vector105.Y = num953 * 16;
							if (npc.position.X + npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
							{
								flag94 = true;
								break;
							}
						}
					}
				}
			}

			if (!flag94)
			{
				Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num954 = (npc.Calamity().enraged > 0) ? 500 : 1000;
				if (enrageScale > 0f)
					num954 = 100;

				bool flag95 = true;
				if (npc.position.Y > player.position.Y)
				{
					int rectWidth = num954 * 2;
					int rectHeight = num954 * 2;
					for (int num955 = 0; num955 < Main.maxPlayers; num955++)
					{
						if (Main.player[num955].active)
						{
							int rectX = (int)Main.player[num955].position.X - num954;
							int rectY = (int)Main.player[num955].position.Y - num954;
							Rectangle rectangle13 = new Rectangle(rectX, rectY, rectWidth, rectHeight);
							if (rectangle12.Intersects(rectangle13))
							{
								flag95 = false;
								break;
							}
						}
					}

					if (flag95)
						flag94 = true;
				}
			}

            if (player.dead)
            {
				npc.TargetClosest(false);
				flag94 = false;
                npc.velocity.Y += 1f;
                if (npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y += 1f;
                }
                if (npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }

            float num188 = speed;
            float num189 = turnSpeed;
			float burrowTarget = player.Center.Y + 1000f;
			float lungeTarget = player.Center.Y - 600f;
			Vector2 vector18 = npc.Center;
            float num191 = player.Center.X;
            float num192 = lungeUpward ? lungeTarget : burrow ? burrowTarget : player.Center.Y;
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);

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

			if (!flag94)
            {
                npc.velocity.Y += turnSpeed * 0.75f;
                if (npc.velocity.Y > num188)
                    npc.velocity.Y = num188;

                if ((System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < num188 * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X -= num189 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X += num189 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == num188)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X += num189;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X -= num189;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X += num189 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X -= num189 * 0.9f;
                    }
                }
            }
            else
            {
                if (!flies && npc.behindTiles && npc.soundDelay == 0)
                {
                    float num195 = num193 / 40f;
                    if (num195 < 10f)
                    {
                        num195 = 10f;
                    }
                    if (num195 > 20f)
                    {
                        num195 = 20f;
                    }
                    npc.soundDelay = (int)num195;
					//Play the worm digging sound.  No, I don't know why it's the same ID (but different style) as the generic boss roar and a scream
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1);
                }
                num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                bool flag21 = false;
                if (!flag21)
                {
                    if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X += num189;
                        }
                        else
                        {
                            if (npc.velocity.X > num191)
                            {
                                npc.velocity.X -= num189;
                            }
                        }
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y += num189;
                        }
                        else
                        {
                            if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y -= num189;
                            }
                        }
                        if (System.Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y += num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.Y -= num189 * 2f;
                            }
                        }
                        if (System.Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X += num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.X -= num189 * 2f;
                            }
                        }
                    }
                    else
                    {
                        if (num196 > num197)
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X += num189 * 1.1f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X -= num189 * 1.1f;
                            }
                            if ((System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y += num189;
                                }
                                else
                                {
                                    npc.velocity.Y -= num189;
                                }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y += num189 * 1.1f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y -= num189 * 1.1f;
                            }
                            if ((System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X += num189;
                                }
                                else
                                {
                                    npc.velocity.X -= num189;
                                }
                            }
                        }
                    }
                }

                npc.rotation = (float)System.Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (flag94)
                {
                    if (npc.localAI[0] != 1f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 0f;
                }

                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                {
                    npc.netUpdate = true;
                }
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
            DropHelper.DropBags(npc);

			// Legendary drop for Desert Scourge
			DropHelper.DropItemCondition(npc, ModContent.ItemType<DuneHopper>(), true, CalamityWorld.malice);

			DropHelper.DropItem(npc, ItemID.LesserHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<DesertScourgeTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDesertScourge>(), true, !CalamityWorld.downedDesertScourge);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SEAHOE>() }, CalamityWorld.downedDesertScourge);

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
                DropHelper.WeightedItemStack[] weapons =
                {
                    DropHelper.WeightStack<AquaticDischarge>(w),
                    DropHelper.WeightStack<Barinade>(w),
                    DropHelper.WeightStack<StormSpray>(w),
                    DropHelper.WeightStack<SeaboundStaff>(w),
                    DropHelper.WeightStack<ScourgeoftheDesert>(w),
                };
                DropHelper.DropEntireWeightedSet(npc, weapons);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<AeroStone>(), 10);
                DropHelper.DropItemChance(npc, ModContent.ItemType<SandCloak>(), 10);

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
