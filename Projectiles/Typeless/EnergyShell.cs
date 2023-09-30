using CalamityMod.Cooldowns;
using CalamityMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class EnergyShell : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 72;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
                playedSound = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 6)
            {
                Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            if (Projectile.timeLeft < 51) //fade out
            {
                Projectile.alpha += 5;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            //if player is dead, null, or stops holding the Lion Heart, kill the projectile
            if (player.dead || player is null || player.ActiveItem().type != ModContent.ItemType<LionHeart>())
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
            player.AddCooldown(LionHeartShield.ID, CalamityUtils.SecondsToFrames(45));
        }

        public override bool? CanDamage() => false;
    }
}
