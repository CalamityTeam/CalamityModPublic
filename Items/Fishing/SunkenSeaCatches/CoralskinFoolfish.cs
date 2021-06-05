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
            item.width = 28;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 7);
            item.rare = ItemRarityID.Green;
        }
    }
}
