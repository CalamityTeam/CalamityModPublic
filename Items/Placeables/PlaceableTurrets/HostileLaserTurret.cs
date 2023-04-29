using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlaceableTurrets
{
    public class HostileLaserTurret : ModItem
    {
        public override string Texture => "CalamityMod/Items/Placeables/PlaceableTurrets/LaserTurret";
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Blasts nearby players with lightning-fast laser beams");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DraedonStructures.HostileLaserTurret>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }
    }
}
