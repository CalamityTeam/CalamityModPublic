using CalamityMod.NPCs.AcidRain;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BloodwormItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Bloodworm");
            Tooltip.SetDefault("Summons The Old Duke if used as bait in the Sulphurous Sea\n" +
                "Enrages outside the Sulphurous Sea");
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.bait = 4444;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.makeNPC = (short)ModContent.NPCType<BloodwormNormal>();
            SacrificeTotal = 3;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}
    }
}
