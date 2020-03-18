using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;

namespace CalamityMod.NPCs.Bumblebirb
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
            npc.LifeMaxNERB(227500, 252500, 3000000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
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
            bossBag = ModContent.ItemType<BumblebirbBag>();
        }

        public override void AI()
        {
			CalamityAI.BumblebirbAI(npc, mod);
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

            DropHelper.DropItemChance(npc, ModContent.ItemType<BumblebirbTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBumblebirb>(), true, !CalamityWorld.downedBumble);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBumble, 5, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EffulgentFeather>(), 6, 11);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<GildedProboscis>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GoldenEagle>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RougeSlash>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Swordsplosion>(), DropHelper.RareVariantDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<BumblefuckMask>(), 7);
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
