using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    public class BleachedAnglingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleached Angling Kit");
            Tooltip.SetDefault("Has a chance to contain various fishing gear\n" +
            "{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Different drop rates on Normal and Expert, so define normal first, then expert
            var normalOnly = itemLoot.DefineNormalOnlyDropSet();
            normalOnly.Add(ItemID.AnglerTackleBag, 72);
            normalOnly.Add(ItemID.HighTestFishingLine, 48);
            normalOnly.Add(ItemID.TackleBox, 48);
            normalOnly.Add(ItemID.AnglerEarring, 48);
            normalOnly.Add(ItemID.FishermansGuide, 36);
            normalOnly.Add(ItemID.WeatherRadio, 36);
            normalOnly.Add(ItemID.Sextant, 36);
            normalOnly.Add(ItemID.AnglerHat, 16);
            normalOnly.Add(ItemID.AnglerVest, 16);
            normalOnly.Add(ItemID.AnglerPants, 16);
            normalOnly.Add(ItemID.FishingPotion, 3, 2, 3);
            normalOnly.Add(ItemID.SonarPotion, 3, 2, 3);
            normalOnly.Add(ItemID.CratePotion, 3, 2, 3);
            normalOnly.Add(ItemID.GoldenBugNet, 60);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ItemID.AnglerTackleBag, 58);
            expertPlus.Add(ItemID.HighTestFishingLine, 38);
            expertPlus.Add(ItemID.TackleBox, 38);
            expertPlus.Add(ItemID.AnglerEarring, 38);
            expertPlus.Add(ItemID.FishermansGuide, 28);
            expertPlus.Add(ItemID.WeatherRadio, 28);
            expertPlus.Add(ItemID.Sextant, 28);
            expertPlus.Add(ItemID.AnglerHat, 12);
            expertPlus.Add(ItemID.AnglerVest, 12);
            expertPlus.Add(ItemID.AnglerPants, 12);
            expertPlus.Add(ItemID.FishingPotion, 2, 2, 3);
            expertPlus.Add(ItemID.SonarPotion, 2, 2, 3);
            expertPlus.Add(ItemID.CratePotion, 2, 2, 3);
            expertPlus.Add(ItemID.GoldenBugNet, 48);
        }
    }
}
