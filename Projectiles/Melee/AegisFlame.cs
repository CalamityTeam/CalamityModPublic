using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AegisFlame : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.1f;
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * -0.5f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            int goldFlameDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
            Dust expr_8976_cp_0 = Main.dust[goldFlameDust];
            expr_8976_cp_0.position.X -= 2f;
            Dust expr_8994_cp_0 = Main.dust[goldFlameDust];
            expr_8994_cp_0.position.Y += 2f;
            Main.dust[goldFlameDust].scale += (float)Main.rand.Next(50) * 0.01f;
            Main.dust[goldFlameDust].noGravity = true;
            Dust expr_89E7_cp_0 = Main.dust[goldFlameDust];
            expr_89E7_cp_0.velocity.Y -= 2f;
            if (Main.rand.NextBool())
            {
                int goldFlameDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
                Dust expr_8A4E_cp_0 = Main.dust[goldFlameDust2];
                expr_8A4E_cp_0.position.X -= 2f;
                Dust expr_8A6C_cp_0 = Main.dust[goldFlameDust2];
                expr_8A6C_cp_0.position.Y += 2f;
                Main.dust[goldFlameDust2].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[goldFlameDust2].noGravity = true;
                Main.dust[goldFlameDust2].velocity *= 0.1f;
            }
            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<AegisBlast2>(), (int)((double)hit.Damage * 0.75), hit.Knockback, Main.myPlayer);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
