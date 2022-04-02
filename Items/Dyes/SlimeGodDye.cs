using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class SlimeGodDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/SlimeGodDyeShader")), "DyePass").
            UseColor(new Color(80, 170, 206)).UseSecondaryColor(new Color(131, 58, 103)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Slime God Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(0, 0, 65, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PinkStatigelDye>());
            recipe.AddIngredient(ModContent.ItemType<BlueStatigelDye>());
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}