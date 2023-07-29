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
        public override int AssociatedItem => ModContent.ItemType<SunkenPylon>();
        public override Color PylonMapColor => Color.Turquoise;
        public override Color DustColor => Color.Cyan;

        public override NPCShop.Entry GetNPCShopEntry()
        {
            Condition biomeCondition = new Condition(CalamityUtils.GetText("Condition.InSunken"), () => Main.LocalPlayer.Calamity().ZoneSunkenSea);
            return new NPCShop.Entry(AssociatedItem, Condition.AnotherTownNPCNearby, biomeCondition);
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.SunkenSeaTiles >= 100;
    }
}
