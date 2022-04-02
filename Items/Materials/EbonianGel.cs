using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class EbonianGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blighted Gel");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 18;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 6);
            item.rare = ItemRarityID.Blue;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.notAmmo)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/EbonianGelRed");
                spriteBatch.Draw(texture, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            }
            return !item.notAmmo;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (item.notAmmo)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/EbonianGelRed");
                spriteBatch.Draw(texture, item.position - Main.screenPosition, new Rectangle(0, 0, item.width, item.height), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            return !item.notAmmo;
        }
    }
}
