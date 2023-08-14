using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    // TODO -- WHY IS THIS IN PLACEABLES. IT IS A CRAFTING MATERIAL FIRST AND FOREMOST.
    public class AstralBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 99; // Luminite
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.AstralBar>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/AstralBar_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 12), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/AstralBar_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 12), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(3).
                AddIngredient<AstralOre>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
