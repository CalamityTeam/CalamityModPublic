using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Malevolence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malevolence");
            Tooltip.SetDefault("Fires two arrows at once\n" +
                "Converts wooden arrows into plague arrows that explode into bees on death");
        }

        public override void SetDefaults()
        {
            item.damage = 56;
            item.ranged = true;
            item.width = 36;
            item.height = 58;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item97;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<PlagueArrow>();
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = speedX + Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-20, 21) * 0.05f;

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PlagueArrow>(), damage, knockBack, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }
    }
}
