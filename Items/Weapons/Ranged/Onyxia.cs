using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    class Onyxia : ModItem
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
            item.damage = 235;
            item.ranged = true;
            item.width = 84;
            item.height = 34;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 28f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 3);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals triple damage and double knockback
            int shardDamage = 3 * damage;
            float shardKB = 2f * knockBack;
            float shardVelocityX = (speedX + (float)Main.rand.Next(-25, 26) * 0.05f) * 0.9f;
            float shardVelocityY = (speedY + (float)Main.rand.Next(-25, 26) * 0.05f) * 0.9f;
            Projectile.NewProjectile(position.X, position.Y, shardVelocityX, shardVelocityY, 661, shardDamage, shardKB, player.whoAmI, 0f, 0f);

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
