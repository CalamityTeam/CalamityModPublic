using CalamityMod.Dusts;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityMod.Tiles.Abyss
{
    public class AcidWoodTree : ModPalmTree
    {
        public override void SetStaticDefaults()
        {
            // Grows on sulphurous sand
            GrowsOnTileId = new int[2] { ModContent.TileType<SulphurousSand>() , ModContent.TileType<SulphurousSandNoWater>()};
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

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeTops");
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTree");

        //I don't know what this means. Why do palm trees have branches?? Since when. Also acidwood trees arent meant to have oasis alts
        public override Asset<Texture2D> GetOasisTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeTops");

        public override int DropWood() => ModContent.ItemType<Acidwood>();
        public override int CreateDust() => (int)CalamityDusts.SulfurousSeaAcid;

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AcidWoodTreeSapling>();
        }
    }
}
