using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Items.Dyes
{
    public class CeaselessDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/CeaselessDyeShader")), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Ceaseless Dye");
        }

		public override void SafeSetDefaults()
		{
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.VoidDye);
            recipe.AddIngredient(ItemID.ShadowDye);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}