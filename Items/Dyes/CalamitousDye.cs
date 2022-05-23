using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class CalamitousDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CalamitousDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(227, 79, 79)).UseSecondaryColor(new Color(145, 27, 135)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Calamitous Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = Item.sellPrice(0, 10, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<CalamitousEssence>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
