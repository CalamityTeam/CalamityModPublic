using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Rogue
{
    public class SulphuricNukesplosion : ModProjectile
    {
        public int frameX = 0;
        public int frameY = 0;
        public int currentFrame => frameY + frameX * 14;

        private bool stealthyNuke = false;
        private int boomerTime = -1;

        private int dustloop = 30;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
        }

        public override void SetDefaults()
        {
            projectile.width = 140;
            projectile.height = 290;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 11;
        }

        public override bool PreAI()
        {
            if (stealthyNuke && boomerTime > -1)
            {
                if (boomerTime == 0)
                {
                    projectile.timeLeft = boomerTime == 0 ? 99 : projectile.timeLeft;
                    projectile.position = projectile.Center;
                    projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                    projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                    projectile.idStaticNPCHitCooldown = 9;

                    boomerTime = 1;
                }


                if (projectile.timeLeft <= 15)
                    dustloop--;

                projectile.Damage();
            }

            if (stealthyNuke && frameX >= 1)
            {
                if (boomerTime == -1)
                    dustloop += Main.rand.Next(1, 3);
                for (int i = 0; i < dustloop; ++i)
                {
                    int dustType = (int)CalamityDusts.SulfurousSeaAcid;
                    float scale = Main.rand.NextFloat(0.5f, 1.5f);
                    float randX = Main.rand.NextFloat(-30f, 30f);
                    float randY = Main.rand.NextFloat(-30f, 30f);
                    float randVelocity = Main.rand.NextFloat(5f, 24f);
                    float speed = (float)Math.Sqrt((double)(randX * randX + randY * randY));
                    speed = randVelocity / speed;
                    randX *= speed;
                    randY *= speed;
                    int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                    Main.dust[idx].position.X = projectile.Center.X + Main.rand.NextFloat(-10f, 10f);
                    Main.dust[idx].position.Y = projectile.Center.Y + Main.rand.NextFloat(-10f, 10f);
                    Main.dust[idx].velocity.X = randX;
                    Main.dust[idx].velocity.Y = randY;
                    Main.dust[idx].scale = scale;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].color = new Color(49, 180, 142);
                }
            }


            return true; //returns false to stop drawing the projectile
        }

        public override void AI()
        {
            if (boomerTime == -1)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    stealthyNuke = true;
                }

                //2-6
                projectile.frameCounter += 1;
                if (projectile.frameCounter % 6 == 0)
                {
                    frameY += 1;
                    if (frameY >= 7)
                    {
                        frameX += 1;
                        frameY = 0;
                    }
                    if (frameX >= 2)
                    {
                        if (!stealthyNuke)
                            projectile.Kill();
                        else
                        {
                            boomerTime = 0;
                            projectile.hide = true;
                        }
                    }
                }
                if (projectile.localAI[0] == 0f)
                {
                    projectile.position.Y -= projectile.height / 2; //position adjustments
                    projectile.localAI[0] = 1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 10 * (stealthyNuke ? 60 : 30)); //5 sec if not stealthstrike, otherwise 10;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 10 * (stealthyNuke ? 60 : 30)); //5 sec if not stealthstrike, otherwise 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(frameX * projectile.width, frameY * projectile.height, projectile.width, projectile.height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SulphuricNukesplosion"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //custom collision when it's explosion, yes i didn't want to make yet another projectile for the stealth explosion :hdfailure:
        {
            if (boomerTime == -1)
                projHitbox.Intersects(targetHitbox); 

            return CalamityUtils.CircularHitboxCollision(projectile.Center, 200f, targetHitbox);
        }
    }
}
