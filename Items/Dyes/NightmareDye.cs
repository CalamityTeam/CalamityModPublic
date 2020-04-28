using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class NightmareDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/NightmareDyeShader")), "DyePass").
            UseColor(new Color(249, 81, 0)).UseSecondaryColor(new Color(255, 203, 106));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Nightmare Dye");
        }

		public override void SafeSetDefaults()
		{
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}