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
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.width = 68;
            item.height = 38;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 1.2f;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.Item11;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FlurrystormCannonShooting>();
            item.useAmmo = AmmoID.Snowball;
            item.shootSpeed = 18f;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FlurrystormCannonShooting>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SnowballCannon);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 10);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 25);
            recipe.AddIngredient(ItemID.WaterBucket, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
