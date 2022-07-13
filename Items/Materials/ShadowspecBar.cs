using CalamityMod.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ShadowspecBar : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Shadowspec Bar");
            Tooltip.SetDefault("A vile, black metal, so horrendous and powerful as to defy explanation");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<ShadowspecBarTile>();
            Item.width = 38;
            Item.height = 40;
            Item.maxStack = 999;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.value = Item.sellPrice(platinum: 1, gold: 92);
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/ShadowspecBar_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/ShadowspecBar_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AuricBar>()).AddIngredient(ModContent.ItemType<ExoPrism>()).AddIngredient(ModContent.ItemType<AshesofAnnihilation>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
