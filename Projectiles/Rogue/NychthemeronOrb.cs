
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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.Calamity().rogue = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool isActive = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == Projectile.ai[1] && p.active)
                {
                    isActive = true;
                }
            }
            if (!isActive)
            {
                Projectile.localAI[0] = 1f;
                Projectile.penetrate = 1;
            }

            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0f)
            {
                // Orbit
                Projectile.rotation += rotationSpeeds[(int)Projectile.localAI[1]];

                Vector2 relativePos = new Vector2(0, -1);
                relativePos *= playerDists[(int)Projectile.localAI[1]];
                relativePos = relativePos.RotatedBy(Projectile.rotation);
                Projectile.Center = player.Center + relativePos;
                velocity = Projectile.position - Projectile.oldPosition;
            }
            else if (Projectile.localAI[0] == 1f)
            {
                if (Projectile.timeLeft > 240 && Projectile.Calamity().lineColor == 1)
                    Projectile.timeLeft = 240;
                // Follow Enemy
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
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
                    velocityNew = Main.npc[index].Center - Projectile.Center;
                    velocityNew.Normalize();
                    Projectile.velocity += velocityNew;
                    if (Projectile.velocity.Length() > 10f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10f;
                    }
                    velocity = Projectile.velocity;
                }
                else
                {
                    Projectile.velocity = velocity;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texLight = Main.projectileTexture[Projectile.type];
            Texture2D texDark = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/NychthemeronOrb2");
            if (Projectile.ai[0] == 0f)
            {
                spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texLight.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }
            else if (Projectile.ai[0] == 1f)
            {
                spriteBatch.Draw(texDark, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texDark.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
