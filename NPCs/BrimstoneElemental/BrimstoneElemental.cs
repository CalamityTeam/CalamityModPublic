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
using Terraria.GameContent.ItemDropRules;
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
            Main.npcFrameCount[NPC.type] = 12;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 64f;
            NPC.GetNPCDamage();
            NPC.width = 100;
            NPC.height = 150;
            NPC.defense = 15;
            NPC.value = Item.buyPrice(0, 40, 0, 0);
            NPC.LifeMaxNERB(41000, 49200, 780000);
            NPC.DR_NERD(0.15f);
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.damage *= 3;
                NPC.defense *= 4;
                NPC.lifeMax *= 5;
                NPC.value *= 3f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath39;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("BrimstoneElemental") ?? MusicID.Boss4;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityAI.BrimstoneElementalAI(NPC, Mod);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180, true);
        }

        public override void FindFrame(int frameHeight) // 9 total frames
        {
            NPC.frameCounter += 1.0;
            if (NPC.ai[0] <= 2f)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
            else if (NPC.ai[0] == 3f || NPC.ai[0] == 5f)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }
            else
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
                if (NPC.frame.Y >= frameHeight * 12)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BrimstoneWaifuBag>()));

            npcLoot.Add(ModContent.ItemType<BrimstoneElementalTrophy>(), 10);
            npcLoot.AddLore(() => !DownedBossSystem.downedBrimstoneElemental, ModContent.ItemType<KnowledgeBrimstoneElemental>());

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Brimlance>(),
                    ModContent.ItemType<SeethingDischarge>(),
                    ModContent.ItemType<DormantBrimseeker>(),
                    ModContent.ItemType<RoseStone>(),
                };
                normalOnly.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, weapons));

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofChaos>(), 1, 4, 8);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<Gehenna>()));
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<Abaddon>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<BrimstoneWaifuMask>(), 7);
            }

            npcLoot.AddIf(() => !Main.expertMode, ModContent.ItemType<FabledTortoiseShell>(), 10);
            npcLoot.AddIf(() => !Main.expertMode, ModContent.ItemType<Hellborn>(), 10);
            npcLoot.AddIf(() => !Main.expertMode && DownedBossSystem.downedProvidence, ModContent.ItemType<Brimrose>());
            npcLoot.AddIf(() => !Main.expertMode && DownedBossSystem.downedProvidence, ModContent.ItemType<Bloodstone>(), 1, 20, 30);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Wizard }, DownedBossSystem.downedBrimstoneElemental);

            // Mark brimmy as dead
            DownedBossSystem.downedBrimstoneElemental = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 235, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore1").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore4").Type, 1f);
                }
            }
        }
    }
}
