using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class StarSwallowerContainmentUnit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Swallower Containment Unit");
		}

		public override void SetDefaults()
		{
			item.shootSpeed = 10f;
			item.damage = 20;
			item.mana = 10;
			item.width = 18;
			item.height = 28;
			item.useTime = item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 2.25f;
			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = 3;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<StarSwallowerSummon>();
			item.shootSpeed = 10f;
			item.summon = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
			if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).active())
			{
				Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}
	}
}
