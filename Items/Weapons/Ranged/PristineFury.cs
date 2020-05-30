using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PristineFury : ModItem
    {
        public static int BaseDamage = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pristine Fury");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Fires an intense helix of flames that explode into a column of fire\n" +
                "Right click to fire a short ranged cloud of lingering flames\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.ranged = true;
            item.width = 88;
            item.height = 44;
            item.useTime = 3;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.rare = 10;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PristineFire>();
            item.shootSpeed = 11f;
            item.useAmmo = 23;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25, -10);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 5;
                item.useAnimation = 20;
            }
            else
            {
                item.useTime = 3;
                item.useAnimation = 15;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
				int flameAmt = 3;
				for (int index = 0; index < flameAmt; ++index)
				{
					float SpeedX = speedX + (float)Main.rand.Next(-25, 26) * 0.05f;
					float SpeedY = speedY + (float)Main.rand.Next(-25, 26) * 0.05f;
					Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PristineSecondary>(), (int)(damage * 0.8f), knockBack, player.whoAmI, 0f, 0f);
				}
            }
            else
            {
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PristineFire>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
			return false;
        }
    }
}
