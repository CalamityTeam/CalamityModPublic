using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class CoralskinFoolfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coralskin Foolfish"); //Potion material
            Tooltip.SetDefault("Camouflage is one of nature's best defenses");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 7);
            Item.rare = ItemRarityID.Green;
        }
    }
}
