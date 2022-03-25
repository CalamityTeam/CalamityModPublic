using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.BrimstoneElemental
{
	[AutoloadBossHead]
    public class BrimstoneElemental : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Elemental");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 64f;
			npc.GetNPCDamage();
			npc.width = 100;
            npc.height = 150;
            npc.defense = 15;
            npc.value = Item.buyPrice(0, 40, 0, 0);
            npc.LifeMaxNERB(41000, 49200, 780000);
			npc.DR_NERD(0.15f);
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.damage *= 3;
                npc.defense *= 4;
                npc.lifeMax *= 5;
                npc.value *= 3f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath39;
            music = CalamityMod.Instance.GetMusicFromMusicMod("BrimstoneElemental") ?? MusicID.Boss4;
            bossBag = ModContent.ItemType<BrimstoneWaifuBag>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToWater = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityAI.BrimstoneElementalAI(npc, mod);
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180, true);
        }

        public override void FindFrame(int frameHeight) // 9 total frames
        {
            npc.frameCounter += 1.0;
            if (npc.ai[0] <= 2f)
            {
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 4)
                {
                    npc.frame.Y = 0;
                }
            }
            else if (npc.ai[0] == 3f || npc.ai[0] == 5f)
            {
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y < frameHeight * 4)
                {
                    npc.frame.Y = frameHeight * 4;
                }
                if (npc.frame.Y >= frameHeight * 8)
                {
                    npc.frame.Y = frameHeight * 4;
                }
            }
            else
            {
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y < frameHeight * 8)
                {
                    npc.frame.Y = frameHeight * 8;
                }
                if (npc.frame.Y >= frameHeight * 12)
                {
                    npc.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<BrimstoneElementalTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBrimstoneCrag>(), true, !CalamityWorld.downedBrimstoneElemental);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBrimstoneElemental>(), true, !CalamityWorld.downedBrimstoneElemental);

			CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Wizard }, CalamityWorld.downedBrimstoneElemental);

			if (!Main.expertMode)
            {
				//Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EssenceofChaos>(), 4, 8);
                if (CalamityWorld.downedProvidence)
				    DropHelper.DropItemSpray(npc, ModContent.ItemType<Bloodstone>(), 20, 30, 2);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<Brimlance>(w),
                    DropHelper.WeightStack<SeethingDischarge>(w),
                    DropHelper.WeightStack<DormantBrimseeker>(w),
					DropHelper.WeightStack<RoseStone>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<Gehenna>(), true);
                DropHelper.DropItem(npc, ModContent.ItemType<Abaddon>(), true);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<Brimrose>(), CalamityWorld.downedProvidence);

				// Vanity
				DropHelper.DropItemChance(npc, ModContent.ItemType<BrimstoneWaifuMask>(), 7);
            }

            DropHelper.DropItemCondition(npc, ModContent.ItemType<FabledTortoiseShell>(), !Main.expertMode, 0.1f);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Hellborn>(), !Main.expertMode, 0.1f);

            // mark brimmy as dead
            CalamityWorld.downedBrimstoneElemental = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 200;
                npc.height = 150;
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
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore4"), 1f);
            }
        }
    }
}
