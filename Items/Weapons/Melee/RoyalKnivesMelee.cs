using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class RoyalKnivesMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Illustrious Knives");
            Tooltip.SetDefault("Throws a flurry of homing knives that can heal the user");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 2000;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.height = 20;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<IllustriousKnife>();
            item.shootSpeed = 9f;
            item.Calamity().customRarity = CalamityRarity.Developer;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int knifeAmt = 4;
            if (Main.rand.NextBool(2))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(4))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(6))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(8))
            {
                knifeAmt++;
            }
			Projectile knife = CalamityUtils.ProjectileToMouse(player, knifeAmt, item.shootSpeed, 0.05f, 35f, type, damage, knockBack, player.whoAmI, false);
			knife.Calamity().forceMelee = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddIngredient(ModContent.ItemType<EmpyreanKnives>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
