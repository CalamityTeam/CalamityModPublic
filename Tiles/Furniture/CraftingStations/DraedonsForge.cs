﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class DraedonsForge : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4); // Uses 5x4 style, but reduces height to 3.
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.newTile.Origin = new Point16(2, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(230, 157, 41), CalamityUtils.GetItemName<Items.Placeables.Furniture.CraftingStations.DraedonsForge>());

            AnimationFrameHeight = 54;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Platinum; // 84

            AdjTiles = new int[] {
                TileID.WorkBenches,
                TileID.Chairs,
                TileID.Tables,
                TileID.Anvils,
                TileID.MythrilAnvil,
                ModContent.TileType<CosmicAnvil>(),
                TileID.Furnaces,
                TileID.Hellforge,
                TileID.AdamantiteForge,
                TileID.TinkerersWorkbench,
                TileID.LunarCraftingStation,
                TileID.DemonAltar
            };
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 11)
            {
                frame = (frame + 1) % 4;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (float)Main.DiscoR / 255f;
            g = (float)Main.DiscoG / 255f;
            b = (float)Main.DiscoB / 255f;
        }
    }
}
