using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
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
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Calamitas
{
	[AutoloadBossHead]
    public class CalamitasRun3 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 14f;
            npc.width = 120;
            npc.height = 120;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                npc.scale = 0.8f;

            npc.defense = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 12 : 25;
            npc.value = Item.buyPrice(0, 50, 0, 0);
			npc.DR_NERD((CalamityWorld.death || BossRushEvent.BossRushActive) ? 0.075f : 0.15f);
			npc.LifeMaxNERB(37500, 45000, 520000);
			if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.damage *= 3;
                npc.defense *= 3;
                npc.lifeMax *= 2;
                npc.value *= 2.5f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            music = CalamityMod.Instance.GetMusicFromMusicMod("Calamitas") ?? MusicID.Boss2;
            bossBag = ModContent.ItemType<CalamitasBag>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToWater = true;
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
			CalamityAI.CalamitasCloneAI(npc, mod);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.npcFrameCount[npc.type] / 2));
			Color white = Color.White;
			float colorLerpAmt = 0.5f;
			int afterimageAmt = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < afterimageAmt; i += 2)
				{
					Color color1 = lightColor;
					color1 = Color.Lerp(color1, white, colorLerpAmt);
					color1 = npc.GetAlpha(color1);
					color1 *= (float)(afterimageAmt - i) / 15f;
					Vector2 offset = npc.oldPos[i] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					offset += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, offset, npc.frame, color1, npc.rotation, origin, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 npcOffset = npc.Center - Main.screenPosition;
			npcOffset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			npcOffset += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture, npcOffset, npc.frame, npc.GetAlpha(lightColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/Calamitas/CalamitasRun3Glow");
			Color color = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < afterimageAmt; i++)
				{
					Color color2 = color;
					color2 = Color.Lerp(color2, white, colorLerpAmt);
					color2 *= (float)(afterimageAmt - i) / 15f;
					Vector2 offset = npc.oldPos[i] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					offset -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					offset += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, offset, npc.frame, color2, npc.rotation, origin, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture, npcOffset, npc.frame, color, npc.rotation, origin, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItem(npc, ItemID.BrokenHeroSword, true);
            DropHelper.DropItemChance(npc, ModContent.ItemType<CalamitasTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCalamitasClone>(), !CalamityWorld.downedCalamitas);

			CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedCalamitas);

			if (!Main.expertMode)
            {
				//Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EssenceofChaos>(), 4, 8);
                DropHelper.DropItem(npc, ModContent.ItemType<CalamityDust>(), 9, 14);
                DropHelper.DropItem(npc, ModContent.ItemType<BlightedLens>(), 1, 2);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 1f, 30, 40);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<TheEyeofCalamitas>(w),
                    DropHelper.WeightStack<Animosity>(w),
                    DropHelper.WeightStack<CalamitasInferno>(w),
                    DropHelper.WeightStack<BlightedEyeStaff>(w),
					DropHelper.WeightStack<ChaosStone>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<CalamityRing>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<CalamitasMask>(), 7);
				if (Main.rand.NextBool(10))
				{
					DropHelper.DropItem(npc, ModContent.ItemType<CalamityHood>());
					DropHelper.DropItem(npc, ModContent.ItemType<CalamityRobes>());
				}
            }

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Regenator>(), !Main.expertMode, 0.1f);

            // Abyss awakens after killing Calamitas
            string key = "Mods.CalamityMod.PlantBossText";
            Color messageColor = Color.RoyalBlue;

            if (!CalamityWorld.downedCalamitas)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Calamitas as dead
            CalamityWorld.downedCalamitas = true;
            CalamityNetcode.SyncWorld();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The Calamitas Clone";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas3"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas4"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas5"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Calamitas6"), npc.scale);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
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
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180, true);
        }
    }
}
