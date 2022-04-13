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
    public class AuricDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/AuricDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(170, 96, 60)).UseSecondaryColor(new Color(226, 196, 106)).SetShaderTextureArmor(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SharpNoise", AssetRequestMode.ImmediateLoad));
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<AuricOre>(5).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
