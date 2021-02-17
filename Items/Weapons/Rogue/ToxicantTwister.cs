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
				"Stealth strikes home in faster and rapidly release sand");
		}

		public override void SafeSetDefaults()
		{
			item.width = 42;
			item.height = 46;
			item.damage = 308;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = item.useTime = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<ToxicantTwisterTwoPointZero>();
			item.shootSpeed = 18f;
			item.Calamity().rogue = true;

			item.value = CalamityGlobalItem.Rarity13BuyPrice;
			item.Calamity().customRarity = CalamityRarity.PureGreen;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			double stealthMult = player.Calamity().StealthStrikeAvailable() ? 1.3333 : 1D;
			int boomer = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * stealthMult), knockBack, player.whoAmI);
			if (boomer.WithinBounds(Main.maxProjectiles))
				Main.projectile[boomer].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
			return false;
		}
	}
}
