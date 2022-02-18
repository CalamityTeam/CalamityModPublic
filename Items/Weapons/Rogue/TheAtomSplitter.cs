using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class TheAtomSplitter : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Atom Splitter");
			Tooltip.SetDefault("Throws a quantum-superimposed javelin that strikes from numerous timelines at once\n" +
				"Stealth strikes perform far more simultaneous strikes");
		}

		public override void SafeSetDefaults()
		{
			item.width = item.height = 128;
			item.damage = 320;
			item.knockBack = 7f;
			item.useAnimation = item.useTime = 25;
			item.Calamity().rogue = true;
			item.autoReuse = true;
			item.shootSpeed = 24f;
			item.shoot = ModContent.ProjectileType<TheAtomSplitterProjectile>();

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
			if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(javelin)) {
				Main.projectile[javelin].Calamity().stealthStrike = true;
				Main.projectile[javelin].damage = (int)(1.10 * Main.projectile[javelin].damage);
			}
			return false;
		}
	}
}
