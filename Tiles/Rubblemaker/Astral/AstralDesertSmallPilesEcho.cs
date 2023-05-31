
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Rubblemake.Astral
{
    public class AstralDesertSmallPilesEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/AstralDesert/AstralDesertSmallPiles";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = ModContent.DustType<AstralBasic>();
            AddMapEntry(new Color(79, 61, 97));

            RegisterItemDrop(ModContent.ItemType<AstralSand>());
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<AstralSand>(), Type, 0, 1, 2, 3, 4, 5);


            base.SetStaticDefaults();
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 4;
        }
    }
}
