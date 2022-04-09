using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables.Ores;
using ReLogic.Content;

namespace CalamityMod.Items.Dyes
{
    public class StratusDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/StratusDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(36, 86, 163)).UseSecondaryColor(new Color(124, 204, 223)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.value = Item.sellPrice(0, 4, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<RuinousSoul>().
                AddIngredient<ExodiumClusterOre>().
                AddIngredient<Lumenite>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
