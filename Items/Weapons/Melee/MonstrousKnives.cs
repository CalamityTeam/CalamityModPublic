using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class MonstrousKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monstrous Knives");
            Tooltip.SetDefault("Throws a spread of knives that can heal the user");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 4;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 21;
            item.useStyle = 1;
            item.useTime = 21;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.height = 20;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<MonstrousKnife>();
            item.shootSpeed = 15f;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int knifeAmt = 3;
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
            recipe.AddIngredient(ItemID.ThrowingKnife, 200);
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddIngredient(ItemID.LesserHealingPotion, 5); //I considered making a recipe group for any healing potion but Merchant sells these
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
