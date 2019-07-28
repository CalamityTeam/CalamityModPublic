using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons.RareVariants
{
    public class ClothiersWrath : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clothier's Wrath");
		}

		public override void SetDefaults()
		{
			item.damage = 24;
			item.magic = true;
			item.mana = 15;
			item.width = 28;
			item.height = 30;
			item.useTime = 22;
			item.useAnimation = 22;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3f;
			item.value = Item.buyPrice(0, 4, 0, 0);
			item.rare = 3;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = ProjectileID.ClothiersCurse;
			item.shootSpeed = 6f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numProj = 2;
			float rotation = MathHelper.ToRadians(2);
			for (int i = 0; i < numProj + 1; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
				int proj = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMagic = true;
			}
			return false;
		}
	}
}
