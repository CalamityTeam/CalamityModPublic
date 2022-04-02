using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class AstralSwirlDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/AstralSwirlDyeShader")), "DyePass").
            UseColor(new Color(42, 147, 154)).UseSecondaryColor(new Color(238, 93, 82)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Swirl Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(0, 3, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralBlueDye>());
            recipe.AddIngredient(ModContent.ItemType<AstralOrangeDye>());
            recipe.AddIngredient(ModContent.ItemType<AstralOre>(), 5);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}