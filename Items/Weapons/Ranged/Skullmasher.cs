using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Ranged
{
    public class Skullmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skullmasher");
            Tooltip.SetDefault("Makes their brain hurt\n" +
                "Fires a spread of 4 high velocity bullets that split into additional bullets upon hitting an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 880;
            item.ranged = true;
            item.width = 142;
            item.height = 40;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.DarkBlue;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
		}

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 5;

        public override Vector2? HoldoutOffset() => new Vector2(-50, 0); //beeg gun

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			for (int index = 0; index < 4; ++index)
			{
				float SpeedX = speedX + Main.rand.Next(-15, 16) * 0.05f;
				float SpeedY = speedY + Main.rand.Next(-15, 16) * 0.05f;
				Projectile.NewProjectile(position, new Vector2(SpeedX, SpeedY), ModContent.ProjectileType<AMRShot>(), damage, knockBack, player.whoAmI, type, 1f);
			}

            return false;
        }
    }
}
