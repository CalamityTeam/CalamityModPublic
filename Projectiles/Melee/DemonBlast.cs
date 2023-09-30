using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DemonBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 180;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                173,
                (int)CalamityDusts.Brimstone,
                172
            });

            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }

            bool tileCheck = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f)];
            dust.position = Projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;

            if (tileCheck)
                dust.noLight = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            bool tileCheck = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    (int)CalamityDusts.Brimstone,
                    172
                });
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    173,
                    (int)CalamityDusts.Brimstone,
                    172
                });
                int dustInt = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[dustInt].noGravity = true;
                Main.dust[dustInt].velocity *= 3f;
                if (tileCheck)
                {
                    Main.dust[dustInt].noLight = true;
                }
                dustInt = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
                Main.dust[dustInt].velocity *= 2f;
                Main.dust[dustInt].noGravity = true;
                if (tileCheck)
                {
                    Main.dust[dustInt].noLight = true;
                }
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 90);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
