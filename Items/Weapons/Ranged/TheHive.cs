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
            Tooltip.SetDefault("Launches a variety of rockets that explode into bees on death");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 62;
            item.height = 30;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item61;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BeeRPG>();
            item.shootSpeed = 13f;
            item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    type = ModContent.ProjectileType<GoliathRocket>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<HiveMissile>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<HiveBomb>();
                    break;
                case 3:
                    type = ModContent.ProjectileType<BeeRPG>();
                    break;
                default:
                    break;
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
