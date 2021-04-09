using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class AuricBar : ModItem
    {
		public int frameCounter = 0;
		public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Bar");
            Tooltip.SetDefault("It radiates godly energy");
        }

        public override void SetDefaults()
        {
			item.createTile = ModContent.TileType<AuricTeslaBar>();
            item.width = 50;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 60);
            item.Calamity().customRarity = CalamityRarity.Violet;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
        }

		public override void UseStyle(Player player)
		{
			player.itemLocation += new Vector2(-10f * player.direction, 10f * player.gravDir).RotatedBy(player.itemRotation);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/AuricBar_Animated");
			spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 16), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/AuricBar_Animated");
			spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 16), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/AuricBarGlow");
			spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 16, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.5f * brightness, 0.7f * brightness, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>());
            recipe.AddIngredient(ModContent.ItemType<AuricOre>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 2);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
