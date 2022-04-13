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
    public class AstralBlueDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/AstralBlueDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(109, 242, 197)).UseSecondaryColor(new Color(42, 147, 154));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Blue Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<AstralOre>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
