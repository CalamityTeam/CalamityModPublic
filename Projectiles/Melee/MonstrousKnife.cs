using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class MonstrousKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 30f)
            {
                Projectile.alpha += 10;
                if (Projectile.damage > 1)
                    Projectile.damage = (int)(Projectile.damage * 0.9);
                Projectile.knockBack = Projectile.knockBack * 0.9f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.ai[0] < 30f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            for (int dustIndex = 0; dustIndex < 3; ++dustIndex)
            {
                int redDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TheDestroyer, 0f, 0f, 100, new Color(), 0.8f);
                Dust dust = Main.dust[redDust];
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ProjectileID.VampireHeal, 1, 3f); //3x cooldown = 23.3 HP bank, 2 hp/s refill
    }
}
