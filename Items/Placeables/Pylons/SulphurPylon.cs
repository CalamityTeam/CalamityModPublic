﻿using CalamityMod.Tiles.Pylons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Pylons
{
    public class SulphurPylon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SulphurPylonTile>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
