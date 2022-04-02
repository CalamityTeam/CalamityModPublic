using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class AstralOrangeDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/AstralOrangeDyeShader")), "DyePass").
            UseColor(new Color(255, 166, 94)).UseSecondaryColor(new Color(238, 93, 82));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Orange Dye");
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