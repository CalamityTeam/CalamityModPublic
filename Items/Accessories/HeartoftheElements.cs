using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class HeartoftheElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of the Elements");
            Tooltip.SetDefault("The heart of the world\n" +
                "Increases max life by 20, life regen by 1, and all damage by 5%\n" +
                "Increases movement speed by 10% and jump speed by 20%\n" +
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
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = 10;
            item.defense = 9;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.brimstoneWaifu || modPlayer.sandWaifu || modPlayer.sandBoobWaifu || modPlayer.cloudWaifu || modPlayer.sirenWaifu)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			if (player != null && !player.dead)
			{
				Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, Main.DiscoR / 255f, Main.DiscoG / 255f, Main.DiscoB / 255f);
			}
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.allWaifus = !hideVisual;
            modPlayer.elementalHeart = true;

			player.lifeRegen += hideVisual ? 2 : 1;
			player.statLifeMax2 += hideVisual ? 25 : 20;
			player.moveSpeed += hideVisual ? 0.12f : 0.1f;
			player.jumpSpeedBoost += hideVisual ? (player.autoJump ? 0.3f : 1.1f) : (player.autoJump ? 0.25f : 1f);
			player.endurance += hideVisual ? 0.06f : 0.05f;
			player.statManaMax2 += hideVisual ? 60 : 50;
			player.manaCost *= hideVisual ? 0.93f : 0.95f;
			player.allDamage += hideVisual ? 0.07f : 0.05f;

			int brimmy = ProjectileType<BrimstoneElementalMinion>();
			int siren = ProjectileType<WaterElementalMinion>();
			int healer = ProjectileType<SandElementalHealer>();
			int sandy = ProjectileType<SandElementalMinion>();
			int cloudy = ProjectileType<CloudElementalMinion>();

            if (!hideVisual)
            {
				Vector2 velocity = new Vector2(0f, -1f);
				int damage = NPC.downedMoonlord ? 150 : 90;
				float damageMult = CalamityWorld.downedDoG ? 2f : 1f;
				int elementalDmg = (int)(damage * damageMult * player.MinionDamage());
				float kBack = 2f + player.minionKB;

				if (player.ownedProjectileCounts[brimmy] > 1 || player.ownedProjectileCounts[siren] > 1 ||
					player.ownedProjectileCounts[healer] > 1 || player.ownedProjectileCounts[sandy] > 1 ||
					player.ownedProjectileCounts[cloudy] > 1)
				{
					player.ClearBuff(BuffType<HotE>());
				}
				if (player != null && player.whoAmI == Main.myPlayer && !player.dead)
				{
					if (player.FindBuffIndex(BuffType<HotE>()) == -1)
					{
						player.AddBuff(BuffType<HotE>(), 3600, true);
					}
					if (player.ownedProjectileCounts[brimmy] < 1)
					{
						Projectile.NewProjectile(player.Center, velocity, brimmy, elementalDmg, kBack, player.whoAmI);
					}
					if (player.ownedProjectileCounts[siren] < 1)
					{
						Projectile.NewProjectile(player.Center, velocity, siren, elementalDmg, kBack, player.whoAmI);
					}
					if (player.ownedProjectileCounts[healer] < 1)
					{
						Projectile.NewProjectile(player.Center, velocity, healer, elementalDmg, kBack, player.whoAmI);
					}
					if (player.ownedProjectileCounts[sandy] < 1)
					{
						Projectile.NewProjectile(player.Center, velocity, sandy, elementalDmg, kBack, player.whoAmI);
					}
					if (player.ownedProjectileCounts[cloudy] < 1)
					{
						Projectile.NewProjectile(player.Center, velocity, cloudy, elementalDmg, kBack, player.whoAmI);
					}
				}
            }
            else
            {
                if (player.ownedProjectileCounts[brimmy] > 0 || player.ownedProjectileCounts[siren] > 0 ||
                    player.ownedProjectileCounts[healer] > 0 || player.ownedProjectileCounts[sandy] > 0 ||
                    player.ownedProjectileCounts[cloudy] > 0)
                {
                    player.ClearBuff(BuffType<HotE>());
                }
            }

			//Flower Boots code
            if (player != null && !player.dead && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                int x = (int)player.Center.X / 16;
                int y = (int)(player.position.Y + player.height - 1f) / 16;
				Tile tile = Main.tile[x, y];
                if (tile is null)
                {
                    tile = new Tile();
                }
                if (!tile.active() && tile.liquid == 0 && Main.tile[x, y + 1] != null && WorldGen.SolidTile(x, y + 1))
                {
                    tile.frameY = 0;
                    tile.slope(0);
                    tile.halfBrick(false);
					//On dirt blocks, there's a small chance to grow a dye plant
                    if (Main.tile[x, y + 1].type == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            tile.active(true);
                            tile.type = TileID.DyePlants;
                            tile.frameX = (short)(34 * Main.rand.Next(1, 13));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(34 * Main.rand.Next(1, 13));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On grass, grow flowers
                    if (Main.tile[x, y + 1].type == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.active(true);
                            tile.type = TileID.Plants;
                            tile.frameX = (short)(18 * Main.rand.Next(6, 11));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(6, 11));
                            }
                        }
                        else
                        {
                            tile.active(true);
                            tile.type = TileID.Plants2;
                            tile.frameX = (short)(18 * Main.rand.Next(6, 21));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(6, 21));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On hallowed grass, grow hallowed flowers
                    else if (Main.tile[x, y + 1].type == TileID.HallowedGrass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.active(true);
                            tile.type = TileID.HallowedPlants;
                            tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                            while (tile.frameX == 90)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                            }
                        }
                        else
                        {
                            tile.active(true);
                            tile.type = TileID.HallowedPlants2;
                            tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                            while (tile.frameX == 90)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On jungle grass, grow jungle flowers
                    else if (Main.tile[x, y + 1].type == TileID.JungleGrass)
                    {
                        tile.active(true);
                        tile.type = TileID.JunglePlants2;
                        tile.frameX = (short)(18 * Main.rand.Next(9, 17));
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<WifeinaBottle>());
            recipe.AddIngredient(ItemType<WifeinaBottlewithBoobs>());
            recipe.AddIngredient(ItemType<LureofEnthrallment>());
            recipe.AddIngredient(ItemType<EyeoftheStorm>());
            recipe.AddIngredient(ItemType<RoseStone>());
            recipe.AddIngredient(ItemType<AeroStone>());
            recipe.AddIngredient(ItemType<CryoStone>());
            recipe.AddIngredient(ItemType<ChaosStone>());
            recipe.AddIngredient(ItemType<BloomStone>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
