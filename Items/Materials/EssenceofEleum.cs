using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class EssenceofEleum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Essence of Eleum");
            Tooltip.SetDefault("The essence of cold creatures");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 40);
            item.rare = ItemRarityID.LightRed;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, Main.itemTexture[item.type]);
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.15f * brightness, 0.05f * brightness, 0.5f * brightness);
        }
    }
}
