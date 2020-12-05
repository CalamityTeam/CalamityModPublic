using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Onyxia : ModItem
    {
        const int NotConsumeAmmo = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyxia");
            Tooltip.SetDefault(NotConsumeAmmo.ToString() + "% chance to not consume ammo\n" +
                "Fires a storm of bullets and onyx shards");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.ranged = true;
            item.width = 84;
            item.height = 34;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.BlackBolt;
            item.shootSpeed = 28f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 3);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals triple damage and double knockback
            int shardDamage = (int)(2.5 * damage);
            float shardKB = 2f * knockBack;
            float shardVelocityX = (speedX + (float)Main.rand.Next(-25, 26) * 0.05f) * 0.9f;
            float shardVelocityY = (speedY + (float)Main.rand.Next(-25, 26) * 0.05f) * 0.9f;
            Projectile.NewProjectile(position.X, position.Y, shardVelocityX, shardVelocityY, ProjectileID.BlackBolt, shardDamage, shardKB, player.whoAmI, 0f, 0f);

            // Fire three symmetric pairs of bullets alongside it
            Vector2 baseVelocity = new Vector2(speedX, speedY);
            for (int i = 0; i < 3; i++)
            {
                float randAngle = Main.rand.NextFloat(0.035f);
                float randVelMultiplier = Main.rand.NextFloat(0.92f, 1.08f);
                Vector2 left = baseVelocity.RotatedBy(-randAngle) * randVelMultiplier;
                Vector2 right = baseVelocity.RotatedBy(randAngle) * randVelMultiplier;
                Projectile.NewProjectile(position.X, position.Y, left.X, left.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Projectile.NewProjectile(position.X, position.Y, right.X, right.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < NotConsumeAmmo)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<OnyxChainBlaster>());
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
