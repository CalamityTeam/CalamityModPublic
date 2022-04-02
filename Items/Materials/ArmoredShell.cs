using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ArmoredShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armored Shell");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 34;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }
    }
}
