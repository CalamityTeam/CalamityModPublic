using CalamityMod.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class EffigyOfDecay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effigy of Decay");
            Tooltip.SetDefault("When placed down nearby players can breath underwater\n" +
                               "This effect does not work in the abyss\n" +
                               "Nearby players are also immune to the sulphuric poisoning");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.createTile = ModContent.TileType<EffigyOfDecayPlaceable>();
        }
    }
}
