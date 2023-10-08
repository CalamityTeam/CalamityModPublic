using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class DILF : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike) 
                .SetNPCAffection(NPCID.Wizard, AffectionLevel.Like) 
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Dislike);
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.8f;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,  
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DILF")
            });
        }

        public override void AI()
        {
            if (!CalamityWorld.foundHomePermafrost && !NPC.homeless)
            {
                CalamityWorld.foundHomePermafrost = true;
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => DownedBossSystem.downedCryogen;

		public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Permafrost") };

        public override string GetChat()
        {
            if (NPC.homeless && !CalamityWorld.foundHomePermafrost)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));

            if (Main.dayTime && !Main.player[Main.myPlayer].ZoneSnow)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Day1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day2"));
            }
            else if (!Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
            }

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"));

            if (Main.bloodMoon)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon1"));
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon2"));
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated1"));
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated2"));
            }

            return dialogue;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
        }

        public override void AddShops()
        {
            Condition potionSells = new(CalamityUtils.GetText("Condition.PotionConfig"), () => CalamityConfig.Instance.PotionSelling);
            NPCShop shop = new(Type);
                shop.Add(ModContent.ItemType<FrostbiteBlaster>())
                .Add(ModContent.ItemType<IcicleTrident>())
                .Add(ModContent.ItemType<IceStar>())
                .Add(ModContent.ItemType<ArcticBearPaw>(), Condition.DownedMechBossAll)
                .Add(ModContent.ItemType<CryogenicStaff>(), Condition.DownedMechBossAll)
                .Add(ModContent.ItemType<FrostyFlare>(), Condition.DownedMechBossAll)
                .Add(ModContent.ItemType<Cryophobia>(), Condition.DownedMechBossAll)
                .Add(ModContent.ItemType<AbsoluteZero>(), Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add(ModContent.ItemType<EternalBlizzard>(), Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add(ModContent.ItemType<WintersFury>(), Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add(ModContent.ItemType<IcyBullet>(), Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add(ModContent.ItemType<IcicleArrow>(), Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add(ModContent.ItemType<PermafrostsConcoction>())
                .Add(ItemID.SuperManaPotion)
                .Add(ModContent.ItemType<DeliciousMeat>())
                .AddWithCustomValue(ModContent.ItemType<Popo>(), Item.buyPrice(5))
                .Add(ModContent.ItemType<BloodRune>(), Condition.PlayerCarriesItem(ModContent.ItemType<IceBarrage>()))
                .Add(ItemID.IceCream, Condition.HappyEnough, Condition.InSnow)
                .Register();
        }

        // Make this Town NPC teleport to the King statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 9f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 50;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<DarkIce>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 2f;
        }
    }
}
