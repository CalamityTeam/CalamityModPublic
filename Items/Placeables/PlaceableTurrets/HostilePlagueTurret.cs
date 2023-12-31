﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlaceableTurrets
{
    public class HostilePlagueTurret : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override string Texture => "CalamityMod/Items/Placeables/PlaceableTurrets/PlagueTurret";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DraedonStructures.HostilePlagueTurret>());

            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
