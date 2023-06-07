using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AetherBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public const int DoubleDamageTime = 90;
        public ref float BeamLength => ref Projectile.localAI[0];

        public bool mainBeam => Projectile.ai[0] == 0f;

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 6;
            Projectile.timeLeft = 30 * Projectile.MaxUpdates; // 30 effective, 180 total
        }

        public override void AI()
        {
            if (mainBeam)
                Projectile.damage += Projectile.originalDamage / DoubleDamageTime;
            else
                Projectile.tileCollide = false;

            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);
            BeamLength = MathHelper.Clamp(BeamLength + 2f, 0f, 100f);
            Lighting.AddLight(Projectile.Center, 1f, 0f, 0.7f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 50, 200, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(100f, 2f, lightColor);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Splitty
            if (Projectile.owner == Main.myPlayer && mainBeam)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / 8f) - (MathHelper.Pi / 8f)).ToRotationVector2() * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 600);
    }
}
