using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CalamitousEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitous Essence");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 34;
            item.maxStack = 999;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(gold: 24);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, Main.itemTexture[item.type]);
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.1f * brightness, 0.1f * brightness, 0.1f * brightness);
        }
    }
}
