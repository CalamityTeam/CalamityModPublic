using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SealedSingularity";

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 70f)
            {
                Projectile.velocity *= 0.96f;
            }
            else
            {
                Projectile.rotation += 0.3f * (float)Projectile.direction;
            }
        }

        public override void OnKill (int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            //glass-pot break sound
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);

            int blackhole = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SealedSingularityBlackhole>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.Calamity().stealthStrike ? -180f : 0f, 0f);
            if (blackhole.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[blackhole].Center = Projectile.Center;
                Main.projectile[blackhole].Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
            }

            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                float SpeedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<SealedSingularityGore>(), (int)(Projectile.damage * 0.25), 0f, Projectile.owner, index, 0f);
            }
        }
    }
}
