using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
	public class TheAmalgam : ModItem
	{
		public const int FireProjectiles = 2;
		public const float FireAngleSpread = 120;
		public int counter = 0;
		public const int ProjectileDamage = 2000;
		public const int FungalClumpDamage = 1000;
		public const int AuraDamage = 300;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Amalgam");
			Tooltip.SetDefault("Extends the duration of potion buffs by 100% and potion buffs remain active even after you die\n" +
							"15% increased damage and movement speed\n" +
							"Shade and brimstone fire rain down when you are hit\n" +
							"Nearby enemies receive a variety of debuffs when you are hit\n" +
							"Brimstone fireballs drop from the sky occasionally\n" +
							"Summons a fungal clump to fight for you that leaves behind poisonous seawater\n" +
							"Temporary immunity to lava and greatly reduces lava burn damage\n" +
							"You have a damaging aura that envenoms nearby enemies");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(9, 6));
		}

		public override void SetDefaults()
		{
			item.width = 34;
			item.height = 34;
			item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.expert = true;
			item.rare = 9;
			item.accessory = true;
			item.defense = 10;
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip7")
					{
						line2.text = "You have a damaging aura that envenoms nearby enemies\n" +
						"Provides heat protection in Death Mode";
					}
				}
			}
		}

		public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().fungalClump;

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			//Counter used for aura and Gehenna effects
			counter++;

			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.amalgam = true;
			modPlayer.fungalClump = true;
			player.ignoreWater = true;
			player.lavaRose = true;
			player.allDamage += 0.15f;
			player.moveSpeed += 0.15f;
			player.lavaMax += 240;

			//emit light
			Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 0.5f, 1.25f);

			//Summon the Fungal Clump, ai[0] is 1f to indicate this is from the Amalgam, not the Fungal Clump
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(BuffType<FungalClumpBuff>()) == -1)
				{
					player.AddBuff(BuffType<FungalClumpBuff>(), 3600, true);
				}
				if (player.ownedProjectileCounts[ProjectileType<FungalClumpMinion>()] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ProjectileType<FungalClumpMinion>(), (int)(FungalClumpDamage * player.MinionDamage()), 1f, player.whoAmI, 1f, 0f);
				}
			}

			if (player.immune)
			{
				if (player.miscCounter % 8 == 0)
				{
					if (player.whoAmI == Main.myPlayer)
					{
						int type = Main.rand.NextBool(2) ? ProjectileType<AuraRain>() : ProjectileType<StandingFire>();
						Projectile rain = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, type, (int)(ProjectileDamage * player.AverageDamage()), 5f, player.whoAmI, 6, 1);
						if (type == ProjectileType<AuraRain>())
						{
							rain.tileCollide = false;
							rain.penetrate = 1;
						}
					}
				}
			}

			int buffType = BuffID.Venom;
			float auraRange = 300f;
			int auraDmg = (int)(AuraDamage * player.AverageDamage());
			if (player.whoAmI == Main.myPlayer)
			{
				for (int l = 0; l < Main.maxNPCs; l++)
				{
					NPC npc = Main.npc[l];
					if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= auraRange)
					{
						if (npc.FindBuffIndex(buffType) == -1 && !npc.buffImmune[buffType])
						{
							npc.AddBuff(buffType, 300, false);
						}
						if (counter % 30 == 0)
						{
							if (player.whoAmI == Main.myPlayer)
							{
								Projectile p = Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ProjectileType<DirectStrike>(), auraDmg, 0f, player.whoAmI, l);
							}
						}
					}
				}
			}

			if (counter % 480 == 0)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					int speed = 25;
					float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
					float spawnY = -1000 + player.Center.Y;
					Vector2 baseSpawn = new Vector2(spawnX, spawnY);
					Vector2 baseVelocity = player.Center - baseSpawn;
					baseVelocity.Normalize();
					baseVelocity *= speed;
					for (int i = 0; i < FireProjectiles; i++)
					{
						Vector2 spawn = baseSpawn;
						spawn.X += i * 30 - (FireProjectiles * 15);
						Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / FireProjectiles)));
						velocity.X += 3 * Main.rand.NextFloat() - 1.5f;
						Projectile.NewProjectile(spawn, velocity, ProjectileType<BrimstoneHellfireballFriendly2>(), (int)(ProjectileDamage * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
					}
				}
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<AmalgamatedBrain>());
			recipe.AddIngredient(ItemType<VoidofExtinction>());
			recipe.AddIngredient(ItemType<FungalClump>());
			recipe.AddIngredient(ItemType<LeviathanAmbergris>());
			recipe.AddIngredient(ItemType<CosmiliteBar>(), 5);
			recipe.AddIngredient(ItemType<AscendantSpiritEssence>());
			recipe.AddTile(TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
