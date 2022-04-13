using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Dyes
{
    public class PinkStatigelDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/SlimeGodDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(249, 129, 185)).UseSecondaryColor(new Color(131, 58, 103)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Pink Statigel Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 60, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<PurifiedGel>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
