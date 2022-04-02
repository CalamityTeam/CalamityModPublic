using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class CryonicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/CryonicDyeShader")), "DyePass").
            UseColor(new Color(138, 225, 255)).UseSecondaryColor(new Color(90, 90, 204)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 0, 75, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<CryonicOre>(), 4);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
