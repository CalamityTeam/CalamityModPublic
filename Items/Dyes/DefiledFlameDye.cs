using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Terraria.ID;
using ReLogic.Content;

namespace CalamityMod.Items.Dyes
{
    public class DefiledFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/DefiledFlameDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(106, 190, 48)).UseSecondaryColor(new Color(204, 248, 48)).UseImage("Images/Misc/Perlin");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Defiled Flame Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
        }
    }
}
