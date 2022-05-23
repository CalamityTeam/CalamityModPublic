using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class AstralSwirlDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/AstralSwirlDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(42, 147, 154)).UseSecondaryColor(new Color(238, 93, 82)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Astral Swirl Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 3, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AstralBlueDye>().
                AddIngredient<AstralOrangeDye>().
                AddIngredient<AstralOre>(5).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
