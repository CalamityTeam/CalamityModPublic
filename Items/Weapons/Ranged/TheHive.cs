using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive");
            Tooltip.SetDefault("Launches a variety of rockets that explode into bees on death\n" +
                "Rockets will destroy tiles with tile-destroying ammo");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 30;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeRPG>();
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int rocket = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.ProjectileType<GoliathRocket>(),
                ModContent.ProjectileType<HiveMissile>(),
                ModContent.ProjectileType<HiveBomb>(),
                ModContent.ProjectileType<BeeRPG>()
            });
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, rocket, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
