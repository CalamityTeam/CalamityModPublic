using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class ProfanedMoonlightDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/ProfanedMoonlightDye", AssetRequestMode.ImmediateLoad).Value), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Profaned Moonlight Dye");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.value = Item.sellPrice(0, 10, 0, 0);
        }
    }
}
