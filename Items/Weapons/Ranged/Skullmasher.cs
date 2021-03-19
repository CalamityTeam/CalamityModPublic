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
            Tooltip.SetDefault("Sniper shotgun, because why not?\n" +
                "Converts musket balls into high velocity bullets that, on crit, fire a second swarm of bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 924;
            item.ranged = true;
            item.width = 76;
            item.height = 30;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 5;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 5; ++index)
            {
				if (type == ProjectileID.Bullet)
					Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<AMRShot>(), damage, knockBack, player.whoAmI);
				else
					Projectile.NewProjectile(position, new Vector2(speedX, speedY), (int)(damage * 0.7), damage, knockBack, player.whoAmI);
			}
            return false;
        }
    }
}
