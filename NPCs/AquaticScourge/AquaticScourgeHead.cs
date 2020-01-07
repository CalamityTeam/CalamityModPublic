using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;

namespace CalamityMod.NPCs.AquaticScourge
{
    [AutoloadBossHead]
    public class AquaticScourgeHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 16f;
            npc.damage = 80;
            npc.width = 100;
            npc.height = 90;
            npc.defense = 10;
            npc.Calamity().RevPlusDR(0.1f);
            npc.aiStyle = -1;
            aiType = -1;
            npc.LifeMaxNERD(73000, 85000, 100000, 10000000, 11000000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 12, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = ModContent.ItemType<AquaticScourgeBag>();
            if (Main.expertMode)
            {
                npc.scale = 1.15f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
			if (npc.justHit || (double)npc.life <= (double)npc.lifeMax * 0.99 || CalamityWorld.bossRushActive)
			{
				Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
				if (calamityModMusic != null)
					music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AquaticScourge");
				else
					music = MusicID.Boss2;
			}
			CalamityAI.AquaticScourgeAI(npc, mod, true);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
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

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return npc.Calamity().newAI[0] == 1f;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
                    return 0.01f;
            }
            return 0f;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SulphurousSand>();
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
			DropHelper.DropItemChance(npc, ModContent.ItemType<AquaticScourgeTrophy>(), 10);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAquaticScourge>(), true, !CalamityWorld.downedAquaticScourge);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSulphurSea>(), true, !CalamityWorld.downedAquaticScourge);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedAquaticScourge, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ItemID.SoulofSight, 20, 40);
                DropHelper.DropItem(npc, ModContent.ItemType<VictoryShard>(), 11, 20);
                DropHelper.DropItem(npc, ItemID.Coral, 5, 9);
                DropHelper.DropItem(npc, ItemID.Seashell, 5, 9);
                DropHelper.DropItem(npc, ItemID.Starfish, 5, 9);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<SubmarineShocker>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Barinautical>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Downpour>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DeepseaStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ScourgeoftheSeas>(), 4);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<AeroStone>(), 9);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<AquaticScourgeMask>(), 7);

                // Fishing
                DropHelper.DropItemChance(npc, ItemID.HighTestFishingLine, 12);
                DropHelper.DropItemChance(npc, ItemID.AnglerTackleBag, 12);
                DropHelper.DropItemChance(npc, ItemID.TackleBox, 12);
                DropHelper.DropItemChance(npc, ItemID.AnglerEarring, 9);
                DropHelper.DropItemChance(npc, ItemID.FishermansGuide, 9);
                DropHelper.DropItemChance(npc, ItemID.WeatherRadio, 9);
                DropHelper.DropItemChance(npc, ItemID.Sextant, 9);
                DropHelper.DropItemChance(npc, ItemID.AnglerHat, 4);
                DropHelper.DropItemChance(npc, ItemID.AnglerVest, 4);
                DropHelper.DropItemChance(npc, ItemID.AnglerPants, 4);
                DropHelper.DropItemChance(npc, ItemID.FishingPotion, 4, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.SonarPotion, 4, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.CratePotion, 4, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.GoldenBugNet, 15, 1, 1);
            }

            // Mark Aquatic Scourge as dead
            CalamityWorld.downedAquaticScourge = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASHead"), 1f);
            }
        }

        public override bool CheckActive()
        {
            if (npc.Calamity().newAI[0] == 1f && !Main.player[npc.target].dead && npc.Calamity().newAI[1] != 1f)
            {
                return false;
            }
            if (npc.timeLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = (int)npc.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
                {
                    if (Main.npc[k].active)
                    {
                        Main.npc[k].active = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Main.npc[k].life = 0;
                            Main.npc[k].netSkip = -1;
                            NetMessage.SendData(23, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 360, true);
            player.AddBuff(BuffID.Venom, 360, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            }
        }
    }
}
