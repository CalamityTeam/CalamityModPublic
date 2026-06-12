using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
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
            get => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ExoDyeShader"), "DyePass").UseImage("Images/Misc/Perlin");
        }

        public override void SafeSetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.value = Item.sellPrice(gold: 2, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<ExoPrism>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
