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
            Projectile.width = 140;
            Projectile.height = 290;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Calamity().rogue = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 11;
        }

        public override bool PreAI()
        {
            if (stealthyNuke && boomerTime > -1)
            {
                if (boomerTime == 0)
                {
                    Projectile.timeLeft = boomerTime == 0 ? 99 : Projectile.timeLeft;
                    Projectile.position = Projectile.Center;
                    Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                    Projectile.idStaticNPCHitCooldown = 9;

                    boomerTime = 1;
                }


                if (Projectile.timeLeft <= 15)
                    dustloop--;

                Projectile.Damage();
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
                    int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                    Main.dust[idx].position.X = Projectile.Center.X + Main.rand.NextFloat(-10f, 10f);
                    Main.dust[idx].position.Y = Projectile.Center.Y + Main.rand.NextFloat(-10f, 10f);
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
                if (Projectile.Calamity().stealthStrike)
                {
                    stealthyNuke = true;
                }

                //2-6
                Projectile.frameCounter += 1;
                if (Projectile.frameCounter % 6 == 0)
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
                            Projectile.Kill();
                        else
                        {
                            boomerTime = 0;
                            Projectile.hide = true;
                        }
                    }
                }
                if (Projectile.localAI[0] == 0f)
                {
                    Projectile.position.Y -= Projectile.height / 2; //position adjustments
                    Projectile.localAI[0] = 1f;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frame = new Rectangle(frameX * Projectile.width, frameY * Projectile.height, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SulphuricNukesplosion").Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //custom collision when it's explosion, yes i didn't want to make yet another projectile for the stealth explosion :hdfailure:
        {
            if (boomerTime == -1)
                projHitbox.Intersects(targetHitbox);

            return CalamityUtils.CircularHitboxCollision(Projectile.Center, 200f, targetHitbox);
        }
    }
}
