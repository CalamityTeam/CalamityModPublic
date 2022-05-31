using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GammaCanister : ModProjectile
    {
        public const float Gravity = 0.2f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Canister");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            NPC potentialTarget = Projectile.Center.MinionHoming(1450f, Main.player[Projectile.owner]);
            if (Projectile.timeLeft >= 180)
                Projectile.velocity.Y += Gravity;

            else if (potentialTarget != null)
            {
                // This looks quite stupid but the weapon is going to be useless otherwise and I'm not reworking this thing a 3rd time.
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center) * 18f, 0.18f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 22, 0, 255);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            for (int i = 0; i < 6; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * Main.rand.NextFloat(6f, 17f);
                int bullet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + shootVelocity * 2f, shootVelocity, ModContent.ProjectileType<HomingGammaBullet>(), Projectile.damage / 2, Projectile.knockBack * 0.4f, Projectile.owner);
                Main.projectile[bullet].originalDamage = Projectile.originalDamage / 2;
            }
        }
    }
}
