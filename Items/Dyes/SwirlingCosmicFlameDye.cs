using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class SwirlingCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CosmicFlameShader").Value), "DyePass").
            UseColor(new Color(52, 212, 229)).UseSecondaryColor(new Color(255, 115, 221)).UseImage("Images/Misc/Noise").UseSaturation(1f);
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Swirling Cosmic Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.sellPrice(0, 7, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<BlueCosmicFlameDye>().
                AddIngredient<PinkCosmicFlameDye>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
