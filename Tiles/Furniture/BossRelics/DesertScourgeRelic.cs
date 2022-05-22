using System;
using CalamityMod.Tiles.BaseTiles;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.BossRelics
{
    public class DesertScourgeRelic : BaseBossRelic
    {
        public override string RelicTextureName => "CalamityMod/Tiles/Furniture/BossRelics/DesertScourgeRelic";

        public override int AssociatedItem => ModContent.ItemType<Items.Placeables.Furniture.BossRelics.DesertScourgeRelic>();
    }
}
