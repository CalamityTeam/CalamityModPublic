using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Dyes
{
    public class BrimflameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/BrimflameDyeShader")), "DyePass").
            UseColor(new Color(252, 147, 34)).UseSecondaryColor(new Color(216, 41, 26)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Brimflame Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 0, 75, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 3);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
