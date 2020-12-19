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
                "If you crit the target a second swarm of bullets will fire near the target");
        }

        public override void SetDefaults()
        {
            item.damage = 2310;
            item.ranged = true;
            item.width = 76;
            item.height = 30;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
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
                float SpeedX = speedX + (float)Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-15, 16) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<AMRShot>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
