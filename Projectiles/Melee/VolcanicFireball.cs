using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class VolcanicFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.MoveShort;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.SporeCloud;
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
            {
                Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0f);
            if (Main.rand.NextBool(4))
            {
                int volcano = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 16 : 174, 0f, 0f);
                Main.dust[volcano].noGravity = true;
                Main.dust[volcano].velocity *= 0f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                int volcano = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 174, 0f, 0f);
                Main.dust[volcano].noGravity = true;
                Main.dust[volcano].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, Projectile.alpha);
        }
    }
}
