using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ChargedDartRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charged Dart Blaster");
            Tooltip.SetDefault("Fires a shotgun spread of darts and a splitting energy blast\n" +
            "Right click to fire a more powerful exploding energy blast that bounces");
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.ranged = true;
            item.width = 74;
            item.height = 40;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.autoReuse = true;
            item.shootSpeed = 22f;
            item.shoot = ModContent.ProjectileType<ChargedBlast>();
            item.useAmmo = AmmoID.Dart;
            item.Calamity().canFirePointBlankShots = true;
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
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ChargedBlast3>(), (int)((double)damage * 2), knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
            else
            {
                int num6 = Main.rand.Next(2, 5);
                for (int index = 0; index < num6; ++index)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                    int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
                }
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ChargedBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DartRifle);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 25);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 3);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DartPistol);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 25);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 3);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
