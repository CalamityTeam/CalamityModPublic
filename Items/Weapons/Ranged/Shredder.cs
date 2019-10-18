using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shredder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shredder");
            Tooltip.SetDefault("The myth, the legend, the weapon that drops more frames than any other\n" +
                "Fires a barrage of energy bolts that split and bounce\n" +
                "Right click to fire a barrage of normal bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.ranged = true;
            item.width = 56;
            item.height = 24;
            item.useTime = 3;
            item.reuseDelay = 10;
            item.useAnimation = 24;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 12f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int bulletAmt = 4;
            if (player.altFunctionUse == 2)
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
                    int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
            else
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
                    int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ChargedBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GalacticaSingularity", 5);
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(null, "ChargedDartRifle");
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle);
            recipe.AddIngredient(ItemID.Shotgun);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
