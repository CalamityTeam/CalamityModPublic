using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BlackAnurianPlankton : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            float projX = Projectile.position.X;
            float projY = Projectile.position.Y;
            float homingRange = 100000f;
            bool isHoming = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && !Main.npc[i].wet)
                    {
                        float npcX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float npcY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < 800f && npcDist < homingRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                        {
                            homingRange = npcDist;
                            projX = npcX;
                            projY = npcY;
                            isHoming = true;
                        }
                    }
                }
            }
            if (!isHoming)
            {
                projX = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                projY = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }

            float projVelModifier = 0.1f;
            Vector2 projDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float xDest = projX - projDirection.X;
            float yDest = projY - projDirection.Y;
            float destinationDist = (float)Math.Sqrt((double)(xDest * xDest + yDest * yDest));
            destinationDist = 6f / destinationDist;
            xDest *= destinationDist;
            yDest *= destinationDist;
            if (Projectile.velocity.X < xDest)
            {
                Projectile.velocity.X = Projectile.velocity.X + projVelModifier;
                if (Projectile.velocity.X < 0f && xDest > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + projVelModifier * 2f;
                }
            }
            else if (Projectile.velocity.X > xDest)
            {
                Projectile.velocity.X = Projectile.velocity.X - projVelModifier;
                if (Projectile.velocity.X > 0f && xDest < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - projVelModifier * 2f;
                }
            }
            if (Projectile.velocity.Y < yDest)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + projVelModifier;
                if (Projectile.velocity.Y < 0f && yDest > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + projVelModifier * 2f;
                }
            }
            else if (Projectile.velocity.Y > yDest)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - projVelModifier;
                if (Projectile.velocity.Y > 0f && yDest < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - projVelModifier * 2f;
                }
            }
        }
    }
}
