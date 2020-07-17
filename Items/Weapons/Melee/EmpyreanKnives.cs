using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EmpyreanKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Knives");
            Tooltip.SetDefault("Throws a flurry of bouncing knives that can heal the user");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 600;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.height = 20;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<EmpyreanKnife>();
            item.shootSpeed = 15f;
            item.Calamity().customRarity = CalamityRarity.Violet;
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
            if (Main.rand.NextBool(8))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(16))
            {
                knifeAmt++;
            }
			CalamityUtils.ProjectileToMouse(player, knifeAmt, item.shootSpeed, 0.05f, 25f, type, damage, knockBack, player.whoAmI, false);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.VampireKnives);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<LunarKunai>(), 999);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
