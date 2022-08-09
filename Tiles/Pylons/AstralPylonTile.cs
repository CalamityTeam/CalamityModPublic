using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Systems;
using CalamityMod.Buffs.Placeables;
using CalamityMod.TileEntities;
using CalamityMod.Items.Placeables.Pylons;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using CalamityMod.Tiles.BaseTiles;

namespace CalamityMod.Tiles.Pylons
{
    public class AstralPylonTile : BasePylonTile
    {
        public override Color LightColor => new Color(0.8f, 0.5f, 0.8f);
        public override string PylonMapText => "Mods.CalamityMod.ItemName.AstralPylon";
        public override int AssociatedItem => ModContent.ItemType<AstralPylon>();
        public override Color PylonMapColor => Color.Coral;
        public override Color DustColor => Main.rand.NextBool() ? Color.Coral : Color.MediumTurquoise;


        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            return isNPCHappyEnough && player.Calamity().ZoneAstral ? AssociatedItem : null;
        }


        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.AstralTiles >= 100;
    }
}
