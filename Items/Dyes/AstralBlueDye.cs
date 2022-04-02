using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class AstralBlueDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/AstralBlueDyeShader")), "DyePass").
            UseColor(new Color(109, 242, 197)).UseSecondaryColor(new Color(42, 147, 154));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Blue Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<AstralOre>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}