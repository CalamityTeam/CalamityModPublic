using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.TownNPCs
{
	[AutoloadHead]
    public class DILF : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archmage");

            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.lavaImmune = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 20000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.8f;
            animationType = NPCID.Guide;
        }

		public override void AI()
		{
			if (!CalamityWorld.foundHomePermafrost && !npc.homeless)
			{
				CalamityWorld.foundHomePermafrost = true;
			}
		}

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => CalamityWorld.downedCryogen;

        public override string TownNPCName() => "Permafrost";

        public override string GetChat()
        {
            if (npc.homeless && !CalamityWorld.foundHomePermafrost)
            {
                if (Main.rand.NextBool(2))
                    return "I deeply appreciate you rescuing me from being trapped within my frozen castle... It's been many, many years...";
                else
                    return "Thank you for saving me...though now I admit I am without a home since mine got destroyed.";
            }

            IList<string> dialogue = new List<string>();

            if (Main.dayTime)
            {
                dialogue.Add("I must admit...I am not quite used to this weather. It's too warm for my taste...");
                dialogue.Add("My dear! What is it you would like to talk about today?");
                dialogue.Add("Why...I don't have to worry about any time of the day! If it is hot...then I can use my ice magic to cool down!");
                dialogue.Add("I do usually prefer a spot of humidity for my ice magic. It likes to come out as steam when it's too hot and dry...");
                dialogue.Add("Magic is a skill that must be learned and practiced! You seem to have an inherent talent for it at your age. I have spent all of my life honing it...");
                dialogue.Add("Why ice magic, you ask? Well, my parents were both pyromaniacs...");
            }
            else
            {
                dialogue.Add("There be monsters lurking in the darkness. Most...unnatural monsters.");
                dialogue.Add("You could break the icy stillness in the air tonight.");
                dialogue.Add("Hmm...some would say that an unforeseen outside force is the root of the blood moon...");
                dialogue.Add("I was once the greatest Archmage of ice that ever hailed the lands. Whether or not that is still applicable, I am not sure...");
                dialogue.Add("There used to be other Archmages of other elements. I wonder where they are now...if they are also alive...");
                dialogue.Add("Oh...I wish I could tell you all about my life and the lessons I have learned, but it appears you have a great many things to do...");
            }

            dialogue.Add("I assure you, I will do my best to act as the cool grandfather figure you always wanted.");

            if (BirthdayParty.PartyIsUp)
                dialogue.Add("Sometimes...I feel like all I'm good for during these events is making ice cubes and slushies.");

            if (NPC.downedMoonlord)
            {
                dialogue.Add("Tread carefully, my friend... Now that the Moon Lord has been defeated, many powerful creatures will crawl out to challenge you...");
                dialogue.Add("I feel the balance of nature tilting farther than ever before. Is it due to you, or because of the events leading to now...?");
            }

            if (CalamityWorld.downedPolterghast)
                dialogue.Add("I felt a sudden chill down my spine. I sense something dangerous stirring in the Abyss...");

            if (CalamityWorld.downedYharon)
                dialogue.Add("...Where is Lord Yharim? He must be up to something...");

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad != -1)
                dialogue.Add("Yes, I am older than " + Main.npc[dryad].GivenName + ". You can stop asking now...");

            if (Main.player[Main.myPlayer].Calamity().chibii)
                dialogue.Add("What an adorable tiny companion you have! Where in the world did you find such a...creature...? Actually, I'd rather not know.");

            if (Main.player[Main.myPlayer].Calamity().cryogenSoul)
                dialogue.Add(Main.player[Main.myPlayer].name + "...just between us, I think I forgot my soul in the ice castle. If you see it, please do let me know.");

            if (Main.hardMode)
                dialogue.Add("It wouldn't be the first time something unknown and powerful dropped from the heavens...I would tread carefully if I were you...");

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
			if (firstButton)
			{
				Main.LocalPlayer.Calamity().newPermafrostInventory = false;
				shop = true;
			}
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<ColdheartIcicle>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostbiteBlaster>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcicleTrident>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<IceStar>());
            nextSlot++;
			if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<ArcticBearPaw>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<CryogenicStaff>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostyFlare>());
				nextSlot++;
			}
			if (NPC.downedChristmasIceQueen && NPC.downedChristmasTree && NPC.downedChristmasSantank)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<AbsoluteZero>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<EternalBlizzard>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<WintersFury>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcyBullet>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcicleArrow>());
				nextSlot++;
			}
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PermafrostsConcoction>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.SuperManaPotion);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeliciousMeat>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<EnchantedMetal>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Popo>());
            shop.item[nextSlot].shopCustomPrice = 1000000; //I think this is 5 platinum
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CryoKey>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 15, 0, 0);
            nextSlot++;
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<IceBarrage>()))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodRune>());
                nextSlot++;
            }
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
