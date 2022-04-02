using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SubductionFlameburst : ModProjectile
    {
        public int frameX = 0;
        public int frameY = 0;
        public int currentFrame => frameY + frameX * 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Pillar");
        }

        public override void SetDefaults()
        {
            projectile.width = 81;
            projectile.height = 322;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //2-6
            projectile.frameCounter += 1;
            if (projectile.frameCounter % 7 == 6)
            {
                frameY += 1;
                if (frameY >= 4)
                {
                    frameX += 1;
                    frameY = 0;
                }
                if (frameX >= 3)
                {
                    projectile.Kill();
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.position.Y -= projectile.height / 2; //position adjustments
                projectile.localAI[0] = 1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle frame = new Rectangle(frameX * projectile.width, frameY * projectile.height, projectile.width, projectile.height);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
            target.AddBuff(BuffID.Daybreak, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
