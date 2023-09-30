using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PenumbraSoul : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.height = 18;
            Projectile.width = 18;
            Projectile.timeLeft = 150;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.alpha = 80;

            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            DrawOffsetX = 1;
            DrawOriginOffsetY = 4;

            // Continuously trail dust
            int trailDust = 1;
            for (int i = 0; i < trailDust; ++i)
            {
                int dustID = DustID.Shadowflame;

                int idx = Dust.NewDust(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, dustID,0f,0f, 0, new Color(38, 30, 43));
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity += Projectile.velocity * 0.8f;
            }

            // If tentacle is currently on cooldown, reduce the cooldown.
            if (Projectile.ai[0] > 0f)
                Projectile.ai[0] -= 1f;

            // Home in on nearby enemies if homing is enabled
            if (Projectile.ai[1] == 0f)
                HomingAI();
        }

        private void HomingAI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Rapidly screech to a halt upon touching an enemy and disable homing.
            Projectile.velocity *= 0.4f;
            Projectile.ai[1] = 1f;

            // Fade out a bit with every hit
            Projectile.alpha += 20;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            // Explode into dust (as if being shredded apart on contact)
            int onHitDust = Main.rand.Next(6, 11);
            for (int i = 0; i < onHitDust; ++i)
            {
                int dustID = DustID.Shadowflame;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f,0, new Color(38, 30, 43));

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(1.4f, 2.6f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Create a burst of dust
            int killDust = Main.rand.Next(30, 41);
            for (int i = 0; i < killDust; ++i)
            {
                int dustID = DustID.Shadowflame;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 0, new Color(38, 30, 43));

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(2.0f, 3.1f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }
    }
}
