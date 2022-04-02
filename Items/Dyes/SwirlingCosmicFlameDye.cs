using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class SwirlingCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/CosmicFlameShader")), "DyePass").
            UseColor(new Color(52, 212, 229)).UseSecondaryColor(new Color(255, 115, 221)).UseImage("Images/Misc/Noise").UseSaturation(1f);
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Swirling Cosmic Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = Item.sellPrice(0, 7, 50, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlueCosmicFlameDye>());
            recipe.AddIngredient(ModContent.ItemType<PinkCosmicFlameDye>());
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
