using CalamityMod.Items.Materials;
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
    public class BloodflareDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/BloodflareDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(122, 10, 60)).UseSecondaryColor(new Color(219, 102, 106)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Bloodflare Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(0, 4, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<Bloodstone>(5).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
