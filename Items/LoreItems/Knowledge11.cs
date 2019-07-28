using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge11 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Brain of Cthulhu");
			Tooltip.SetDefault("An eye and now a brain.\n" +
                "Most likely another abomination spawned from this inchoate mass of flesh.\n" +
				"Allows you to teleport similar to the Rod of Discord while in the crimson. Place in your hotbar to use it.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 2;
			item.autoReuse = false;
			item.useStyle = 1;
			item.useAnimation = 20;
			item.useTime = 20;
			item.UseSound = SoundID.Item8;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ZoneCrimson;
		}

		public override bool UseItem(Player player)
		{
			if (Main.myPlayer == player.whoAmI && player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				Vector2 vector31;
				vector31.X = (float)Main.mouseX + Main.screenPosition.X;
				if (player.gravDir == 1f)
				{
					vector31.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)player.height;
				}
				else
				{
					vector31.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
				}
				vector31.X -= (float)(player.width / 2);
				if (vector31.X > 50f && vector31.X < (float)(Main.maxTilesX * 16 - 50) && vector31.Y > 50f && vector31.Y < (float)(Main.maxTilesY * 16 - 50))
				{
					int num275 = (int)(vector31.X / 16f);
					int num276 = (int)(vector31.Y / 16f);
					if ((Main.tile[num275, num276].wall != 87 || (double)num276 <= Main.worldSurface || NPC.downedPlantBoss) && !Collision.SolidCollision(vector31, player.width, player.height))
					{
						player.Teleport(vector31, 1, 0);
						NetMessage.SendData(65, -1, -1, null, 0, (float)player.whoAmI, vector31.X, vector31.Y, 1, 0, 0);
						if (player.chaosState)
						{
							player.statLife -= player.statLifeMax2 / 7;
							PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
							if (Main.rand.Next(2) == 0)
							{
								damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
							}
							if (player.statLife <= 0)
							{
								player.KillMe(damageSource, 1.0, 0, false);
							}
							player.lifeRegenCount = 0;
							player.lifeRegenTime = 0;
						}
						player.AddBuff(BuffID.ChaosState, 480, true);
					}
				}
			}
			return true;
		}
	}
}
