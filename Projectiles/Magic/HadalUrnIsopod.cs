using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class HadalUrnIsopod : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                //Donor requested each bounce deal less damage
                Projectile.damage = (int)(0.8f * Projectile.damage);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            //Black Anurian bubble dust
            int inc;
            for (int i = 0; i < 25; i = inc + 1)
            {
                int hadalDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 0, default, 1f);
                Main.dust[hadalDust].position = (Main.dust[hadalDust].position + Projectile.position) / 2f;
                Main.dust[hadalDust].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[hadalDust].velocity.Normalize();
                Dust dust = Main.dust[hadalDust];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[hadalDust].alpha = Projectile.alpha;
                inc = i;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }
    }
}
