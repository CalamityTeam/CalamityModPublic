using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlaceableTurrets
{
    public class HostileFireTurret : ModItem
    {
        public override string Texture => "CalamityMod/Items/Placeables/PlaceableTurrets/FireTurret";
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Roasts nearby players with a flamethrower at full blast");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DraedonStructures.HostileFireTurret>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }
    }
}
