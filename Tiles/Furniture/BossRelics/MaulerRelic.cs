using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossRelics
{
    public class MaulerRelic : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityMod/Tiles/Furniture/BossRelics/MaulerRelic";

        public override int AssociatedItem => ModContent.ItemType<Items.Placeables.Furniture.BossRelics.MaulerRelic>();
    }
}
