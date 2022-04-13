using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Dyes
{
    public class CryonicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CryonicDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(138, 225, 255)).UseSecondaryColor(new Color(90, 90, 204)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<CryonicOre>(4).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
