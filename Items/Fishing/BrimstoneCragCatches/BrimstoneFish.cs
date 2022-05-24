using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class BrimstoneFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fish"); //Future potion ingredient
            Tooltip.SetDefault("Fire is a living being");
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 8);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
