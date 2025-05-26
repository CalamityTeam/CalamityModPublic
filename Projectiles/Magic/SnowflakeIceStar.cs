using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SnowflakeIceStar : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/KelvinCatalystStar";
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
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
                CalamityUtils.HomeInOnNPC(Projectile, true, 500f, 12f, 20f);
            }

            Lighting.AddLight(Projectile.Center, Main.DiscoR * 0.075f / 255f, Main.DiscoR * 0.1f / 255f, Main.DiscoR * 0.125f / 255f);

            if (Main.rand.NextBool(6))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod, 0f, 0f, 100, default, 0.4f);
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
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            Projectile.ExpandHitboxBy(24);
            int dustAmt = 36;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int icyDust = Dust.NewDust(rotate + faceDirection, 0, 0, DustID.IceRod, faceDirection.X * 0.5f, faceDirection.Y * 0.5f, 100, default, 0.75f);
                Main.dust[icyDust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn2, 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Frostburn2, 60);
    }
}
