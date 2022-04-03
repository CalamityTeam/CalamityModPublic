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
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = 16;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 4, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BouncingBettyProjectile>();
            Item.shootSpeed = 16f;
            Item.Calamity().rogue = true;
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
