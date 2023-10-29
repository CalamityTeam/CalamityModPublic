using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SolarBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Ray;
            Projectile.friendly = true;
            Projectile.extraUpdates = 220;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 9f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustPos = Projectile.position;
                    dustPos -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int dust = Dust.NewDust(dustPos, 1, 1, 246, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].position = dustPos;
                    Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
        }
    }
}
