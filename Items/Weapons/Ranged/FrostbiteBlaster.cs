using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FrostbiteBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostbite Blaster");
            Tooltip.SetDefault("Fires a spread of 6 bullets\n" +
                "Converts musket balls into icicles");
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 22;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.reuseDelay = 54;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Blizzard;
            Item.shootSpeed = 9f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            SoundEngine.PlaySound(SoundID.Item36, position);
            for (int i = 0; i < 2; i++)
            {
                float newSpeedX = speedX + Main.rand.Next(-40, 41) * 0.06f;
                float newSpeedY = speedY + Main.rand.Next(-40, 41) * 0.06f;

                if (type == ProjectileID.Bullet)
                {
                    int p = Projectile.NewProjectile(position.X, position.Y, newSpeedX, newSpeedY, ProjectileID.Blizzard, damage, knockBack, player.whoAmI);
                    // Main.projectile[p].magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                    Main.projectile[p].DamageType = DamageClass.Ranged;
                }
                else
                    Projectile.NewProjectile(position.X, position.Y, newSpeedX, newSpeedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
