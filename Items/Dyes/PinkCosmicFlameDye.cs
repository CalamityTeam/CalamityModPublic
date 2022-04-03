using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Dyes
{
    public class PinkCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CosmicFlameShader").Value), "DyePass").
            UseColor(new Color(255, 115, 221)).UseSecondaryColor(new Color(255, 115, 221)).UseImage("Images/Misc/Noise").UseSaturation(0f);
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Pink Cosmic Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemID.BottledWater, 2).AddIngredient(ModContent.ItemType<CosmiliteBar>()).AddTile(TileID.DyeVat).Register();
        }
    }
}
