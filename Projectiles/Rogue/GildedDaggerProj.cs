using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GildedDaggerProj : ModProjectile
    {
        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gilded Dagger");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(7))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.GoldCoin, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (hasHitEnemy)
            {
                projectile.rotation += projectile.direction * 0.4f;
            }
            else
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.PiOver2);
                //Rotating 90 degrees if shooting right
                if (projectile.spriteDirection == 1)
                {
                    projectile.rotation += MathHelper.ToRadians(90f);
                }
            }

            if (!projectile.Calamity().stealthStrike && projectile.timeLeft < 575)
            {
                projectile.velocity.Y += 0.5f;
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }
            }
            else if (projectile.Calamity().stealthStrike && hasHitEnemy)
            {
                if (targetNPC >= 0 && Main.npc[targetNPC].active)
                {
                    Vector2 newVelocity = Main.npc[targetNPC].Center - projectile.Center;
                    newVelocity.Normalize();
                    newVelocity *= 15f;
                    projectile.velocity = newVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float minDist = 999f;
            int index = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                bool hasHitNPC = false;
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == i)
                    {
                        hasHitNPC = true;
                    }
                }

                NPC npc = Main.npc[i];
                if (npc == target)
                {
                    previousNPCs.Add(i);
                }
                if (npc.CanBeChasedBy(projectile, false) && npc != target && !hasHitNPC)
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
                if (projectile.Calamity().stealthStrike)
                {
                    projectile.damage = (int)(projectile.damage * 1.05f);
                }
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                projectile.velocity = velocityNew;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int num621 = 0; num621 < 8; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 1f;
            }
        }
    }
}
