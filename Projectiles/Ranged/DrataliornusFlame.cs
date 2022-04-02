using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrataliornusFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drataliornus Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.hide = true;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            if (projectile.hide) //called on first AI tick only - more initializations
            {
                projectile.hide = false;
                projectile.ai[1] = -1f;

                if (projectile.ai[0] != 0f) //if empowered fireball
                {
                    projectile.extraUpdates = 1;
                    projectile.localAI[0] = Main.rand.Next(30);

                    if (projectile.ai[0] == 2f) //if homing fireball
                        projectile.timeLeft += 180;
                }

                projectile.netUpdate = true;
            }

            //intangible until it's in completely open space
            if (!projectile.tileCollide && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.tileCollide = true;
                projectile.netUpdate = true;
            }

            projectile.localAI[0]++;
            if (projectile.localAI[0] > 60f) //dragon dust trail counter, but only empowered proj spawns it
            {
                projectile.localAI[0] = 0f;

                if (projectile.ai[0] != 0f && projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), projectile.damage / 3, projectile.knockBack * 3f, projectile.owner);
            }

            projectile.localAI[1]++;
            if (projectile.localAI[1] > 12f) //homing counter, checks every 12/2=6 ticks
            {
                projectile.localAI[1] = 0f;

                if (projectile.ai[0] == 2f && projectile.ai[1] < 0f) //if homing fireball and no target
                {
                    int possibleTarget = -1;
                    float closestDistance = 700f;

                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.chaseable && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly &&
                            !npc.immortal && Collision.CanHit(projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float distance = Vector2.Distance(projectile.Center, npc.Center);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                possibleTarget = i;
                            }
                        }
                    }

                    projectile.ai[1] = possibleTarget;
                    projectile.netUpdate = true;
                }
            }

            if (projectile.ai[1] != -1f) //if has target
            {
                NPC npc = Main.npc[(int)projectile.ai[1]];

                if (npc.active && npc.chaseable && !npc.dontTakeDamage) //do homing
                {
                    Vector2 distance = npc.Center - projectile.Center;
                    double angle = distance.ToRotation() - projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (Math.Abs(angle) > Math.PI * 0.75)
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(angle * 0.07);
                    }
                    else
                    {
                        float range = distance.Length();
                        float difference = 12.7f / range;
                        distance *= difference;
                        distance /= 7f;
                        projectile.velocity += distance;
                        if (range > 70f)
                        {
                            projectile.velocity *= 0.98f;
                        }
                    }
                }
                else //target not valid, stop homing
                {
                    projectile.ai[1] = -1;
                    projectile.netUpdate = true;
                }
            }

            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f + Main.rand.NextFloat());
            Main.dust[d].noGravity = true;

            Lighting.AddLight(projectile.Center, 255f / 255f, 154f / 255f, 58f / 255f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 154, 58, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture2D13.Width, texture2D13.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)texture2D13.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft != 0)
            {
                Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);

                if (projectile.ai[0] != 0f && projectile.owner == Main.myPlayer) //if empowered, make exo arrow and dragon dust
                {
                    Vector2 vector3 = projectile.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = projectile.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), projectile.damage / 2, projectile.knockBack, projectile.owner);

                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), projectile.damage / 3, projectile.knockBack * 2f, projectile.owner);
                }

                projectile.position = projectile.Center;
                projectile.width = 180;
                projectile.height = 180;
                projectile.position.X = projectile.position.X - 90;
                projectile.position.Y = projectile.position.Y - 90;

                //just dusts
                const int num226 = 24;
                float modifier = 4f + 8f * Main.rand.NextFloat();
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * modifier;
                    vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 45, default, 2f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                for (int num193 = 0; num193 < 4; num193++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 50, default, 1.5f);

                    int num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 50, default, 1f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 2f;
                }
                for (int num194 = 0; num194 < 12; num194++)
                {
                    int num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 0, default, 3f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 3f;
                }

                projectile.timeLeft = 0; //should avoid infinite loop if a hit npc calls proj.Kill()
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.damage /= 3;
                projectile.Damage();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

            if (projectile.ai[0] != 0f && projectile.owner == Main.myPlayer) //if empowered
            {
                if (projectile.timeLeft != 0) //will not be called on npcs hit by explosion (only direct hits)
                {
                    //make exo arrow, make meteor
                    Vector2 vector3 = target.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = target.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), projectile.damage / 2, projectile.knockBack, projectile.owner);

                    Vector2 vel = new Vector2(Main.rand.Next(-400, 401), Main.rand.Next(500, 801));
                    Vector2 pos = target.Center - vel;
                    vel.X += Main.rand.Next(-100, 101);
                    vel.Normalize();
                    vel *= 30f;
                    Projectile.NewProjectile(pos, vel + target.velocity, ModContent.ProjectileType<SkyFlareFriendly>(), (int)(projectile.damage * 1.5f), projectile.knockBack * 5f, projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;

            target.AddBuff(BuffID.Daybreak, 240);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

            if (projectile.ai[0] != 0f && projectile.owner == Main.myPlayer) //if empowered
            {
                if (projectile.timeLeft != 0) //will not be called on npcs hit by explosion (only direct hits)
                {
                    //make exo arrow, make meteor
                    Vector2 vector3 = target.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = target.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), projectile.damage / 2, projectile.knockBack, projectile.owner);

                    Vector2 vel = new Vector2(Main.rand.Next(-400, 401), Main.rand.Next(500, 801));
                    Vector2 pos = target.Center - vel;
                    vel.X += Main.rand.Next(-100, 101);
                    vel.Normalize();
                    vel *= 30f;
                    Projectile.NewProjectile(pos, vel + target.velocity, ModContent.ProjectileType<SkyFlareFriendly>(), projectile.damage * 3, projectile.knockBack * 5f, projectile.owner);
                }
            }
        }
    }
}
