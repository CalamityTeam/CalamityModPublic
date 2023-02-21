using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Items.Dyes
{
    public class PinkCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CosmicFlameShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(255, 115, 221)).UseSecondaryColor(new Color(255, 115, 221)).UseImage("Images/Misc/noise").UseSaturation(0f);
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Pink Cosmic Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<CosmiliteBar>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
