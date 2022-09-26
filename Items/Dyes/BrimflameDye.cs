using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables;
using ReLogic.Content;

namespace CalamityMod.Items.Dyes
{
    public class BrimflameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/BrimflameDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(252, 147, 34)).UseSecondaryColor(new Color(216, 41, 26)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Brimflame Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 0, 75, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BrimstoneSlag>(3).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
