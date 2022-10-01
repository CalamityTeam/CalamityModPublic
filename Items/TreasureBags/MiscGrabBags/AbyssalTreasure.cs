using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    public class AbyssalTreasure : ModItem
    {
        internal static readonly int[] AbyssalTreasurePotions = new int[]
        {
            ItemID.SpelunkerPotion,
            ItemID.MagicPowerPotion,
            ItemID.ShinePotion,
            ItemID.WaterWalkingPotion,
            ItemID.ObsidianSkinPotion,
            ItemID.WaterWalkingPotion,
            ItemID.GravitationPotion,
            ItemID.RegenerationPotion,
            ModContent.ItemType<TriumphPotion>(),
            ModContent.ItemType<AnechoicCoating>(),
            ItemID.GillsPotion,
            ItemID.EndurancePotion,
            ItemID.HeartreachPotion,
            ItemID.FlipperPotion,
            ItemID.LifeforcePotion,
            ItemID.InfernoPotion
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Treasure");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue; //Blue for thematics
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;

        // TML 1.4 August Stable Grab Bag hook
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // 10% chance for potions
            var tenPercentPotions = itemLoot.Add(new OneFromOptionsNotScaledWithLuckDropRule(10, 1, AbyssalTreasurePotions));

            // 1/30 chance for a Wormhole Potion in multiplayer only
            IItemDropRule wormholePotion = ItemDropRule.ByCondition(DropHelper.If(() => Main.netMode == NetmodeID.MultiplayerClient), ItemID.WormholePotion, 30);
            LeadingConditionRule singlePlayer = new LeadingConditionRule(DropHelper.If(() => Main.netMode != NetmodeID.MultiplayerClient));

            // IF YOU DONT GET POTIONS
            // 10% chance for 4-8 Abyss Torches
            // 10% chance 10-20 Hellfire Arrows
            // 10% chance for 1 Hadal Stew
            // 10% chance for 1 Sticky Dynamite
            // 60% chance for 40-60 Silver Coins

            // 4-8 Abyss Torches
            CommonDrop torches = new CommonDrop(ModContent.ItemType<AbyssTorch>(), 1, 4, 8);
            // 10-20 Hellfire Arrows
            CommonDrop hellfireArrows = new CommonDrop(ItemID.HellfireArrow, 1, 10, 20);
            // 1 Hadal Stew
            CommonDrop hadalStew = new CommonDrop(ModContent.ItemType<HadalStew>(), 1);
            // 1 Sticky Dynamite
            CommonDrop stickyDynamite = new CommonDrop(ItemID.StickyDynamite, 1);
            // 40-60 Silver Coin
            CommonDrop silver = new CommonDrop(ItemID.SilverCoin, 1, 40, 60);

            OneFromRulesRule otherDrops = new OneFromRulesRule(1, new IItemDropRule[] { torches, hellfireArrows, hadalStew, stickyDynamite, silver, silver, silver, silver, silver, silver });

            tenPercentPotions.OnFailedRoll(wormholePotion).OnFailedRoll(otherDrops);
            tenPercentPotions.OnFailedRoll(singlePlayer).OnSuccess(otherDrops);
        }
    }
}
