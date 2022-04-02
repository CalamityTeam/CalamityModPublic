using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables.Ores;

namespace CalamityMod.Items.Dyes
{
    public class StratusDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/StratusDyeShader")), "DyePass").
            UseColor(new Color(36, 86, 163)).UseSecondaryColor(new Color(124, 204, 223)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.value = Item.sellPrice(0, 4, 50, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>());
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>());
            recipe.AddIngredient(ModContent.ItemType<Lumenite>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}