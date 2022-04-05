using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using CrateTile = CalamityMod.Tiles.Abyss.AbyssalCrateTile;

namespace CalamityMod.Items.Fishing.SulphurCatches
{
    public class AbyssalCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.createTile = ModContent.TileType<CrateTile>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            //Modded materials
            DropHelper.DropItem(player, ModContent.ItemType<SulphurousSand>(), 5, 10);
            DropHelper.DropItem(player, ModContent.ItemType<SulphurousSandstone>(), 5, 10);
            DropHelper.DropItem(player, ModContent.ItemType<HardenedSulphurousSandstone>(), 5, 10);
            DropHelper.DropItem(player, ModContent.ItemType<Acidwood>(), 5, 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<SulfuricScale>(), DownedBossSystem.downedEoCAcidRain, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CorrodedFossil>(), DownedBossSystem.downedAquaticScourgeAcidRain, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<DepthCells>(), DownedBossSystem.downedCalamitas, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Lumenite>(), DownedBossSystem.downedCalamitas, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<PlantyMush>(), DownedBossSystem.downedCalamitas, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Tenebris>(), DownedBossSystem.downedCalamitas, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CruptixBar>(), NPC.downedGolemBoss, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<ReaperTooth>(), DownedBossSystem.downedPolterghast && DownedBossSystem.downedBoomerDuke, 0.1f, 1, 5);

            // Weapons
            DropHelper.DropItemFromSetCondition(player, DownedBossSystem.downedSlimeGod || Main.hardMode, 0.1f,
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<BlackAnurian>());

            DropHelper.DropItemFromSetCondition(player, DownedBossSystem.downedAquaticScourgeAcidRain, 0.1f,
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<NuclearRod>(),
                ModContent.ItemType<SulphurousGrabber>(),
                ModContent.ItemType<FlakToxicannon>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SlitheringEels>(),
                ModContent.ItemType<BelchingSaxophone>());

            // Equipment
            DropHelper.DropItemFromSetCondition(player, DownedBossSystem.downedSlimeGod || Main.hardMode, 0.25f,
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<AnechoicPlating>(),
                ModContent.ItemType<TorrentialTear>());

            //Bait
            DropHelper.DropItemChance(player, ItemID.MasterBait, 10, 1, 2);
            DropHelper.DropItemChance(player, ItemID.JourneymanBait, 5, 1, 3);
            DropHelper.DropItemChance(player, ItemID.ApprenticeBait, 3, 2, 3);

            //Potions
            DropHelper.DropItemChance(player, ItemID.ObsidianSkinPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.SwiftnessPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.IronskinPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.NightOwlPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.ShinePotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.MiningPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.HeartreachPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.TrapsightPotion, 10, 1, 3); //Dangersense Potion
            DropHelper.DropItemChance(player, ModContent.ItemType<AnechoicCoating>(), 10, 1, 3);
            int healingPotID = ItemID.LesserHealingPotion;
            int manaPotID = ItemID.LesserManaPotion;
            if (DownedBossSystem.downedDoG)
            {
                healingPotID = ModContent.ItemType<SupremeHealingPotion>();
                manaPotID = ModContent.ItemType<SupremeManaPotion>();
            }
            else if (DownedBossSystem.downedProvidence)
            {
                healingPotID = ItemID.SuperHealingPotion;
                manaPotID = ItemID.SuperManaPotion;
            }
            else if (NPC.downedMechBossAny)
            {
                healingPotID = ItemID.GreaterHealingPotion;
                manaPotID = ItemID.GreaterManaPotion;
            }
            else if (NPC.downedBoss3)
            {
                healingPotID = ItemID.HealingPotion;
                manaPotID = ItemID.ManaPotion;
            }
            DropHelper.DropItemChance(player, Main.rand.NextBool(2) ? healingPotID : manaPotID, 0.25f, 2, 5);

            //Money
            DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
