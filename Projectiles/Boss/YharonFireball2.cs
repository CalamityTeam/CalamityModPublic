using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class YharonFireball2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/YharonFireball";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Fireball");
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
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            CooldownSlot = 1;
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
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
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

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = SoundID.Item14.Volume * 0.5f }, Projectile.position);
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

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.velocity.Y >= -16f)
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 240);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = Projectile;
        }
    }
}
