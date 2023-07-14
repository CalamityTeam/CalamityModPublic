using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Infinity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        internal int rotation = 0;
        internal bool limit = true;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 24;
            Item.useTime = 2;
            Item.useAnimation = 18;
            Item.reuseDelay = 6;
            Item.useLimitPerAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
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
                //If you right click, shoots an helix of normal bullets
                Vector2 num7 = velocity.RotatedBy(MathHelper.ToRadians(rotation));
                Vector2 num8 = velocity.RotatedBy(MathHelper.ToRadians(-rotation));
                int shot1 = Projectile.NewProjectile(source, position.X, position.Y, num7.X, num7.Y, type, damage, knockback, player.whoAmI, 0f, 0f);
                Main.projectile[shot1].timeLeft = 180;
                int shot2 = Projectile.NewProjectile(source, position.X, position.Y, num8.X, num8.Y, type, damage, knockback, player.whoAmI, 0f, 0f);
                Main.projectile[shot2].timeLeft = 180;
                //Code to constantly make the shooting go side to side to make the helix
                if (limit)
                {
                    rotation += 2;
                }
                else
                {
                    rotation -= 2;
                }
                if (rotation >= 15)
                {
                    limit = false;
                }
                else if (rotation <= -15)
                {
                    limit = true;
                }
                return false;
            }
            else
            {
                //If left click, do the same as above but spawn Charged Blasts instead
                Vector2 num7 = velocity.RotatedBy(MathHelper.ToRadians(rotation));
                Vector2 num8 = velocity.RotatedBy(MathHelper.ToRadians(-rotation));
                int shot1 = Projectile.NewProjectile(source, position.X, position.Y, num7.X, num7.Y, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0f);
                Main.projectile[shot1].timeLeft = 180;
                int shot2 = Projectile.NewProjectile(source, position.X, position.Y, num8.X, num8.Y, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0f);
                Main.projectile[shot2].timeLeft = 180;
                if (limit)
                {
                    rotation += 2;
                }
                else
                {
                    rotation -= 2;
                }
                if (rotation >= 15)
                {
                    limit = false;
                }
                else if (rotation <= -15)
                {
                    limit = true;
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Shredder>().
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
