using Terraria.DataStructures;
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
            Item.damage = 90;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 84;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BlackBolt;
            Item.shootSpeed = 28f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 3);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals 145% damage and double knockback
            int shardDamage = (int)(1.45f * damage);
            float shardKB = 2f * knockBack;
            Projectile shard = Projectile.NewProjectileDirect(source, position, velocity, ProjectileID.BlackBolt, shardDamage, shardKB, player.whoAmI, 0f, 0f);
            shard.timeLeft = (int)(shard.timeLeft * 1.4f);

            // Fire three symmetric pairs of bullets alongside it
            for (int i = 0; i < 3; i++)
            {
                float randAngle = Main.rand.NextFloat(0.035f);
                float randVelMultiplier = Main.rand.NextFloat(0.92f, 1.08f);
                Vector2 ccwVelocity = velocity.RotatedBy(-randAngle) * randVelMultiplier;
                Vector2 cwVelocity = velocity.RotatedBy(randAngle) * randVelMultiplier;
                Projectile.NewProjectile(source, position, ccwVelocity, type, damage, knockback, player.whoAmI, 0f, 0f);
                Projectile.NewProjectile(source, position, cwVelocity, type, damage, knockback, player.whoAmI, 0f, 0f);
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OnyxChainBlaster>()).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<DarksunFragment>(), 8).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
