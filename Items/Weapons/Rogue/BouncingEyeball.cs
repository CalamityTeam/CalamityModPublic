using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BouncingEyeball : RogueWeapon
    {
        public const int BaseDamage = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Eyeball");
            Tooltip.SetDefault("Throws an eyeball that bounces off of surfaces.\n" +
                               "Knockback is much stronger during a blood moon\n" +
                               "Stealth strikes cause the eyeballs to move much faster and bounce more energetically");
        }
        public override void SafeSetDefaults()
        {
            Item.damage = BaseDamage;
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.Calamity().rogue = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<BouncingEyeballProjectile>();
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool bloodMoon = Main.bloodMoon;
            Vector2 initialVelocity = new Vector2(speedX, speedY);
            if (bloodMoon)
            {
                knockBack *= 3f;
            }
            if (player.Calamity().StealthStrikeAvailable())
            {
                initialVelocity *= 2f;
                int p = Projectile.NewProjectile(position, initialVelocity, ModContent.ProjectileType<BouncingEyeballProjectileStealthStrike>(), damage, knockBack, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            else
            {
                initialVelocity *= Main.rand.NextFloat(0.85f, 1.3f);
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(10f)); //random spread
                Projectile.NewProjectile(position, initialVelocity, ModContent.ProjectileType<BouncingEyeballProjectile>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
