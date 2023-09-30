using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MortalityBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Color ProjectileColor => Main.hslToRgb(Projectile.localAI[0], 1f, 0.5f);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
            if (!Main.dedServ)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust rainbowDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), 261);
                    rainbowDust.color = ProjectileColor;
                    rainbowDust.velocity += Projectile.velocity;
                    rainbowDust.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust rainbowDust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), 40, 40, 261);
                    rainbowDust.color = ProjectileColor;
                    rainbowDust.noGravity = true;
                }
            }
        }
    }
}
