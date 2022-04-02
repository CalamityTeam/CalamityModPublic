using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class NadirSpear : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nadir");
        }

        public override void SetDefaults()
        {
            projectile.width = 55;
            projectile.height = 55;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override float InitialSpeed => 5.5f;
        public override float ReelbackSpeed => 2.1f;
        public override float ForwardSpeed => 1.1f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            int damage = projectile.damage / 4;
            float kb = projectile.knockBack * 0.5f;
            Vector2 projPos = projectile.Center + projectile.velocity;
            Vector2 projVel = projectile.velocity * 0.75f;
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projPos, projVel, ModContent.ProjectileType<VoidEssence>(), damage, kb, projectile.owner, 0f, 0f);

            // Play a screaming soul sound effect (unused Shadowflame Hex Doll noise)
            Main.PlaySound(SoundID.Item104, projectile.Center);

            // Create a circle of purple dust where the projectile comes out, looking like the edge of a portal
            int circleDust = 18;
            Vector2 baseDustVel = new Vector2(3.8f, 0f);
            for (int i = 0; i < circleDust; ++i)
            {
                int dustID = 27;
                float angle = i * (MathHelper.TwoPi / circleDust);
                Vector2 dustVel = baseDustVel.RotatedBy(angle);

                int idx = Dust.NewDust(projectile.Center, 1, 1, dustID);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].position = projectile.Center;
                Main.dust[idx].velocity = dustVel;
                Main.dust[idx].scale = 2.4f;
            }
        };
        public override void ExtraBehavior()
        {
            int movingDust = 3;
            for (int i = 0; i < movingDust; ++i)
            {
                int dustID = Main.rand.NextBool(4) ? 27 : 118;
                Vector2 corner = 0.5f * projectile.position + 0.5f * projectile.Center;
                int idx = Dust.NewDust(corner, projectile.width / 2, projectile.height / 2, dustID);

                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
            }
        }
    }
}
