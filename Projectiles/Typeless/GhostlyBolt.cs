using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class GhostlyBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 6f)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                for (int i = 0; i < 40; i++)
                {
                    int cursedDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 181, 0f, 0f, 100, default, 1f);
                    Main.dust[cursedDust].velocity *= 3f;
                    Main.dust[cursedDust].velocity += Projectile.velocity * 0.75f;
                    Main.dust[cursedDust].scale *= 1.2f;
                    Main.dust[cursedDust].noGravity = true;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 6f)
            {
                for (int j = 0; j < 3; j++)
                {
                    int cursedDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 181, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[cursedDust2].velocity *= 0.6f;
                    Main.dust[cursedDust2].scale *= 1.4f;
                    Main.dust[cursedDust2].noGravity = true;
                }
            }
        }
    }
}
