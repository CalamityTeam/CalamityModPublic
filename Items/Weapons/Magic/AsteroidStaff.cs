using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AsteroidStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asteroid Staff");
            Tooltip.SetDefault("Summons asteroids from the sky");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 180;
            item.magic = true;
            item.mana = 20;
            item.width = 50;
            item.height = 50;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6.75f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item88;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Asteroid>();
            item.shootSpeed = 20f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(12, 25);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteorStaff);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Projectile asteroid = CalamityUtils.ProjectileToMouse(player, 3, item.shootSpeed, 0.03f, 40f, type, damage, knockBack, player.whoAmI, true, 40f);
			asteroid.ai[1] = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
            return false;
        }
    }
}
