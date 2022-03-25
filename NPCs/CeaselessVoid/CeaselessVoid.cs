using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace CalamityMod.NPCs.CeaselessVoid
{
    [AutoloadBossHead]
    public class CeaselessVoid : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceaseless Void");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 36f;
            npc.width = 100;
            npc.height = 100;
            npc.defense = 80;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.5f;

			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel1;
			npc.LifeMaxNERB(notDoGFight ? 64400 : 16100, notDoGFight ? 77280 : 19320, 72000);

            // If fought alone, Ceaseless Void plays its own theme
            if (notDoGFight)
            {
                npc.value = Item.buyPrice(2, 0, 0, 0);
                music = CalamityMod.Instance.GetMusicFromMusicMod("Void") ?? MusicID.Boss3;
            }
            // If fought as a DoG interlude, keep the DoG music playing
            else
                music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.DeathSound = SoundID.NPCDeath14;
            bossBag = ModContent.ItemType<CeaselessVoidBag>();
			npc.Calamity().VulnerableToSickness = false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
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
			CalamityAI.CeaselessVoidAI(npc, mod);
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 50f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/CeaselessVoid/CeaselessVoidGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void NPCLoot()
        {
            // Only drop items if fought at full strength
			bool fullStrength = !CalamityWorld.downedSentinel1 || CalamityWorld.DoGSecondStageCountdown <= 0;
            if (fullStrength)
            {
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				DropHelper.DropBags(npc);

				DropHelper.DropItemChance(npc, ModContent.ItemType<CeaselessVoidTrophy>(), 10);
				bool lastSentinelKilled = !CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3;
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);

				if (!Main.expertMode)
				{
					// Materials
					DropHelper.DropItem(npc, ModContent.ItemType<DarkPlasma>(), true, 2, 3);

                    // Weapons
                    float dropChance = DropHelper.NormalWeaponDropRateFloat;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MirrorBlade>(), dropChance);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<VoidConcentrationStaff>(), dropChance);

					// Equipment
					DropHelper.DropItem(npc, ModContent.ItemType<TheEvolution>(), true);

					// Vanity
					DropHelper.DropItemChance(npc, ModContent.ItemType<CeaselessVoidMask>(), 7);
					if (Main.rand.NextBool(20))
					{
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerHelm>());
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerChestplate>());
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerLeggings>());
					}
				}
            }

            // If DoG's fight is active, set the timer for the remaining two sentinels
            if (CalamityWorld.DoGSecondStageCountdown > 14460)
            {
                CalamityWorld.DoGSecondStageCountdown = 14460;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

			// Mark Ceaseless Void as dead
			if (fullStrength)
			{
				CalamityWorld.downedSentinel1 = true;
				CalamityNetcode.SyncWorld();
			}
        }

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
		}

		public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

		public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
            }

			for (int k = 0; k < 5; k++)
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
				Main.dust[dust].noGravity = true;
			}

            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
					Main.dust[num622].noGravity = true;
					if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
					Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 2f;
                }

                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
            }
        }
    }
}
