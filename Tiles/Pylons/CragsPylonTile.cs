using CalamityMod.Items.Placeables.Pylons;
using CalamityMod.Systems;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Pylons
{

    public class CragsPylonTile : BasePylonTile
    {
        public override Color LightColor => new Color(1f, 0.3f, 0f);
        public override int AssociatedItem => ModContent.ItemType<CragsPylon>();
        public override Color PylonMapColor => Color.OrangeRed;
        public override Color DustColor => Color.OrangeRed;

        public override NPCShop.Entry GetNPCShopEntry()
        {
            Condition biomeCondition = new Condition(CalamityUtils.GetText("Condition.InCrag"), () => Main.LocalPlayer.Calamity().ZoneCalamity);
            return new NPCShop.Entry(AssociatedItem, biomeCondition);
        }

        //Doesnt require npcs to function
        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.BrimstoneCragTiles >= 100;
    }
}
