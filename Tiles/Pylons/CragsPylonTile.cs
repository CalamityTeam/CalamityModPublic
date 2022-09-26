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
        public override string PylonMapText => "Mods.CalamityMod.ItemName.CragsPylon";
        public override int AssociatedItem => ModContent.ItemType<CragsPylon>();
        public override Color PylonMapColor => Color.OrangeRed;
        public override Color DustColor => Color.OrangeRed;

        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            //Purchaseable regardless of happiness, therefore, these NPCs need to be checked
			if (npcType == NPCID.TravellingMerchant || npcType == NPCID.SkeletonMerchant)
				return null;
            return player.Calamity().ZoneCalamity ? AssociatedItem : null;
        }

        //Doesnt require npcs to function
        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.BrimstoneCragTiles >= 100;
    }
}
