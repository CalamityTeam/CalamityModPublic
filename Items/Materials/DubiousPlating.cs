using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DubiousPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dubious Plating");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = 1;
        }
    }
}
