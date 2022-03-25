using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    public class AstrumDeusHeadSpectral : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 20;
			npc.DR_NERD(0.1f);
			npc.LifeMaxNERB(200000, 240000, 650000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                npc.scale = 1.5f;
            else if (CalamityWorld.death)
                npc.scale = 1.4f;
            else if (CalamityWorld.revenge)
                npc.scale = 1.35f;
            else if (Main.expertMode)
                npc.scale = 1.2f;

            npc.boss = true;
            npc.value = Item.buyPrice(1, 0, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstrumDeusDeath");
            npc.netAlways = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("AstrumDeus") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<AstrumDeusBag>();
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToSickness = false;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityAI.AstrumDeusAI(npc, mod, true);
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			bool drawCyan = npc.Calamity().newAI[3] >= 600f;
            bool deathModeEnragePhase = npc.Calamity().newAI[0] == 3f;
            bool doubleWormPhase = npc.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow2");
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = deathModeEnragePhase ? 10 : 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow");
			Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
			if (doubleWormPhase)
			{
				texture2D15 = drawCyan ? texture2D15 : ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow3");
				texture2D16 = drawCyan ? ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow4") : texture2D16;
			}
			Color color37 = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Cyan, 0.5f) * (deathModeEnragePhase ? 1f : npc.Opacity);
			Color color42 = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : npc.Opacity);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 *= (num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 1 : 2;
			for (int i = 0; i < timesToDraw; i++)
				spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
			for (int i = 0; i < timesToDraw; i++)
				spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
        }

		public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ModContent.ItemType<Stardust>();

        public override bool SpecialNPCLoot()
        {
			if (npc.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && npc.Calamity().newAI[0] != 3f))
				return false;

			int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<AstrumDeusHeadSpectral>(),
                ModContent.NPCType<AstrumDeusBodySpectral>(),
                ModContent.NPCType<AstrumDeusTailSpectral>());
            npc.position = Main.npc[closestSegmentID].position;

            return false;
        }

		public override bool PreNPCLoot()
        {
			// Unsplit Deus does not drop anything when killed/despawned.
			if (npc.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && npc.Calamity().newAI[0] != 3f))
				return false;

            // Killing ANY split Deus makes all other Deus heads die immediately.
            for (int i = 0; i < Main.maxNPCs; ++i)
			{
                NPC otherWormHead = Main.npc[i];
                if (otherWormHead.active && otherWormHead.type == npc.type)
                {
                    // Kill the other worm head after setting it to not drop loot.
                    otherWormHead.Calamity().newAI[0] = 0f;
                    otherWormHead.life = 0;
                    otherWormHead.checkDead();
                    otherWormHead.netUpdate = true;
                }
			}

            return true;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<AstrumDeusTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAstrumDeus>(), !CalamityWorld.downedStarGod);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAstralInfection>(), !CalamityWorld.downedStarGod);

            // Drop a large spray of all 4 lunar fragments
            int minFragments = Main.expertMode ? 20 : 16;
            int maxFragments = Main.expertMode ? 32 : 24;
            DropHelper.DropItemSpray(npc, ItemID.FragmentSolar, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(npc, ItemID.FragmentVortex, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(npc, ItemID.FragmentNebula, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(npc, ItemID.FragmentStardust, minFragments, maxFragments, 4);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                DropHelper.DropItemSpray(npc, ModContent.ItemType<Stardust>(), 50, 80, 5);
                DropHelper.DropItemSpray(npc, ItemID.FallenStar, 25, 40, 5);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<TheMicrowave>(w),
                    DropHelper.WeightStack<StarSputter>(w),
                    DropHelper.WeightStack<Starfall>(w),
                    DropHelper.WeightStack<GodspawnHelixStaff>(w),
                    DropHelper.WeightStack<RegulusRiot>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<HideofAstrumDeus>(), true);
				DropHelper.DropItemChance(npc, ModContent.ItemType<ChromaticOrb>(), 5);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<AstrumDeusMask>(), 7);
            }

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!CalamityWorld.downedStarGod)
            {
                string key = "Mods.CalamityMod.AstralBossText";
                Color messageColor = Color.Gold;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Astrum Deus as dead
            CalamityWorld.downedStarGod = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
