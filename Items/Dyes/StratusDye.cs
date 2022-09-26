using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class StratusDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/StratusDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(36, 86, 163)).UseSecondaryColor(new Color(124, 204, 223)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Stratus Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(0, 4, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<RuinousSoul>().
                AddIngredient<ExodiumCluster>().
                AddIngredient<Lumenyl>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
