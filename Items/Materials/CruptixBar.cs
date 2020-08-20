using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
	public class CruptixBar : ModItem
	{
		private int frameCounter = 0;
		private int frame = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scoria Bar");
			Tooltip.SetDefault("The smoke feels warm");
		}

		public override void SetDefaults()
		{
			item.createTile = ModContent.TileType<ChaoticBarPlaced>();
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.width = 40;
			item.height = 52;
			item.maxStack = 999;
			item.value = Item.sellPrice(gold: 1, silver: 20);
			item.rare = 8;
		}

		internal Rectangle GetCurrentFrame()
		{
			int frameAmt = 6;
			if (frameCounter >= 6)
			{
				frameCounter = -1;
				frame = frame == frameAmt - 1 ? 0 : frame + 1;
			}
			frameCounter++;
			return new Rectangle(0, item.height * frame, item.width, item.height);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/CruptixBar_Animated");
			spriteBatch.Draw(texture, position, GetCurrentFrame(), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/CruptixBar_Animated");
			spriteBatch.Draw(texture, item.position - Main.screenPosition, GetCurrentFrame(), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<ChaoticOre>(), 5);
			recipe.AddTile(TileID.AdamantiteForge);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
