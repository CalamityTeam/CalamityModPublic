using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class BouncingEyeball : CalamityDamageItem
    {
        public const int BaseDamage = 12;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bouncing Eyeball");
			Tooltip.SetDefault("Throws an eyeball that bounces off of surfaces.\n" +
                               "Knockback is much stronger during a blood moon\n" +
                               "Stealth Strike Effect: Eyeballs move much faster and bounce more energetically");
		}
		public override void SafeSetDefaults()
		{
			item.damage = BaseDamage;
            item.width = 26;
			item.height = 26;
			item.useTime = 23;
			item.useAnimation = 23;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 3.5f;
            item.rare = 1;
            item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
            item.noUseGraphic = true;
			item.UseSound = SoundID.Item1;
            item.shoot = mod.ProjectileType("BouncingEyeballProjectile");
            item.shootSpeed = 10f;
            item.autoReuse = true;
		}
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool bloodMoon = Main.bloodMoon;
            Vector2 initialVelocity = new Vector2(speedX, speedY);
            if (bloodMoon)
            {
                knockBack *= 3f;
            }
            if (player.GetCalamityPlayer().StealthStrikeAvailable())
            {
                initialVelocity *= 2f;
                int p = Projectile.NewProjectile(position, initialVelocity, mod.ProjectileType("BouncingEyeballProjectileStealthStrike"),damage,knockBack,player.whoAmI);
                Main.projectile[p].GetCalamityProj().stealthStrike = true;
            }
            else
            {
                initialVelocity *= Main.rand.NextFloat(0.85f, 1.3f);
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(10f)); //random spread
                Projectile.NewProjectile(position, initialVelocity, mod.ProjectileType("BouncingEyeballProjectile"), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
