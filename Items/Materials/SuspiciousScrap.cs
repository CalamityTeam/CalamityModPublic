using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Materials
{
    public class SuspiciousScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Scrap");
            Tooltip.SetDefault("Looks like it may be part of a greater whole...");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 0, 20);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = Main.itemTexture[item.type];
            Rectangle variant = new Rectangle(CalamityWorld.OreTypes[0] == TileID.Tin ? 0 : 32, CalamityWorld.OreTypes[1] == TileID.Lead ? 0 : 32, 30, 30);
            spriteBatch.Draw(tex, position, variant, drawColor, 0f, origin, scale * 2f, 0f, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Main.itemTexture[item.type];
            Rectangle variant = new Rectangle(CalamityWorld.OreTypes[0] == TileID.Tin ? 0 : 32, CalamityWorld.OreTypes[1] == TileID.Lead ? 0 : 32, 30, 30);
            Vector2 positionDisplace = new Vector2( 16 , 16 ) * scale;

            spriteBatch.Draw(tex, item.position + positionDisplace - Main.screenPosition, variant, lightColor, rotation, variant.Size() / 2f, scale, 0f, 0f);
            return false;
        }
    }
}
