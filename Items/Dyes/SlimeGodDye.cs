using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using ReLogic.Content;

namespace CalamityMod.Items.Dyes
{
    public class SlimeGodDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/SlimeGodDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(80, 170, 206)).UseSecondaryColor(new Color(131, 58, 103)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Slime God Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 65, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<PinkStatigelDye>().
                AddIngredient<BlueStatigelDye>().
                AddIngredient<PurifiedGel>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
