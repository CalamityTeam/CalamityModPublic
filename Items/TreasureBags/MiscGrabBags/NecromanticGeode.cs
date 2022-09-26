using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    [LegacyName("FleshyGeodeT2")]
    public class NecromanticGeode : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromantic Geode");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Different drop rates on Normal and Expert, so define normal first, then expert
            // 5-10 bars on Normal, 7-12 bars on Expert
            // 1-3 cores on Normal, 2-4 cores on Expert
            // 50% chance of life alloy on Normal, 100% on Expert
            // 33% chance of core of calamity on Normal, 50% on Expert
            // 100-120 bloodstone on Normal, 120-140 bloodstone on Expert
            var normalOnly = itemLoot.DefineNormalOnlyDropSet();
            normalOnly.Add(ModContent.ItemType<CryonicBar>(), 1, 5, 10);
            normalOnly.Add(ModContent.ItemType<PerennialBar>(), 1, 5, 10);
            normalOnly.Add(ModContent.ItemType<ScoriaBar>(), 1, 5, 10);
            normalOnly.Add(ModContent.ItemType<CoreofEleum>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<CoreofSunlight>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<CoreofChaos>(), 1, 1, 3);
            normalOnly.Add(ModContent.ItemType<LifeAlloy>(), 2);
            normalOnly.Add(ModContent.ItemType<CoreofCalamity>(), 3);
            normalOnly.Add(ModContent.ItemType<Bloodstone>(), 1, 100, 120);

            var expertPlus = itemLoot.DefineConditionalDropSet(new Conditions.IsExpert());
            expertPlus.Add(ModContent.ItemType<CryonicBar>(), 1, 7, 12);
            expertPlus.Add(ModContent.ItemType<PerennialBar>(), 1, 7, 12);
            expertPlus.Add(ModContent.ItemType<ScoriaBar>(), 1, 7, 12);
            expertPlus.Add(ModContent.ItemType<CoreofEleum>(), 1, 2, 4);
            expertPlus.Add(ModContent.ItemType<CoreofSunlight>(), 1, 2, 4);
            expertPlus.Add(ModContent.ItemType<CoreofChaos>(), 1, 2, 4);
            expertPlus.Add(ModContent.ItemType<LifeAlloy>());
            expertPlus.Add(ModContent.ItemType<CoreofCalamity>(), 2);
            expertPlus.Add(ModContent.ItemType<Bloodstone>(), 1, 120, 140);
        }
    }
}
