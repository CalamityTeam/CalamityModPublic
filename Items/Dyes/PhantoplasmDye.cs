using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class PhantoplasmDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/PhantoplasmDyeShader")), "DyePass").
            UseColor(new Color(245, 143, 182)).UseSecondaryColor(new Color(119, 238, 255)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Phantoplasm Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 2, 50, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 3);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
