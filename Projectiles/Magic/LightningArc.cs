using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class LightningArc : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Arc");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.timeLeft = 20;
            projectile.penetrate = 6;
            projectile.tileCollide = true;
        }


        HashSet<NPC> shockedbefore = new HashSet<NPC>();
        int prevX = 0;
        public override void AI()
        {
            //projectile.alpha exists
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 160f;
            bool target = false;
            NPC npc = null;
            bool pastNPC = false;
            if (projectile.timeLeft < 18)
            { 
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !shockedbefore.Contains(Main.npc[k]))
                    {
                        Vector2 newMove = Main.npc[k].Center - (projectile.velocity + projectile.Center);
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                            npc = Main.npc[k];

                        }
                    }
                }
            }
            //if not found, look through npcs that have been shocked before
            if (!target) {
                foreach (NPC pastnpc in shockedbefore) {
                    Vector2 newMove = pastnpc.Center -(projectile.velocity + projectile.Center);
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                        npc = pastnpc;
                        pastNPC = true;
                    }
                }  
            }


            // Main.dust[dust].velocity /= 2f;
            Vector2 current = projectile.Center;
            if (target)
            {
                shockedbefore.Add(npc);
                npc.HitEffect(0, projectile.damage);
                //AdjustMagnitude(ref move);
                //AdjustMagnitude(ref projectile.velocity);
                //projectile.velocity = (10 * projectile.velocity + move) / 11f;
                
                move+= new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10))*distance/30;
                if (pastNPC) {
                    prevX++;
                    move += new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)) * prevX;
                }
                

            }
            else {
                move = (projectile.velocity+ new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)))*5;
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(current, projectile.width, projectile.height, 20, 0, 0);
                Main.dust[dust].velocity = new Vector2(0);
                current += move / 20f;
            }
            projectile.position = current;
            /*int tx = (int)(projectile.position.X / 16f);
            int ty = (int)(projectile.position.Y / 16f);

            if (tx < 0)
            {
                tx = 0;
            }
            if (tx > Main.maxTilesX)
            {
                tx = Main.maxTilesX;
            }
            if (ty < 0)
            {
                ty = 0;
            }
            if (ty > Main.maxTilesY)
            {
                ty = Main.maxTilesY;
            }

            if (Main.tile[tx, ty] != null && Main.tileSolid[(int)Main.tile[tx,ty].type] && Main.tile[tx, ty].wall == 0)
            {
                projectile.timeLeft -= 12;
            }*/
             

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            projectile.timeLeft -= 12;
            return false;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
