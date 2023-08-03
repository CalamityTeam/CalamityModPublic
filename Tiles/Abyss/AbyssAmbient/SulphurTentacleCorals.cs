using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class SulphurTentacleCorals : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.SwaysInWindBasic[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(32, 65, 65));
            DustType = 32;

            base.SetStaticDefaults();
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -30;
            height = 48;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX <= 324)
            {
                float brightness = 0.7f;
                float declareThisHereToPreventRunningTheSameCalculationMultipleTimes = Main.GameUpdateCount * 0.0025f;
                brightness *= (float)MathF.Sin(-j / 2f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes + i);
                brightness *= (float)MathF.Sin(-i / 2f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes + j);
                brightness += 0.7f;
                r = 0.26f;
                g = 0.4f;
                b = 0.41f;
                r *= brightness;
                g *= brightness;
                b *= brightness;
            }

            if (tile.TileFrameX > 324)
            {
                float brightness = 0.7f;
                float declareThisHereToPreventRunningTheSameCalculationMultipleTimes = Main.GameUpdateCount * 0.0025f;
                brightness *= (float)MathF.Sin(-j / 2f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes + i);
                brightness *= (float)MathF.Sin(-i / 2f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes + j);
                brightness += 0.7f;
                r = 0.46f;
                g = 0.51f;
                b = 0f;
                r *= brightness;
                g *= brightness;
                b *= brightness;
            }
       }
    }
}
