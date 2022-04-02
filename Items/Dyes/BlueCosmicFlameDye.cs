using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class BlueCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/CosmicFlameShader")), "DyePass").
            UseColor(new Color(52, 212, 229)).UseSecondaryColor(new Color(52, 212, 229)).UseImage("Images/Misc/Noise").UseSaturation(0f);
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Cosmic Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 2);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
