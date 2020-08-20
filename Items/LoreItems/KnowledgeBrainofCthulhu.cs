using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBrainofCthulhu : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Brain of Cthulhu");
            Tooltip.SetDefault("An eye and now a brain.\n" +
                "Most likely another abomination spawned from this inchoate mass of flesh.\n" +
                "Allows you to teleport similar to the Rod of Discord while in the crimson. Place in your hotbar to use it.\n" +
				"Teleportation is disabled while Chaos State is active.\n" +
				"However, you become confused for a few seconds after you use it due to your overwhelming brain power.\n" +
				"Teleportation only occurs if the item is favorited.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 2;
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.UseSound = SoundID.Item8;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneCrimson && item.favorited;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.itemAnimation > 0 && player.itemTime == 0)
            {
				if (!player.chaosState)
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
							NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, vector31.X, vector31.Y, 1, 0, 0);
							player.AddBuff(BuffID.ChaosState, 480, true);
							player.AddBuff(BuffID.Confused, 150, true);
						}
					}
				}
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ItemID.BrainofCthulhuTrophy);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
