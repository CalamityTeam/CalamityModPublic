using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class EndothermicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/EndothermicDyeShader")), "DyePass").
            UseColor(new Color(123, 205, 237)).UseSecondaryColor(new Color(85, 85, 171));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Endothermic Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
