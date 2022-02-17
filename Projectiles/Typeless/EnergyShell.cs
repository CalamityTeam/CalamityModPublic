using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.UI.CooldownIndicators;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class EnergyShell : ModProjectile
    {
		private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Shell");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 72;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
			if (!playedSound)
			{
				Main.PlaySound(SoundID.Item92, (int)projectile.position.X, (int)projectile.position.Y);
				playedSound = true;
			}
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 6)
            {
                projectile.frame = 0;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
			if (projectile.timeLeft < 51) //fade out
			{
				projectile.alpha += 5;
			}
            Player player = Main.player[projectile.owner];
            projectile.Center = player.Center;
			//if player is dead, null, or stops holding the Lion Heart, kill the projectile
			if (player.dead || player is null || player.ActiveItem().type != ModContent.ItemType<LionHeart>())
			{
				projectile.Kill();
			}
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            Main.PlaySound(SoundID.Item94, (int)projectile.position.X, (int)projectile.position.Y);
            player.Calamity().Cooldowns.Add(new LionHeartShieldCooldown(CalamityUtils.SecondsToFrames(45), player));
		}

        public override bool CanDamage() => false;
    }
}
