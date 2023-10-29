using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class SHPB : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public int explosionTimer = 120;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float lights = (float)Main.rand.Next(90, 111) * 0.01f;
            lights *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 1f * lights, 0.2f * lights, 0.75f * lights);
            Projectile.alpha -= 2;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            Projectile.ai[0] = (float)Main.rand.Next(-100, 101) * 0.0025f;
            Projectile.ai[1] = (float)Main.rand.Next(-100, 101) * 0.0025f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.05f;
                if ((double)Projectile.scale > 1.2)
                {
                    Projectile.localAI[0] = 1f;
                }
            }
            else
            {
                Projectile.scale -= 0.05f;
                if ((double)Projectile.scale < 0.8)
                {
                    Projectile.localAI[0] = 0f;
                }
            }
            Projectile.velocity.X *= 0.985f;
            Projectile.velocity.Y *= 0.985f;
            float projX = Projectile.Center.X;
            float projY = Projectile.Center.Y;
            float explodeRange = 250f;
            bool canExplode = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float npcX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float npcY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                    if (npcDist < explodeRange)
                    {
                        explodeRange = npcDist;
                        canExplode = true;
                    }
                }
            }
            if (canExplode)
            {
                explosionTimer--;
                if (explosionTimer <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 155, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item105, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<SHPExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
        }
    }
}
