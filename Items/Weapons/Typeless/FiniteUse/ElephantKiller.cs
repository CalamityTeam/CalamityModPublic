using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class ElephantKiller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elephant Killer");
            Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Does more damage to organic enemies\n" +
                "Can be used thrice per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 2000;
            item.crit += 66;
            item.width = 46;
            item.height = 26;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<MagnumRound>();
            item.useAmmo = ModContent.ItemType<MagnumRounds>();
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 3;
            }
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 3;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return item.Calamity().timesUsed < 3;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 0;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
				player.inventory[player.selectedItem].Calamity().timesUsed++;
				for (int i = 0; i < Main.maxInventory; i++)
				{
					if (player.inventory[i].type == item.type)
					{
						player.inventory[i].Calamity().timesUsed++;
					}
				}
			}
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LightningHawk>());
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
