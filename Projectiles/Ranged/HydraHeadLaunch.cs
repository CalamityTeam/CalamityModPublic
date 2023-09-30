using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class HydraHeadLaunch : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Hydra";

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        //Normal hitbox size for explosion, rotated otherwise
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => (Projectile.ai[1] < 1f ? Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size()) : null);

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(Projectile.ai[0]);
            Projectile.ai[0] += 12f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 60);

        public override void OnKill(int timeLeft)
        {
            Projectile.ai[1] = 1f;
            Projectile.ExpandHitboxBy(180);
            Projectile.Damage();

            float randAngle = Main.rand.NextFloat(1f, 2f);
            //8-pointed star
            for (int i = 0; i < 8; i++)
            {
                float angle = MathHelper.Pi * randAngle - i * MathHelper.TwoPi / 8f;
                float nextAngle = MathHelper.Pi * randAngle - (i + 2) * MathHelper.TwoPi / 8f;
                Vector2 start = angle.ToRotationVector2();
                Vector2 end = nextAngle.ToRotationVector2();
                for (int j = 0; j < 30; j++)
                {
                    Dust starDust = Dust.NewDustPerfect(Projectile.Center, 267);
                    starDust.scale = 1.5f;
                    starDust.velocity = Vector2.Lerp(start, end, j / 30f) * 12f;
                    starDust.color = Color.Orchid;
                    starDust.noGravity = true;
                }
            }
            SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound, Projectile.Center);
        }
    }
}