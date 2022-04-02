using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ExoDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind
        {
            get => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/ExoDyeShader")), "DyePass").UseImage("Images/Misc/Perlin");
        }

        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Dye");
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
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
