using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ChargedDartRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charged Dart Blaster");
            Tooltip.SetDefault("Fires a shotgun spread of darts and a splitting energy blast\n" +
            "Right click to fire a more powerful exploding energy blast that bounces");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<ChargedBlast>();
            Item.useAmmo = AmmoID.Dart;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ChargedBlast3>(), (int)((double)damage * 2), knockback, player.whoAmI, 0f, 0f);
                return false;
            }
            else
            {
                int num6 = Main.rand.Next(2, 5);
                for (int index = 0; index < num6; ++index)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-40, 41) * 0.05f;
                    int projectile = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage / 2, knockback, player.whoAmI, 0f, 0f);
                }
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0f);
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DartRifle).
                AddIngredient(ItemID.MartianConduitPlating, 25).
                AddIngredient<CoreofEleum>(3).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.DartPistol).
                AddIngredient(ItemID.MartianConduitPlating, 25).
                AddIngredient<CoreofEleum>(3).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
