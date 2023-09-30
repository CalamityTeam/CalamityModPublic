using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class YharonFireball2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override string Texture => "CalamityMod/Projectiles/Boss/YharonFireball";

        public static readonly SoundStyle FireballSound = new("CalamityMod/Sounds/Custom/Yharon/YharonFireball", 3);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.Opacity = 0.25f;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            if (Projectile.velocity.Y >= -16f)
            {
                if (Projectile.Opacity < 1f)
                {
                    Projectile.Opacity = 1f;
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                    int dustAmount = 36;
                    for (int i = 0; i < dustAmount; i++)
                    {
                        Vector2 dustSpawnPosition = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                        dustSpawnPosition = dustSpawnPosition.RotatedBy((double)((float)(i - (dustAmount / 2 - 1)) * MathHelper.TwoPi / (float)dustAmount), default) + Projectile.Center;
                        Vector2 dustVelocity = dustSpawnPosition - Projectile.Center;
                        int dust = Dust.NewDust(dustSpawnPosition + dustVelocity, 0, 0, 55, dustVelocity.X, dustVelocity.Y);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                        Main.dust[dust].velocity = dustVelocity;
                    }
                }
            }

            if (Projectile.velocity.Y < -1f)
            {
                // 129 frames to get from -50 to -1
                Projectile.velocity.Y *= 0.97f;
            }
            else
            {
                // 85 frames to get from -1 to 16
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            Projectile.velocity.X *= 0.995f;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(FireballSound, Projectile.Center);
            }

            if (Projectile.ai[0] >= 2f)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            if (Main.rand.NextBool(16))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 55, 0f, 0f, 200, default, 1f);
                dust.scale *= 0.7f;
                dust.velocity += Projectile.velocity * 0.25f;
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.velocity.Y >= -16f;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = SoundID.Item14.Volume * 0.5f }, Projectile.Center);
            Projectile.ExpandHitboxBy(144);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 0f, 0f, 100, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 0f, 0f, 100, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
            Projectile.Damage();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.velocity.Y >= -16f)
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 60);
        }
    }
}
