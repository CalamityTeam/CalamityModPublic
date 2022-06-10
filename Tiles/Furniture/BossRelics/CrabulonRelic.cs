using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossRelics
{
    public class CrabulonRelic : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityMod/Tiles/Furniture/BossRelics/CrabulonRelic";

        public override int AssociatedItem => ModContent.ItemType<Items.Placeables.Furniture.BossRelics.CrabulonRelic>();
    }
}
