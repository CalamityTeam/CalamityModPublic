using CalamityMod.Dusts;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.Abyss
{
    public class AcidWoodTree : ModPalmTree
    {
        public override Texture2D GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeTops");
        public override Texture2D GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTree");
        public override int DropWood() => ModContent.ItemType<Acidwood>();
        public override int CreateDust() => (int)CalamityDusts.SulfurousSeaAcid;
    }
}
