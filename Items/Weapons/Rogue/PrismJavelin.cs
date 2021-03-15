using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class PrismJavelin : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prism Javelin");
			Tooltip.SetDefault("Fires a javelin that releases energy duplicates at enemies it hits\n" +
				"Stealth strikes release duplicates much more quickly");
		}

		public override void SafeSetDefaults()
		{
			item.width = item.height = 128;
			item.damage = 375;
			item.knockBack = 7f;
			item.useAnimation = item.useTime = 14;
			item.Calamity().rogue = true;
			item.autoReuse = true;
			item.shootSpeed = 24f;
			item.shoot = ModContent.ProjectileType<PrismJavelinProjectile>();

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
			int javelin = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, -1f);
			if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(javelin))
				Main.projectile[javelin].Calamity().stealthStrike = true;
			return false;
		}
	}
}
