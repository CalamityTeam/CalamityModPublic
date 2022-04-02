using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CosmiliteBar : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmilite Bar");
            Tooltip.SetDefault("A chunk of durable and highly resilient cosmic steel");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<CosmiliteBarTile>();
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 20);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/CosmiliteBar_Animated");
            spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/CosmiliteBar_Animated");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.5f * brightness, 0f, 0.5f * brightness);
        }
    }
}
