using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SerpentineTail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Serpentine");
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
            int num1051 = 10;
            Vector2 value68 = Vector2.Zero;
            if (Projectile.ai[1] == 1f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            int chase = Projectile.GetByUUID(Projectile.owner, (int)Projectile.ai[0]);
            float num1064;
            float scaleFactor17;
            float scaleFactor18;
            if (chase >= 0 && Main.projectile[chase].active)
            {
                value68 = Main.projectile[chase].Center;
                Vector2 arg_2DE6A_0 = Main.projectile[chase].velocity;
                num1064 = Main.projectile[chase].rotation;
                scaleFactor18 = MathHelper.Clamp(Main.projectile[chase].scale, 0f, 50f);
                scaleFactor17 = 16f;
                Main.projectile[chase].localAI[0] = Projectile.localAI[0] + 1f;
            }
            else
            {
                for (int k = 0; k < 8; k++)
                {
                    int num114 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 68, 0f, 0f, 100, default, 1.25f);
                    Dust dust = Main.dust[num114];
                    dust.velocity *= 0.3f;
                    Main.dust[num114].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                    Main.dust[num114].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
                    Main.dust[num114].noGravity = true;
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
            Vector2 vector134 = value68 - Projectile.Center;
            if (num1064 != Projectile.rotation)
            {
                float num1068 = MathHelper.WrapAngle(num1064 - Projectile.rotation);
                vector134 = vector134.RotatedBy((double)(num1068 * 0.1f), default);
            }
            Projectile.rotation = vector134.ToRotation() + 1.57079637f;
            Projectile.position = Projectile.Center;
            Projectile.scale = scaleFactor18;
            Projectile.width = Projectile.height = (int)((float)num1051 * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (vector134 != Vector2.Zero)
            {
                Projectile.Center = value68 - Vector2.Normalize(vector134) * scaleFactor17 * scaleFactor18;
            }
            Projectile.spriteDirection = (vector134.X > 0f) ? 1 : -1;
            return;
        }
    }
}
