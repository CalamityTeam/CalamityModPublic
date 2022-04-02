using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ElementalDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/ElementalDyeShader")), "DyePass").UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 2, 50, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SolarDye);
            recipe.AddIngredient(ItemID.VortexDye);
            recipe.AddIngredient(ItemID.StardustDye);
            recipe.AddIngredient(ItemID.NebulaDye);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
        }
    }
}
