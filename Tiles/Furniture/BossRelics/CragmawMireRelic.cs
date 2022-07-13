using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossRelics
{
    public class CragmawMireRelic : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityMod/Tiles/Furniture/BossRelics/CragmawMireRelic";

        public override int AssociatedItem => ModContent.ItemType<Items.Placeables.Furniture.BossRelics.CragmawMireRelic>();
    }
}
