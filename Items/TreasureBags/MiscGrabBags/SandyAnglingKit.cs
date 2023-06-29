using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    public class SandyAnglingKit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void SetStaticDefaults()
        {
                       Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
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
            normalOnly.Add(ItemID.FishermansGuide, 20);
            normalOnly.Add(ItemID.WeatherRadio, 20);
            normalOnly.Add(ItemID.Sextant, 20);
            normalOnly.Add(ItemID.FishingPotion, 3, 2, 3);
            normalOnly.Add(ItemID.SonarPotion, 3, 2, 3);
            normalOnly.Add(ItemID.CratePotion, 3, 2, 3);
            normalOnly.Add(ItemID.GoldCoin, 1, 1, 2);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ItemID.FishermansGuide, 16);
            expertPlus.Add(ItemID.WeatherRadio, 16);
            expertPlus.Add(ItemID.Sextant, 16);
            expertPlus.Add(ItemID.FishingPotion, 2, 2, 3);
            expertPlus.Add(ItemID.SonarPotion, 2, 2, 3);
            expertPlus.Add(ItemID.CratePotion, 2, 2, 3);
            expertPlus.Add(ItemID.GoldCoin, 1, 2, 3);
        }
    }
}
