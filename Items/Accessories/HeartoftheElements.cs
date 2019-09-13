using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class HeartoftheElements : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heart of the Elements");
			Tooltip.SetDefault("The heart of the world\n" +
            	"Increases max life by 20, life regen by 1, and all damage by 8%\n" +
            	"Increases movement speed by 10% and jump speed by 100%\n" +
            	"Increases damage reduction by 5%\n" +
            	"Increases max mana by 50 and reduces mana usage by 5%\n" +
            	"You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt\n" +
            	"Summons all elementals to protect you\n" +
				"Toggling the visibility of this accessory also toggles the elementals on and off\n" +
				"Stat increases are slightly higher if the elementals are turned off");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 8));
		}

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 9;
			item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.brimstoneWaifu || modPlayer.sandWaifu || modPlayer.sandBoobWaifu || modPlayer.cloudWaifu || modPlayer.sirenWaifu)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, ((float)Main.DiscoR / 255f), ((float)Main.DiscoG / 255f), ((float)Main.DiscoB / 255f));
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.allWaifus = !hideVisual;
            modPlayer.elementalHeart = true;
            if (!hideVisual)
            {
                player.lifeRegen += 1;
                player.statLifeMax2 += 20;
                player.moveSpeed += 0.1f;
                player.jumpSpeedBoost += 1.0f;
                player.endurance += 0.05f;
                player.statManaMax2 += 50;
                player.manaCost *= 0.95f;
				player.allDamage += 0.08f;
				int damage = NPC.downedMoonlord ? 150 : 90;
                float damageMult = CalamityWorld.downedDoG ? 2f : 1f;
                if (player.ownedProjectileCounts[mod.ProjectileType("BigBustyRose")] > 1 || player.ownedProjectileCounts[mod.ProjectileType("SirenLure")] > 1 ||
                    player.ownedProjectileCounts[mod.ProjectileType("DrewsSandyWaifu")] > 1 || player.ownedProjectileCounts[mod.ProjectileType("SandyWaifu")] > 1 ||
                    player.ownedProjectileCounts[mod.ProjectileType("CloudyWaifu")] > 1)
                {
                    player.ClearBuff(mod.BuffType("HotE"));
                }
                if (player.FindBuffIndex(mod.BuffType("HotE")) == -1)
                {
                    player.AddBuff(mod.BuffType("HotE"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("BigBustyRose")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("BigBustyRose"), (int)((float)damage * damageMult * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("SirenLure")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SirenLure"), (int)((float)damage * damageMult * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("DrewsSandyWaifu")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("DrewsSandyWaifu"), (int)((float)damage * damageMult * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("SandyWaifu")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SandyWaifu"), (int)((float)damage * damageMult * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("CloudyWaifu")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("CloudyWaifu"), (int)((float)damage * damageMult * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
            }
            else
            {
                player.lifeRegen += 2;
                player.statLifeMax2 += 25;
                player.moveSpeed += 0.12f;
                player.jumpSpeedBoost += 1.1f;
                player.endurance += 0.06f;
                player.statManaMax2 += 60;
                player.manaCost *= 0.93f;
				player.allDamage += 0.1f;
				if (player.ownedProjectileCounts[mod.ProjectileType("BigBustyRose")] > 0 || player.ownedProjectileCounts[mod.ProjectileType("SirenLure")] > 0 ||
                    player.ownedProjectileCounts[mod.ProjectileType("DrewsSandyWaifu")] > 0 || player.ownedProjectileCounts[mod.ProjectileType("SandyWaifu")] > 0 ||
                    player.ownedProjectileCounts[mod.ProjectileType("CloudyWaifu")] > 0)
                {
                    player.ClearBuff(mod.BuffType("HotE"));
                }
            }
			if (player.velocity.Y == 0f && player.grappling[0] == -1)
			{
				int num4 = (int)player.Center.X / 16;
				int num5 = (int)(player.position.Y + (float)player.height - 1f) / 16;
				if (Main.tile[num4, num5] == null)
				{
					Main.tile[num4, num5] = new Tile();
				}
				if (!Main.tile[num4, num5].active() && Main.tile[num4, num5].liquid == 0 && Main.tile[num4, num5 + 1] != null && WorldGen.SolidTile(num4, num5 + 1))
				{
					Main.tile[num4, num5].frameY = 0;
					Main.tile[num4, num5].slope(0);
					Main.tile[num4, num5].halfBrick(false);
					if (Main.tile[num4, num5 + 1].type == 0)
					{
						if (Main.rand.Next(1000) == 0)
						{
							Main.tile[num4, num5].active(true);
							Main.tile[num4, num5].type = 227;
							Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
							while (Main.tile[num4, num5].frameX == 144)
							{
								Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
							}
						}
						if (Main.netMode == 1)
						{
							NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
						}
					}
					if (Main.tile[num4, num5 + 1].type == 2)
					{
						if (Main.rand.Next(2) == 0)
						{
							Main.tile[num4, num5].active(true);
							Main.tile[num4, num5].type = 3;
							Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
							while (Main.tile[num4, num5].frameX == 144)
							{
								Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
							}
						}
						else
						{
							Main.tile[num4, num5].active(true);
							Main.tile[num4, num5].type = 73;
							Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
							while (Main.tile[num4, num5].frameX == 144)
							{
								Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
							}
						}
						if (Main.netMode == 1)
						{
							NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
						}
					}
					else if (Main.tile[num4, num5 + 1].type == 109)
					{
						if (Main.rand.Next(2) == 0)
						{
							Main.tile[num4, num5].active(true);
							Main.tile[num4, num5].type = 110;
							Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
							while (Main.tile[num4, num5].frameX == 90)
							{
								Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
							}
						}
						else
						{
							Main.tile[num4, num5].active(true);
							Main.tile[num4, num5].type = 113;
							Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
							while (Main.tile[num4, num5].frameX == 90)
							{
								Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
							}
						}
						if (Main.netMode == 1)
						{
							NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
						}
					}
					else if (Main.tile[num4, num5 + 1].type == 60)
					{
						Main.tile[num4, num5].active(true);
						Main.tile[num4, num5].type = 74;
						Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(9, 17));
						if (Main.netMode == 1)
						{
							NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
						}
					}
				}
			}
		}

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "WifeinaBottle");
			recipe.AddIngredient(null, "WifeinaBottlewithBoobs");
			recipe.AddIngredient(null, "LureofEnthrallment");
			recipe.AddIngredient(null, "EyeoftheStorm");
			recipe.AddIngredient(null, "RoseStone");
			recipe.AddIngredient(null, "AeroStone");
			recipe.AddIngredient(null, "CryoStone");
			recipe.AddIngredient(null, "ChaosStone");
			recipe.AddIngredient(null, "BloomStone");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
    }
}
