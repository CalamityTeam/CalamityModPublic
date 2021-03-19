using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class RefractionRotor : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Refraction Rotor");
			Tooltip.SetDefault("Fires a large prismatic razorblade\n" +
				"After hitting something, the blade dies moments later\n" +
				"The blade explodes into rockets on death\n" +
				"Stealth strikes explode into more rockets on death");
		}

		public override void SafeSetDefaults()
		{
			item.width = item.height = 120;
			item.damage = 300;
			item.knockBack = 8.5f;
			item.useAnimation = item.useTime = 14;
			item.Calamity().rogue = true;
			item.autoReuse = true;
			item.shootSpeed = 18f;
			item.shoot = ModContent.ProjectileType<RefractionRotorProjectile>();

			item.useStyle = ItemUseStyleID.SwingThrow;
			item.UseSound = SoundID.Item1;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.Violet;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int shuriken = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
			if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(shuriken))
				Main.projectile[shuriken].Calamity().stealthStrike = true;
			return false;
		}
	}
}
