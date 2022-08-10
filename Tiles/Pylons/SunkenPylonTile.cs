using CalamityMod.Items.Placeables.Pylons;
using CalamityMod.Systems;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Pylons
{

    public class SunkenPylonTile : BasePylonTile
    {
        public override Color LightColor => new Color(0.2f, 0.8f, 1f);
        public override string PylonMapText => "Mods.CalamityMod.ItemName.SunkenPylon";
        public override int AssociatedItem => ModContent.ItemType<SunkenPylon>();
        public override Color PylonMapColor => Color.Turquoise;
        public override Color DustColor => Color.Cyan;

        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            // Pylon will sell regardless of NPC happiness.
            return isNPCHappyEnough && player.Calamity().ZoneSunkenSea ? AssociatedItem : null;
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.SunkenSeaTiles >= 100;
    }
}
