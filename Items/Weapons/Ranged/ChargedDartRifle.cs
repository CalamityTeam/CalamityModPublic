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
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
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
            CreateRecipe(1).AddIngredient(ItemID.DartRifle).AddIngredient(ItemID.MartianConduitPlating, 25).AddIngredient(ModContent.ItemType<CoreofEleum>(), 3).AddIngredient(ItemID.FragmentVortex, 5).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.DartPistol).AddIngredient(ItemID.MartianConduitPlating, 25).AddIngredient(ModContent.ItemType<CoreofEleum>(), 3).AddIngredient(ItemID.FragmentVortex, 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
