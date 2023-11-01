using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class KelvinCatalystStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 90f)
            {
                Projectile.ai[0] += 1f;
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y *= 0.98f;
            }
            else
            {
                Projectile.extraUpdates = 1;

                Vector2 center = Projectile.Center;
                float maxDistance = 500f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 12f) / (21f);
                }
                else
                    Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, Main.DiscoR * 0.075f / 255f, Main.DiscoR * 0.1f / 255f, Main.DiscoR * 0.125f / 255f);

            if (Main.rand.NextBool(6))
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 100, default, 0.4f);
                Main.dust[dust].velocity *= 0.3f;
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation += 0.25f;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 90f ? null : false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            Projectile.ExpandHitboxBy(24);
            int dustAmt = 36;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int icyDust = Dust.NewDust(rotate + faceDirection, 0, 0, 67, faceDirection.X * 0.5f, faceDirection.Y * 0.5f, 100, default, 0.75f);
                Main.dust[icyDust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn2, 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Frostburn2, 60);
    }
}
