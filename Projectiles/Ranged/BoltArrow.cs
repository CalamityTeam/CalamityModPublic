using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class BoltArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 60);

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 15f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 rotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    rotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    rotate = rotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int blueDust = Dust.NewDust(Projectile.Center, 0, 0, 221, 0f, 0f, 160, default, 1f);
                    Main.dust[blueDust].scale = 1.1f;
                    Main.dust[blueDust].noGravity = true;
                    Main.dust[blueDust].position = Projectile.Center + rotate;
                    Main.dust[blueDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[blueDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[blueDust].position) * 1.25f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage = (int)(Projectile.damage * 0.6f);
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int rando = Main.rand.Next(10, 20);
            for (int i = 0; i < rando; i++)
            {
                int dust = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
