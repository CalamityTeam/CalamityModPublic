using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlurrystormCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurrystorm Cannon");
            Tooltip.SetDefault("Fires a chain of snowballs that become faster over time\n" +
            "Has a chance to also fire an ice chunk that shatters into shards\n" +
            "50% chance to not consume snowballs");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 68;
            Item.height = 38;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.2f;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlurrystormCannonShooting>();
            Item.useAmmo = AmmoID.Snowball;
            Item.shootSpeed = 18f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<FlurrystormCannonShooting>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SnowballCannon).
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.Bone, 50).
                AddIngredient<VictoryShard>(25).
                AddIngredient(ItemID.WaterBucket, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
