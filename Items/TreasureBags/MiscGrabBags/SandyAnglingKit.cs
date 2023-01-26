using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
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
            normalOnly.Add(ItemID.HighTestFishingLine, 60);
            normalOnly.Add(ItemID.TackleBox, 60);
            normalOnly.Add(ItemID.AnglerEarring, 60);
            normalOnly.Add(ItemID.FishermansGuide, 40);
            normalOnly.Add(ItemID.WeatherRadio, 40);
            normalOnly.Add(ItemID.Sextant, 40);
            normalOnly.Add(ItemID.AnglerHat, 20);
            normalOnly.Add(ItemID.AnglerVest, 20);
            normalOnly.Add(ItemID.AnglerPants, 20);
            normalOnly.Add(ItemID.FishingPotion, 3, 2, 3);
            normalOnly.Add(ItemID.SonarPotion, 3, 2, 3);
            normalOnly.Add(ItemID.CratePotion, 3, 2, 3);
            normalOnly.AddIf(() => NPC.downedBoss3, ItemID.GoldenBugNet, 80);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ItemID.HighTestFishingLine, 48);
            expertPlus.Add(ItemID.TackleBox, 48);
            expertPlus.Add(ItemID.AnglerEarring, 48);
            expertPlus.Add(ItemID.FishermansGuide, 32);
            expertPlus.Add(ItemID.WeatherRadio, 32);
            expertPlus.Add(ItemID.Sextant, 32);
            expertPlus.Add(ItemID.AnglerHat, 16);
            expertPlus.Add(ItemID.AnglerVest, 16);
            expertPlus.Add(ItemID.AnglerPants, 16);
            expertPlus.Add(ItemID.FishingPotion, 2, 2, 3);
            expertPlus.Add(ItemID.SonarPotion, 2, 2, 3);
            expertPlus.Add(ItemID.CratePotion, 2, 2, 3);
            expertPlus.AddIf(() => NPC.downedBoss3, ItemID.GoldenBugNet, 64);
        }
    }
}
