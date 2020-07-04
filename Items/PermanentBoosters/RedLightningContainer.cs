using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
	public class RedLightningContainer : ModItem
	{
		private int frameCounter = 0;
		private int frame = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Lightning Container");
			Tooltip.SetDefault("Permanently makes Rage Mode do 15% more damage\n" +
				"Revengeance drop");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 40;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item122;
			item.consumable = true;
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
		}

		public override bool CanUseItem(Player player) => !player.Calamity().rageBoostThree;

		internal Rectangle GetCurrentFrame(bool frameCounterUp = true)
		{
			int frameAmt = 6;
			if (frameCounter >= 5)
			{
				frameCounter = -1;
				frame = frame == frameAmt - 1 ? 0 : frame + 1;
			}
			if (frameCounterUp)
				frameCounter++;
			return new Rectangle(0, item.height * frame, item.width, item.height);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/RedLightningContainer_Animated");
			spriteBatch.Draw(texture, position, GetCurrentFrame(), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/RedLightningContainer_Animated");
			spriteBatch.Draw(texture, item.position - Main.screenPosition, GetCurrentFrame(), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/RedLightningContainerGlow");
			spriteBatch.Draw(texture, item.position - Main.screenPosition, GetCurrentFrame(false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				CalamityPlayer modPlayer = player.Calamity();
				modPlayer.rageBoostThree = true;
			}
			return true;
		}
	}
}
