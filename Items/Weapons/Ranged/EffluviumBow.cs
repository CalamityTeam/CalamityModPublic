using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class EffluviumBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effluvium Bow");
            Tooltip.SetDefault("Fires two arrows at once\n" +
                "Converts wooden arrows into mist arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 56;
            item.ranged = true;
            item.width = 26;
            item.height = 70;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MistArrow>();
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-30, 31) * 0.05f;

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<MistArrow>(), damage, knockBack, player.whoAmI);
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
