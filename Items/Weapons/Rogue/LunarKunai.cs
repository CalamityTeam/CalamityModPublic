using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class LunarKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Kunai");
			Tooltip.SetDefault("Throws out a set of three kunai that ignore gravity and slightly home in on enemies\n"
							  +"After traveling enough distance, the kunai supercharge with lunar energy, homing in far more aggressively and exploding on impact\n"
							  +"Stealth strikes instantly throw five supercharged Kunai");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
			item.height = 38;
            item.damage = 100;
			item.maxStack = 999;
			item.consumable = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 0, 1, 20);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<LunarKunaiProj>();
            item.shootSpeed = 22f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().rogue = true;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int knifeAmt = player.Calamity().StealthStrikeAvailable() ? 5 : 3;
			Projectile knife = CalamityUtils.ProjectileToMouse(player, knifeAmt, item.shootSpeed, 0.05f, 35f, type, damage, knockBack, player.whoAmI, false);
			knife.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
			return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 333);
            recipe.AddRecipe();
        }
    }
}
