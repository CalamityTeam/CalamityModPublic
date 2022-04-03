using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class InfectedArmorPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Armor Plating"); //haha recycled and recolored Corroded Metal Plating from T2 Acid Rain
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
