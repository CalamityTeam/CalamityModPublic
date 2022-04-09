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
    public class AstralOrangeDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/AstralOrangeDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(255, 166, 94)).UseSecondaryColor(new Color(238, 93, 82));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Orange Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<AstralOre>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
