using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossRelics
{
    public class AstrumAureusRelic : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityMod/Tiles/Furniture/BossRelics/AstrumAureusRelic";

        public override int AssociatedItem => ModContent.ItemType<Items.Placeables.Furniture.BossRelics.AstrumAureusRelic>();
    }
}
