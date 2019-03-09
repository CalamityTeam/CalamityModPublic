using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.TheDevourerofGods
{
	public class DevourerofGodsBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = 9;
			item.expert = true;
			bossBagNPC = mod.NPCType("DevourerofGodsHeadS");
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(18f, 17f);
			spriteBatch.Draw(mod.GetTexture("Items/TheDevourerofGods/DevourerofGodsBagGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (CalamityWorld.revenge)
			{
				if (player.GetModPlayer<CalamityPlayer>(mod).fabsolVodka)
				{
					player.QuickSpawnItem(mod.ItemType("Fabsol"));
				}
				if (CalamityWorld.death && player.difficulty == 2)
				{
					player.QuickSpawnItem(mod.ItemType("CosmicPlushie"));
				}
				if (Main.rand.Next(100) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("CosmicDischarge"));
				}
				else if (CalamityWorld.defiled)
				{
					if (Main.rand.Next(20) == 0)
					{
						player.QuickSpawnItem(mod.ItemType("CosmicDischarge"));
					}
				}
				if (Main.rand.Next(20) == 0)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							player.QuickSpawnItem(mod.ItemType("StressPills"));
							break;
						case 1:
							player.QuickSpawnItem(mod.ItemType("Laudanum"));
							break;
						case 2:
							player.QuickSpawnItem(mod.ItemType("HeartofDarkness"));
							break;
					}
				}
			}
			player.TryGettingDevArmor();
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DeathhailStaff"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DevourerofGodsMask"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Eradicator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Excelsus"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TheObliterator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("EradicatorMelee"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Deathwind"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("StaffoftheMechworm"));
			}
			player.QuickSpawnItem(mod.ItemType("CosmiliteBar"), Main.rand.Next(30, 40));
			player.QuickSpawnItem(mod.ItemType("NebulousCore"));
		}
	}
}