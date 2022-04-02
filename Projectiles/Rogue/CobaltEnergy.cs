using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CobaltEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cobalt Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int index = 0; index < 2; ++index)
            {
                int ruby = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 88, projectile.velocity.X, projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            if (!hasHitEnemy && projectile.timeLeft < 575)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
            }
            else if (hasHitEnemy)
            {
                if (targetNPC >= 0)
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
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int ruby = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 88, projectile.oldVelocity.X, projectile.oldVelocity.Y, 50, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.scale *= 1.25f;
                dust.velocity *= 0.5f;
            }
        }
    }
}
