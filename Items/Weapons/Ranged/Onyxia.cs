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
            item.damage = 90;
            item.ranged = true;
            item.width = 84;
            item.height = 34;
            item.useTime = item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.BlackBolt;
            item.shootSpeed = 28f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 3);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);

            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals 145% damage and double knockback
            int shardDamage = (int)(1.45f * damage);
            float shardKB = 2f * knockBack;
            Projectile shard = Projectile.NewProjectileDirect(position, velocity, ProjectileID.BlackBolt, shardDamage, shardKB, player.whoAmI, 0f, 0f);
            shard.timeLeft = (int)(shard.timeLeft * 1.4f);

            // Fire three symmetric pairs of bullets alongside it
            for (int i = 0; i < 3; i++)
            {
                float randAngle = Main.rand.NextFloat(0.035f);
                float randVelMultiplier = Main.rand.NextFloat(0.92f, 1.08f);
                Vector2 ccwVelocity = velocity.RotatedBy(-randAngle) * randVelMultiplier;
                Vector2 cwVelocity = velocity.RotatedBy(randAngle) * randVelMultiplier;
                Projectile.NewProjectile(position, ccwVelocity, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Projectile.NewProjectile(position, cwVelocity, type, damage, knockBack, player.whoAmI, 0f, 0f);
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
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 8);
            r.AddTile(ModContent.TileType<CosmicAnvil>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
