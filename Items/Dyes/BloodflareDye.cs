using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class BloodflareDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/BloodflareDyeShader").Value), "DyePass").
            UseColor(new Color(122, 10, 60)).UseSecondaryColor(new Color(219, 102, 106)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.value = Item.sellPrice(0, 4, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<Bloodstone>(3).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
