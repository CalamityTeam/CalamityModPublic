using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimAngelicLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.scale = CalamityUtils.Convert01To010(Projectile.timeLeft / 10f);
            Projectile.Opacity = (float)Math.Sqrt(Projectile.scale);
            Projectile.velocity *= 0.8f;
            Projectile.frame = Main.projFrames[Projectile.type] - Projectile.timeLeft;
        }

        public override void OnKill(int timeLeft)
        {
            // Release a puff of golden light dust.
            for (int i = 0; i < 15; i++)
            {
                Dust light = Dust.NewDustPerfect(Projectile.Center, 267);
                light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                light.velocity = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5f;
                light.scale = 1.35f;
                light.noGravity = true;
            }

            // Release a laser at the nearest target, if one exists.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(1300f, true, true);
            if (Main.myPlayer != Projectile.owner || potentialTarget is null)
                return;

            int damage = Projectile.damage;
            Vector2 laserDirection = Projectile.SafeDirectionTo(potentialTarget.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, laserDirection, ModContent.ProjectileType<SeraphimBeamLarge>(), damage, 0f, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255 - Projectile.alpha);

        public override bool? CanDamage() => false;
    }
}
