using CalamityMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.GameContent;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralPalmTree : ModPalmTree
    {
        public override void SetStaticDefaults()
        {
            // Grows on astral sand
            GrowsOnTileId = new int[1] { ModContent.TileType<AstralSand>() };
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

        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralPalmTree");

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralPalmTree_Tops");


        //I don't know what this means. Why do palm trees have branches?? Since when. Will ask spriters for an oasis alt later
        public override Asset<Texture2D> GetOasisTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralPalmTree_OasisTops");

        public override int DropWood() => ModContent.ItemType<Items.Placeables.AstralMonolith>();

        public override int CreateDust() => ModContent.DustType<AstralBasic>();

        public override int GrowthFXGore() => -1;

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AstralPalmSapling>();
        }
    }
}
