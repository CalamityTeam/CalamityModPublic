using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaBoom : ModProjectile
    {
        public int frameX = 0;
        public int frameY = 0;
        private const int horizontalFrames = 5;
        private const int verticalFrames = 4;
        private const int frameLength = 4;
        private const float radius = 204.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 408;
            projectile.height = 410;
            projectile.scale = 2f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = frameLength * horizontalFrames * verticalFrames / 5;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % frameLength == frameLength - 1)
            {
                frameY++;
                if (frameY >= verticalFrames)
                {
                    frameX++;
                    frameY = 0;
                }
                if (frameX >= horizontalFrames)
                {
                    projectile.Kill();
                }
            }

            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);

            /*float dustSpeed = (float)Main.rand.Next(12, 36);
            Vector2 dustVel = CalamityUtils.RandomVelocity(40f, dustSpeed, dustSpeed, 1f);
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                107,
                234,
                269
            });
            int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[rainbow];
            dust.noGravity = true;
            dust.position = projectile.Center;
            dust.position.X += (float)Main.rand.Next(-10, 11);
            dust.position.Y += (float)Main.rand.Next(-10, 11);
            dust.velocity = dustVel;*/
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int length = texture.Width / horizontalFrames;
            int height = texture.Height / verticalFrames;
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            Rectangle frame = new Rectangle(frameX * length, frameY * height, length, height);
            Vector2 origin = new Vector2(length / 2f, height / 2f);
            spriteBatch.Draw(texture, drawPos, frame, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }
    }
}
