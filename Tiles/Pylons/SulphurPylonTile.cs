using CalamityMod.Items.Placeables.Pylons;
using CalamityMod.Systems;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Pylons
{

    public class SulphurPylonTile : BasePylonTile
    {
        public override int AssociatedItem => ModContent.ItemType<SulphurPylon>();
        public override string PylonMapText => "Mods.CalamityMod.ItemName.SulphurPylon";
        public override Color PylonMapColor => Color.YellowGreen;
        public override Color DustColor => Color.GreenYellow;
        public override Color LightColor => new Color(1f, 0.8f, 0f);

        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            return isNPCHappyEnough && player.Calamity().ZoneSulphur ? AssociatedItem : null;
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.SulphurTiles >= 100;
    }
}
