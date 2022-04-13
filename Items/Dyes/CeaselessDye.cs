using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Dyes
{
    public class CeaselessDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/CeaselessDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Ceaseless Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.VoidDye).
                AddIngredient(ItemID.ShadowDye).
                AddIngredient<DarkPlasma>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
