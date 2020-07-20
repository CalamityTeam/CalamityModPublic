using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ToxicantTwister : RogueWeapon
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Toxicant Twister");
			Tooltip.SetDefault("Throws a slow moving boomerang\n" +
				"After a few moments, the boomerang chooses a target and rapidly homes in\n" +
				"Stealth strikes fire three boomerangs that home in faster and rapidly release sand");
		}

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 46;
            item.damage = 750;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = item.useTime = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
			item.rare = 10;
            item.shoot = ModContent.ProjectileType<ToxicantTwisterTwoPointZero>();
            item.shootSpeed = 18f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
			{
				for (int i = 0; i < 3; i++)
				{
					float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
					float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
					int stealth = Projectile.NewProjectile(position, new Vector2(SpeedX, SpeedY), type, damage, knockBack, player.whoAmI);
					Main.projectile[stealth].Calamity().stealthStrike = true;
				}
			}
			else
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
            return false;
        }
    }
}
