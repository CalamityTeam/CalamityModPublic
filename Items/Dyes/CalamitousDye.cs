using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class CalamitousDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/CalamitousDyeShader")), "DyePass").
            UseColor(new Color(227, 79, 79)).UseSecondaryColor(new Color(145, 27, 135)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitous Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = Item.sellPrice(0, 10, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 3);
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
