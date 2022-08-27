using CalamityMod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    [LegacyName("FleshyGeodeT1")]
    public class FleshyGeode : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fleshy Geode");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Yellow;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Different drop rates on Normal and Expert, so define normal first, then expert
            // 1-3 bars on Normal, 2-3 bars on Expert
            // 1-2 cores on Normal, 1-3 cores on Expert
            var normalOnly = itemLoot.DefineNormalOnlyDropSet();
            normalOnly.Add(ModContent.ItemType<CryonicBar>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<PerennialBar>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<ScoriaBar>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<CoreofEleum>(), 1, 1, 2);
            normalOnly.Add(ModContent.ItemType<CoreofSunlight>(), 1, 1, 2);
            normalOnly.Add(ModContent.ItemType<CoreofChaos>(), 1, 1, 2);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ModContent.ItemType<CryonicBar>(), 1, 2, 3);
            expertPlus.Add(ModContent.ItemType<PerennialBar>(), 1, 2, 3);
            expertPlus.Add(ModContent.ItemType<ScoriaBar>(), 1, 2, 3);
            expertPlus.Add(ModContent.ItemType<CoreofEleum>(), 1, 1, 3);
            expertPlus.Add(ModContent.ItemType<CoreofSunlight>(), 1, 1, 3);
            expertPlus.Add(ModContent.ItemType<CoreofChaos>(), 1, 1, 3);
        }
    }
}
