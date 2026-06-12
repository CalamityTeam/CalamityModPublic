using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Placeables.Furniture.Monoliths;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    [LegacyName("DILF")]
    public class Archmage : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Dislike);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
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
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Archmage")
            });
        }

        public override void AI()
        {
            if (!CalamityWorld.foundHomePermafrost && !NPC.homeless)
            {
                CalamityWorld.foundHomePermafrost = true;
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas.SupremeCalamitas>()) && Main.zenithWorld)
                return false;

            return DownedBossSystem.downedCryogen;
        }

        public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Permafrost") };

        public override string GetChat()
        {
            if (NPC.homeless && !CalamityWorld.foundHomePermafrost)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));

            if (Main.dayTime && !Main.LocalPlayer.ZoneSnow)
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

        public override void PartyHatPosition(ref Vector2 position, ref SpriteEffects spriteEffects) => position.X -= 5f * NPC.direction;

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
            NPCShop shop = new(Type);
                shop.Add<FrostbiteBlaster>()
                .Add<IcicleTrident>()
                .Add<IceStar>()
                .Add<ArcticBearPaw>(Condition.DownedMechBossAll)
                .Add<CryogenicStaff>(Condition.DownedMechBossAll)
                .Add<FrostyFlare>(Condition.DownedMechBossAll)
                .Add<Cryophobia>(Condition.DownedMechBossAll)
                .Add<AbsoluteZero>(Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add<EternalBlizzard>(Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add<WintersFury>(Condition.DownedEverscream, Condition.DownedSantaNK1, Condition.DownedIceQueen)
                .Add<PermafrostsConcoction>()
                .Add(ItemID.SuperManaPotion)
                .Add<Popo>()
                .Add<FrigidMonolith>()
                .Add<BloodRune>(Condition.PlayerCarriesItem(ModContent.ItemType<IceBarrage>()))
                .Register();
        }

        // Make this Town NPC teleport to the King statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 150;
            knockback = 9f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 15;
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
