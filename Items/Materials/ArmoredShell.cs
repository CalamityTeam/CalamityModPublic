using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ArmoredShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Armored Shell");
			ItemID.Sets.SortingPriorityMaterials[Type] = 107;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 34;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ModContent.RarityType<Turquoise>();
        }    }
}
