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
    public class EndothermicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/EndothermicDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(123, 205, 237)).UseSecondaryColor(new Color(85, 85, 171));
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Endothermic Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<EndothermicEnergy>(5).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
