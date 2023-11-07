using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicSpiritBomb3 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            float scaleModd = (float)Main.mouseTextColor / 200f - 0.35f;
            scaleModd *= 0.2f;
            Projectile.scale = scaleModd + 0.95f;

            float projDistance = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
            if (projDistance <= 2f)
            {
                projDistance = 1f;
            }
            else
            {
                if (projDistance > 8f)
                {
                    projDistance = 12f;
                }
                else if (projDistance > 6f)
                {
                    projDistance = 9f;
                }
                else if (projDistance > 5f)
                {
                    projDistance = 7f;
                }
                else if (projDistance > 4f)
                {
                    projDistance = 5f;
                }
                else if (projDistance > 3f)
                {
                    projDistance = 4f;
                }
                else if (projDistance > 2.5f)
                {
                    projDistance = 3f;
                }
                else
                {
                    projDistance = 2f;
                }
            }
            Projectile.velocity = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * projDistance;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.oldVelocity.X * 2.5f, Projectile.oldVelocity.Y * 2.5f);
            }
        }
    }
}
