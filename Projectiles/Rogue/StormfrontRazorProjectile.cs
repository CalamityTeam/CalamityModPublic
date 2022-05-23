using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class StormfrontRazorProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StormfrontRazor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Knife");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            DrawOriginOffsetX = 50;
            DrawOriginOffsetY = 20;
            Projectile.ai[0]++;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 1;
            }
            float sparkFreq = 125f / Projectile.ai[1];
            if (Projectile.ai[0] >= sparkFreq)
            {
                Vector2 sparkS = new Vector2(Main.rand.NextFloat(-14f, 14f), Main.rand.NextFloat(-14f, 14f));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkS, ModContent.ProjectileType<Stormfrontspark>(), Projectile.damage, 3f, Projectile.owner);
                Projectile.ai[0] = 0;
            }
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
