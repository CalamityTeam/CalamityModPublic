using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class ProfanedMoonlightDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(mod.GetEffect("Effects/Dyes/ProfanedMoonlightDye")), "DyePass");
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Moonlight Dye");
        }

        public override void SafeSetDefaults()
        {
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.value = Item.sellPrice(0, 10, 0, 0);
        }
    }
}