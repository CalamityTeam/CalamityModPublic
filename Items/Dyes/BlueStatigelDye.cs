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
    public class BlueStatigelDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/SlimeGodDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(80, 170, 206)).UseSecondaryColor(new Color(81, 87, 119)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Statigel Dye");
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
