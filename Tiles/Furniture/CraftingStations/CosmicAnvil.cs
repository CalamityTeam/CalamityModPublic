﻿using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class CosmicAnvil : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            // You cannot face the Cosmic Anvil left or right, it only has one orientation
            TileObjectData.newTile.Direction = Terraria.Enums.TileObjectDirection.None;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(159, 125, 201), CalamityUtils.GetItemName<Items.Placeables.Furniture.CraftingStations.CosmicAnvilItem>());
            TileID.Sets.DisableSmartCursor[Type] = true;
            // Visual Studio complains about this line. However, if you change it to DustID.BubbleBurst_Purple, it won't compile.
            DustType = 179;
            AdjTiles = new int[] { TileID.Anvils, TileID.MythrilAnvil };
        }
    }
}
