using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class ProcyonidPrawn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procyonid Prawn");
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Fish;
		}
    }
}
