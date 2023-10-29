using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlanetaryAnnihilationProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dustType = 0;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            switch ((int)Projectile.ai[1])
            {
                case 0:
                    dustType = 15;
                    break;
                case 1:
                    dustType = 74;
                    break;
                case 2:
                    dustType = 73;
                    break;
                case 3:
                    dustType = 162;
                    break;
                case 4:
                    dustType = 90;
                    break;
                case 5:
                    dustType = 173;
                    break;
                case 6:
                    dustType = 57;
                    break;
            }
            int addedDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2.2f);
            Main.dust[addedDust].noGravity = true;
            Main.dust[addedDust].velocity *= 0f;

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 60);
        }
        public override void OnKill(int timeLeft)
        {
            int height = 40;
            Vector2 dustRotation = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 dustVel = dustRotation * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            int inc;
            for (int i = 0; i < 20; i = inc + 1)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 200, default, 2.1f);
                Main.dust[dustID].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[dustID].noGravity = true;
                Dust dust = Main.dust[dustID];
                dust.velocity *= 3f;
                dust = Main.dust[dustID];
                dust.velocity += dustVel * Main.rand.NextFloat();
                dustID = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.1f);
                Main.dust[dustID].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[dustID];
                dust.velocity *= 2f;
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].fadeIn = 1f;
                dust = Main.dust[dustID];
                dust.velocity += dustVel * Main.rand.NextFloat();
                inc = i;
            }
            for (int j = 0; j < 10; j = inc + 1)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[dustID].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Main.dust[dustID].noGravity = true;
                Dust dust = Main.dust[dustID];
                dust.velocity *= 0.5f;
                dust = Main.dust[dustID];
                dust.velocity += dustVel * (0.6f + 0.6f * Main.rand.NextFloat());
                inc = j;
            }
        }
    }
}
