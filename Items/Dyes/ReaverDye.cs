using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ReaverDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/ReaverDyeShader")), "DyePass").
            UseColor(new Color(54, 164, 66)).UseSecondaryColor(new Color(224, 115, 65));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Lime;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<PerennialOre>(), 4);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
