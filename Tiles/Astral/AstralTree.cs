using CalamityMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;

namespace CalamityMod.Tiles.Astral
{
    public class AstralTree : ModTree
    {
        public override void SetStaticDefaults()
        {
            // Grows on astral grass
            GrowsOnTileId = new int[1] { ModContent.TileType<AstralGrass>() };
        }

        //Copypasted from vanilla, just as ExampleMod did, due to the lack of proper explanation
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Astral/AstralTree_Tops");

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            //What does this code do?
            //treeFrame = (i + j * j) % 3;
        }

        public override Asset<Texture2D> GetBranchTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Astral/AstralTree_Branches");
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Astral/AstralTree");
        public override int DropWood() => ModContent.ItemType<Items.Placeables.AstralMonolith>();
        public override int CreateDust() => ModContent.DustType<AstralBasic>();

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AstralTreeSapling>();
        }
    }
}
