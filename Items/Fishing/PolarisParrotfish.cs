using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Fishing
{
    public class PolarisParrotfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polaris Parrotfish");
            Tooltip.SetDefault("It carries the mark of the Northern Star\n" +
				"Projectile hits grant buffs to the weapon and the player\n" +
				"Buffs are removed on hit");
            Item.staff[item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.ranged = true;
            item.width = 38;
            item.height = 34;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"); //pew pew
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PolarStar>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.polarisBoostThree)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PolarStar>(), damage, knockBack, player.whoAmI, 0f, 2f);
                return false;
            }
			else if (modPlayer.polarisBoostTwo)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PolarStar>(), (int)((double)damage * 1.25), knockBack, player.whoAmI, 0f, 1f);
                return false;
            }
            return true;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(10, 10);
        }
    }
}
