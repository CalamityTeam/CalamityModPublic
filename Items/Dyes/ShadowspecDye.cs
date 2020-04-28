using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class ShadowspecDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/DragonSoulDyeShader")), "DyePass").
            UseColor(new Color(46, 27, 60)).UseSecondaryColor(new Color(132, 142, 191)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowspec Dye");
        }

		public override void SafeSetDefaults()
		{
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Developer;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}