using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SandyAnglingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandy Angling Kit");
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
            Item.rare = ItemRarityID.Blue;
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
            normalOnly.Add(ItemID.HighTestFishingLine, 15);
            normalOnly.Add(ItemID.TackleBox, 15);
            normalOnly.Add(ItemID.AnglerEarring, 15);
            normalOnly.Add(ItemID.FishermansGuide, 10);
            normalOnly.Add(ItemID.WeatherRadio, 10);
            normalOnly.Add(ItemID.Sextant, 10);
            normalOnly.Add(ItemID.AnglerHat, 5);
            normalOnly.Add(ItemID.AnglerVest, 5);
            normalOnly.Add(ItemID.AnglerPants, 5);
            normalOnly.Add(ItemID.FishingPotion, 5, 2, 3);
            normalOnly.Add(ItemID.SonarPotion, 5, 2, 3);
            normalOnly.Add(ItemID.CratePotion, 5, 2, 3);
            normalOnly.AddIf(() => NPC.downedBoss3, ItemID.GoldenBugNet, 20);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ItemID.HighTestFishingLine, 12);
            expertPlus.Add(ItemID.TackleBox, 12);
            expertPlus.Add(ItemID.AnglerEarring, 12);
            expertPlus.Add(ItemID.FishermansGuide, 9);
            expertPlus.Add(ItemID.WeatherRadio, 9);
            expertPlus.Add(ItemID.Sextant, 9);
            expertPlus.Add(ItemID.AnglerHat, 4);
            expertPlus.Add(ItemID.AnglerVest, 4);
            expertPlus.Add(ItemID.AnglerPants, 4);
            expertPlus.Add(ItemID.FishingPotion, 4, 2, 3);
            expertPlus.Add(ItemID.SonarPotion, 4, 2, 3);
            expertPlus.Add(ItemID.CratePotion, 4, 2, 3);
            expertPlus.AddIf(() => NPC.downedBoss3, ItemID.GoldenBugNet, 18);
        }
    }
}
