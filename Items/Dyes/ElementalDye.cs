using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class ElementalDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/ElementalDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Elemental Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 2, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.SolarDye).
                AddIngredient(ItemID.VortexDye).
                AddIngredient(ItemID.StardustDye).
                AddIngredient(ItemID.NebulaDye).
                AddIngredient<GalacticaSingularity>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
