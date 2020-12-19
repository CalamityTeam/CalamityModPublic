using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class TacticalPlagueEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactical Plague Engine");
            Tooltip.SetDefault("Summons a plague jet to pummel your enemies into submission\n" +
                               "Requires bullets to shoot\n" +
                               "Sometimes shoots a missile instead of a bullet");
        }

        public override void SetDefaults()
        {
            item.damage = 34;
            item.mana = 10;
            item.width = 28;
            item.height = 20;
            item.useTime = item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item14;
            item.autoReuse = true;
            item.summon = true;
            item.shoot = ModContent.ProjectileType<TacticalPlagueJet>();
            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool ConsumeAmmo(Player player) => false;

        public override bool CanUseItem(Player player) => player.HasAmmo(item, false);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			// Undo the vanilla addition of ammo damage
			damage -= GetVanillaAmmoDamage(player);

			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, item.shoot, damage, knockBack, player.whoAmI, 0f, 1f);
            return false;
        }

		private int GetVanillaAmmoDamage(Player player)
		{
			Item ammo = null;
			int ammoBonus = 0;
			bool ammoSlots = false;
			for (int index = Main.maxInventory - 4; index < Main.maxInventory; ++index)
			{
				if (player.inventory[index].ammo == item.useAmmo && player.inventory[index].stack > 0)
				{
					ammo = player.inventory[index];
					ammoSlots = true;
					break;
				}
			}
			if (!ammoSlots)
			{
				for (int index = 0; index < Main.maxInventory - 4; ++index)
				{
					if (player.inventory[index].ammo == item.useAmmo && player.inventory[index].stack > 0)
					{
						ammo = player.inventory[index];
						break;
					}
				}
			}

			if (ammo != null)
			{
				if (ammo.damage > 0)
				{
					ammoBonus = (int)(ammo.damage * player.MinionDamage());
				}
			}
			return ammoBonus;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlackHawkRemote>());
            recipe.AddIngredient(ModContent.ItemType<InfectedRemote>());
            recipe.AddIngredient(ModContent.ItemType<FuelCellBundle>());
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
