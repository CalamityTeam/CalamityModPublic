
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NychthemeronOrb : ModProjectile
    {
        public static float[] playerDists = new float[10] { 48f, 80f, 80f, 80f, 80f, 112f, 112f, 112f, 112f, 112f };
        public static float[] rotationSpeeds = new float[10] { 0.1f, -0.075f, -0.075f, -0.075f, -0.075f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f };

        private Vector2 velocity = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nychthemeron Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool isActive = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == projectile.ai[1] && p.active)
                {
                    isActive = true;
                }
            }
            if (!isActive)
            {
                projectile.localAI[0] = 1f;
                projectile.penetrate = 1;
            }

            Player player = Main.player[projectile.owner];

            if (projectile.localAI[0] == 0f)
            {
                // Orbit
                projectile.rotation += rotationSpeeds[(int)projectile.localAI[1]];

                Vector2 relativePos = new Vector2(0, -1);
                relativePos *= playerDists[(int)projectile.localAI[1]];
                relativePos = relativePos.RotatedBy(projectile.rotation);
                projectile.Center = player.Center + relativePos;
                velocity = projectile.position - projectile.oldPosition;
            }
            else if (projectile.localAI[0] == 1f)
            {
                if (projectile.timeLeft > 240 && projectile.Calamity().lineColor == 1)
                    projectile.timeLeft = 240;
                // Follow Enemy
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    projectile.velocity += velocityNew;
                    if (projectile.velocity.Length() > 10f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 10f;
                    }
                    velocity = projectile.velocity;
                }
                else
                {
                    projectile.velocity = velocity;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texLight = Main.projectileTexture[projectile.type];
            Texture2D texDark = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/NychthemeronOrb2");
            if (projectile.ai[0] == 0f)
            {
                spriteBatch.Draw(texLight, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, texLight.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            else if (projectile.ai[0] == 1f)
            {
                spriteBatch.Draw(texDark, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, texDark.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
