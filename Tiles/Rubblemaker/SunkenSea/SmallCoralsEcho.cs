using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class SmallCoralsEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/SmallCorals";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = false;
            Main.tileCut[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.SwaysInWindBasic[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(178, 28, 153));
            DustType = DustID.Coralstone;
            HitSound = SoundID.Grass;
            RegisterItemDrop(ItemID.CoralstoneBlock);
            // this one is weird
            //FlexibleTileWand.RubblePlacementSmall.AddVariations(ItemID.CoralstoneBlock, Type, 0, 1, 2, 3, 4, 5);

            base.SetStaticDefaults();
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -16;
            height = 32;
        }
    }
}
