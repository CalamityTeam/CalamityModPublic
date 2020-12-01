using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class CorpusAvertor : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corpus Avertor");
			Tooltip.SetDefault("Seems like it has worn down over time\n" +
				"Attacks grant lifesteal based on damage dealt\n" +
				"The lower your HP the more damage this weapon does and heals the player on enemy hits");
		}

		public override void SafeSetDefaults()
		{
			item.width = 18;
			item.height = 32;
			item.damage = 98;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTime = 15;
			item.knockBack = 3f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(gold: 80);
			item.rare = ItemRarityID.Yellow;
			item.Calamity().customRarity = CalamityRarity.Dedicated;
			item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
			item.shootSpeed = 5f;
			item.Calamity().rogue = true;
		}

		// Gains 10% of missing health as base damage.
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			int lifeAmount = player.statLifeMax2 - player.statLife;
			flat += lifeAmount * 0.1f * player.RogueDamage();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int dagger = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
			Main.projectile[dagger].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
			return false;
		}
	}
}
