using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHive : ModNPC
    {
        private bool small = false;
        private bool medium = false;
        private bool large = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator Hive");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 18f;
            npc.damage = 30;
            npc.width = 110;
            npc.height = 100;
            npc.defense = 4;
            npc.LifeMaxNERB(3750, 5400, 2700000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 6, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath19;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/BloodCoagulant");
            else
                music = MusicID.Boss2;
            bossBag = ModContent.ItemType<PerforatorBag>();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

		public override void AI()
		{
			CalamityGlobalNPC.perfHive = npc.whoAmI;

			npc.TargetClosest(true);
			Player player = Main.player[npc.target];

			bool isCrimson = player.ZoneCrimson || CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = 12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			int wormsAlive = 0;
			bool largeWormAlive = false;
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
			{
				wormsAlive++;
				largeWormAlive = true;
			}
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>()))
				wormsAlive++;
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadSmall>()))
				wormsAlive++;

			if (largeWormAlive && expertMode)
			{
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = isCrimson ? false : true;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int shoot = revenge ? 6 : 4;
				npc.localAI[0] += (float)Main.rand.Next(shoot);
				if (npc.localAI[0] >= (float)Main.rand.Next(300, 901) && npc.position.Y + (float)npc.height < player.position.Y && Vector2.Distance(player.Center, npc.Center) > 80f)
				{
					npc.localAI[0] = 0f;
					Main.PlaySound(SoundID.NPCHit20, npc.position);

					for (int num621 = 0; num621 < 8; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 170, 0f, 0f, 100, default, 1f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.NextBool(2))
						{
							Main.dust[num622].scale = 0.25f;
							Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
						}
					}

					for (int num623 = 0; num623 < 16; num623++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1f);
						Main.dust[num624].velocity *= 2f;
					}

					float num179 = 8f;
					Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
					float num181 = Math.Abs(num180) * 0.1f;
					float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
					float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
					npc.netUpdate = true;
					num183 = num179 / num183;
					num180 *= num183;
					num182 *= num183;
					int num184 = expertMode ? 14 : 18;
					int num185 = Main.rand.NextBool(2) ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
					value9.X += num180;
					value9.Y += num182;

					for (int num186 = 0; num186 < 20; num186++)
					{
						num180 = player.position.X + (float)player.width * 0.5f - value9.X;
						num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
						num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
						num183 = num179 / num183;
						num180 += (float)Main.rand.Next(-180, 181);
						num180 *= num183;
						Projectile.NewProjectile(value9.X, value9.Y, num180, -5f, num185, num184, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			npc.rotation = npc.velocity.X * 0.04f;

			if (revenge)
			{
				if (wormsAlive == 1)
				{
					Movement(player, 4f, 1f, (CalamityWorld.bossRushActive ? 0.2f : 0.15f), 160f, 300f, 400f, false);
					npc.ai[0] = 0f;
				}
				else
				{
					if (npc.ai[0] == 1f)
					{
						if (large || death)
						{
							Movement(player, 5f, 1.5f, (CalamityWorld.bossRushActive ? 0.195f : 0.13f), 360f, 10f, 50f, true);
						}
						else if (medium)
						{
							Movement(player, 6f, 2f, (CalamityWorld.bossRushActive ? 0.18f : 0.12f), 340f, 15f, 50f, true);
						}
						else if (small)
						{
							Movement(player, 7f, 2.5f, (CalamityWorld.bossRushActive ? 0.165f : 0.11f), 320f, 20f, 50f, true);
						}
						else
						{
							Movement(player, 8f, 3f, (CalamityWorld.bossRushActive ? 0.15f : 0.1f), 300f, 25f, 50f, true);
						}
					}
					else
					{
						npc.velocity.X += (npc.Center.X <= player.Center.X ? -0.1f : 0.1f);
						if (npc.Center.X > player.Center.X + 320f || npc.Center.X < player.Center.X - 320f)
						{
							npc.ai[0] = 1f;
						}
					}
				}
			}
			else
			{
				Movement(player, 4f, 1f, 0.1f, 160f, 300f, 400f, false);
			}

			if (npc.ai[3] == 0f && npc.life > 0)
			{
				npc.ai[3] = (float)npc.lifeMax;
			}
			if (npc.life > 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num660 = (int)((double)npc.lifeMax * 0.3);
					if ((float)(npc.life + num660) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int wormType = ModContent.NPCType<PerforatorHeadSmall>();
						if (!small)
						{
							small = true;
						}
						else if (!medium)
						{
							medium = true;
							wormType = ModContent.NPCType<PerforatorHeadMedium>();
						}
						else if (!large)
						{
							large = true;
							wormType = ModContent.NPCType<PerforatorHeadLarge>();
						}
						NPC.SpawnOnPlayer(npc.FindClosestPlayer(), wormType);
					}
				}
			}
		}

		private void Movement(Player target, float velocityX, float velocityY, float acceleration, float x, float y, float y2, bool charging)
		{
			if (npc.position.Y > target.position.Y - y) //200
			{
				if (npc.velocity.Y > 0f)
				{
					npc.velocity.Y *= 0.98f;
				}
				npc.velocity.Y -= acceleration;
				if (npc.velocity.Y > velocityY)
				{
					npc.velocity.Y = velocityY;
				}
			}
			else if (npc.position.Y < target.position.Y - y2) //500
			{
				if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y *= 0.98f;
				}
				npc.velocity.Y += acceleration;
				if (npc.velocity.Y < -velocityY)
				{
					npc.velocity.Y = -velocityY;
				}
			}

			if (npc.Center.X > target.Center.X + x)
			{
				if (npc.velocity.X > 0f)
				{
					npc.velocity.X *= 0.98f;
				}
				npc.velocity.X -= acceleration;
				if (npc.velocity.X > velocityX)
				{
					npc.velocity.X = velocityX;
				}
			}
			else if (npc.Center.X < target.Center.X - x)
			{
				if (npc.velocity.X < 0f)
				{
					npc.velocity.X *= 0.98f;
				}
				npc.velocity.X += acceleration;
				if (npc.velocity.X < -velocityX)
				{
					npc.velocity.X = -velocityX;
				}
			}

			if (charging)
			{
				if (npc.Center.X <= target.Center.X + x && npc.Center.X >= target.Center.X - x)
				{
					npc.velocity.X += (npc.Center.X <= target.Center.X ? acceleration : -acceleration) * 0.25f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorHiveGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CheckDead()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
            {
                return false;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<PerforatorTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePerforators>(), true, !CalamityWorld.downedPerforator);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedPerforator, 2, 0, 0);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, CalamityWorld.downedPerforator);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<BloodSample>(), 7, 14);
                DropHelper.DropItemSpray(npc, ItemID.CrimtaneBar, 2, 5);
                DropHelper.DropItemSpray(npc, ItemID.Vertebrae, 3, 9);
                if (Main.hardMode)
                    DropHelper.DropItemSpray(npc, ItemID.Ichor, 10, 20);

				// Weapons
				float w = DropHelper.DirectWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack<VeinBurster>(w),
					DropHelper.WeightStack<BloodyRupture>(w),
					DropHelper.WeightStack<SausageMaker>(w),
					DropHelper.WeightStack<Aorta>(w),
					DropHelper.WeightStack<Eviscerator>(w),
					DropHelper.WeightStack<BloodBath>(w),
					DropHelper.WeightStack<BloodClotStaff>(w),
					DropHelper.WeightStack<ToothBall>(w, 30, 50)
				);

				//Equipment
				DropHelper.DropItemChance(npc, ModContent.ItemType<BloodstainedGlove>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PerforatorMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodyVein>(), 10);
            }

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator)
            {
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
                WorldGenerationMethods.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, .4f, .6f);

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Mark The Perforator Hive as dead
            CalamityWorld.downedPerforator = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive4"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
