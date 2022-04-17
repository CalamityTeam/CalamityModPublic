using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using Terraria.GameContent;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class SuspiciousScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
            DisplayName.SetDefault("Suspicious Scrap");
            Tooltip.SetDefault("Looks like it may be part of a greater whole...");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 0, 20);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;
            Rectangle variant = new Rectangle(CalamityWorld.OreTypes[0] == TileID.Tin ? 0 : 32, CalamityWorld.OreTypes[1] == TileID.Lead ? 0 : 32, 30, 30);
            spriteBatch.Draw(tex, position, variant, drawColor, 0f, origin, scale * 2f, 0f, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;
            Rectangle variant = new Rectangle(CalamityWorld.OreTypes[0] == TileID.Tin ? 0 : 32, CalamityWorld.OreTypes[1] == TileID.Lead ? 0 : 32, 30, 30);
            Vector2 positionDisplace = new Vector2( 16 , 16 ) * scale;

            spriteBatch.Draw(tex, Item.position + positionDisplace - Main.screenPosition, variant, lightColor, rotation, variant.Size() / 2f, scale, 0f, 0f);
            return false;
        }
    }
}
