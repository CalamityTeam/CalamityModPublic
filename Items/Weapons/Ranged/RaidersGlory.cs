using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RaidersGlory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Raider's Glory");
            Tooltip.SetDefault("Fires ichor arrows with increased velocity\n" +
            "These arrows also cause enemies to drop more money");
        }

        public override void SetDefaults()
        {
            item.damage = 44;
            item.ranged = true;
            item.width = 50;
            item.height = 22;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.IchorArrow, damage, knockBack, player.whoAmI);
            Main.projectile[projectile].extraUpdates++;
            return false;
        }
    }
}
