using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class ParasiticSceptor : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Parasitic Scepter");
			Tooltip.SetDefault("Fires a spread of water leeches that latch onto enemies, dealing a stacking damage over time");
			Item.staff[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 12;
			item.knockBack = 3f;
			item.mana = 10;
			item.useTime = item.useAnimation = 25;
			item.autoReuse = true;
			item.magic = true;
			item.shootSpeed = 10f;
			item.shoot = ModContent.ProjectileType<WaterLeechProj>();

			item.width = item.height = 52;
			item.UseSound = SoundID.Item46;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.rare = 2;
			item.value = CalamityGlobalItem.Rarity2BuyPrice;
		}

		public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int leechAmt = 2;
			if (Main.rand.NextBool(3))
			{
				leechAmt++;
			}
			if (Main.rand.NextBool(4))
			{
				leechAmt++;
			}
			if (Main.rand.NextBool(5))
			{
				leechAmt++;
			}
			CalamityUtils.ProjectileToMouse(player, leechAmt, item.shootSpeed, 0.05f, 25f, type, damage, knockBack, player.whoAmI, false);
			return false;
		}
	}
}
