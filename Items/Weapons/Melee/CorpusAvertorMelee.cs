using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class CorpusAvertorMelee : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corpus Avertor");
			Tooltip.SetDefault("Seems like it has worn down over time\n" +
				"Attacks grant lifesteal based on damage dealt\n" +
				"The lower your HP the more damage this weapon does and heals the player on enemy hits");
		}

		public override void SetDefaults()
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
			item.rare = 8;
			item.Calamity().customRarity = CalamityRarity.Dedicated;
			item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
			item.shootSpeed = 5f;
			item.melee = true;
		}

		// Gains 10% of missing health as base damage.
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			int lifeAmount = player.statLifeMax2 - player.statLife;
			flat += lifeAmount * 0.1f * player.MeleeDamage();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}
	}
}
