using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class BouncingBetty : RogueWeapon
    {
        public const int BaseDamage = 52;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Betty");
            Tooltip.SetDefault("Throws a grenade which bounces after exploding\n" +
                               "Stealth strikes explode into a violent blast of fire and shrapnel when it bounces");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 24;
            item.useTime = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.width = 16;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 4, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<BouncingBettyProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            int projectileIndex = Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && projectileIndex.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[projectileIndex].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}
