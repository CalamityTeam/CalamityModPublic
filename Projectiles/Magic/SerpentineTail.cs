using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SerpentineTail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            Vector2 zeroing = Vector2.Zero;
            if (Projectile.ai[1] == 1f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            int chase = Projectile.GetByUUID(Projectile.owner, (int)Projectile.ai[0]);
            float projRotation;
            float sixteenScale;
            float projScale;
            if (chase >= 0 && Main.projectile[chase].active)
            {
                zeroing = Main.projectile[chase].Center;
                Vector2 arg_2DE6A_0 = Main.projectile[chase].velocity;
                projRotation = Main.projectile[chase].rotation;
                projScale = MathHelper.Clamp(Main.projectile[chase].scale, 0f, 50f);
                sixteenScale = 16f;
                Main.projectile[chase].localAI[0] = Projectile.localAI[0] + 1f;
            }
            else
            {
                for (int k = 0; k < 8; k++)
                {
                    int seaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 68, 0f, 0f, 100, default, 1.25f);
                    Dust dust = Main.dust[seaDust];
                    dust.velocity *= 0.3f;
                    Main.dust[seaDust].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                    Main.dust[seaDust].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
                    Main.dust[seaDust].noGravity = true;
                }
                Projectile.active = false;
                Projectile.Kill();
                return;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 40;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.velocity = Vector2.Zero;
            Vector2 rotateInLine = zeroing - Projectile.Center;
            if (projRotation != Projectile.rotation)
            {
                float angleWrap = MathHelper.WrapAngle(projRotation - Projectile.rotation);
                rotateInLine = rotateInLine.RotatedBy((double)(angleWrap * 0.1f), default);
            }
            Projectile.rotation = rotateInLine.ToRotation() + 1.57079637f;
            Projectile.position = Projectile.Center;
            Projectile.scale = projScale;
            Projectile.width = Projectile.height = (int)(10f * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (rotateInLine != Vector2.Zero)
            {
                Projectile.Center = zeroing - Vector2.Normalize(rotateInLine) * sixteenScale * projScale;
            }
            Projectile.spriteDirection = (rotateInLine.X > 0f) ? 1 : -1;
            return;
        }
    }
}
