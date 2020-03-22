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
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Bumblebirb
{
    [AutoloadBossHead]
    public class Bumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bumblebirb");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/Birb";
        public override string BossHeadTexture => "CalamityMod/NPCs/Bumblebirb/Birb_Head_Boss";

        public override void SetDefaults()
        {
            npc.npcSlots = 32f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 160;
            npc.width = 130;
            npc.height = 100;
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
            npc.visualOffset = new Vector2(20f, 50f);
			CalamityAI.BumblebirbAI(npc, mod);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            bool lightningBreathAttack = false; //leaving this for fabbo to sort
            if (lightningBreathAttack)
            {
                npc.frame.Y = npc.frame.Height * 5;
            }
            else
            {
                if (npc.frameCounter >= 5) //iban said the time between frames was 5 so using that as a base
                {
                    if (npc.frame.Y >= npc.frame.Height * 4) //frame 5 or 6 for transitioning from open jaw
                    {
                        npc.frame.Y = 0;
                    }
                    else
                    {
                        npc.frame.Y += npc.frame.Height;
                    }
                    npc.frameCounter = -1; //set to -1 to account for the framecounter increment shortly after
                }
            }
            npc.frameCounter++;
			if (npc.ai[0] == 3.2f)
				npc.frameCounter += 0.5;
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
                for (int i = 0; i < 6; i++) // 1 head, 1 wing, 4 legs = 6. one wing due to them being chonky boyes now
                {
                    string gore = "Gores/Bumble";
                    float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                    gore += i == 0 ? "Head" : i > 1 ? "Leg" : "Wing";
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot(gore), 1f);
                }
            }
        }
    }
}
