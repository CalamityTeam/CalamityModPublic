using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
	public class P90 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("P90");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "It's a bullet hose");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.ranged = true;
            item.width = 60;
            item.height = 28;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 7f;
            item.useAmmo = AmmoID.Bullet;
			item.Calamity().canFirePointBlankShots = true;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14, -1);
        }

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float SpeedX = speedX + Main.rand.Next(-15, 16) * 0.05f;
			float SpeedY = speedY + Main.rand.Next(-15, 16) * 0.05f;
			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}

        public override bool ConsumeAmmo(Player player) => Main.rand.NextFloat() < 0.5f;
    }
}
