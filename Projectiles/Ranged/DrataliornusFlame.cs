using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.hide = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.hide) //called on first AI tick only - more initializations
            {
                Projectile.hide = false;
                Projectile.ai[1] = -1f;

                if (Projectile.ai[0] != 0f) //if empowered fireball
                {
                    Projectile.extraUpdates = 1;
                    Projectile.localAI[0] = Main.rand.Next(30);

                    if (Projectile.ai[0] == 2f) //if homing fireball
                        Projectile.timeLeft += 180;
                }

                Projectile.netUpdate = true;
            }

            //intangible until it's in completely open space
            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60f) //dragon dust trail counter, but only empowered proj spawns it
            {
                Projectile.localAI[0] = 0f;

                if (Projectile.ai[0] != 0f && Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), Projectile.damage / 3, Projectile.knockBack * 3f, Projectile.owner);
            }

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] > 12f) //homing counter, checks every 12/2=6 ticks
            {
                Projectile.localAI[1] = 0f;

                if (Projectile.ai[0] == 2f && Projectile.ai[1] < 0f) //if homing fireball and no target
                {
                    int possibleTarget = -1;
                    float closestDistance = 700f;

                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.chaseable && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly &&
                            !npc.immortal && Collision.CanHit(Projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float distance = Vector2.Distance(Projectile.Center, npc.Center);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                possibleTarget = i;
                            }
                        }
                    }

                    Projectile.ai[1] = possibleTarget;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[1] != -1f) //if has target
            {
                NPC npc = Main.npc[(int)Projectile.ai[1]];

                if (npc.active && npc.chaseable && !npc.dontTakeDamage) //do homing
                {
                    Vector2 distance = npc.Center - Projectile.Center;
                    double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (Math.Abs(angle) > Math.PI * 0.75)
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.07);
                    }
                    else
                    {
                        float range = distance.Length();
                        float difference = 12.7f / range;
                        distance *= difference;
                        distance /= 7f;
                        Projectile.velocity += distance;
                        if (range > 70f)
                        {
                            Projectile.velocity *= 0.98f;
                        }
                    }
                }
                else //target not valid, stop homing
                {
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                }
            }

            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f + Main.rand.NextFloat());
            Main.dust[d].noGravity = true;

            Lighting.AddLight(Projectile.Center, 255f / 255f, 154f / 255f, 58f / 255f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 154, 58, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture2D13.Width, texture2D13.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)texture2D13.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft != 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, (int)Projectile.position.X, (int)Projectile.position.Y);

                if (Projectile.ai[0] != 0f && Projectile.owner == Main.myPlayer) //if empowered, make exo arrow and dragon dust
                {
                    Vector2 vector3 = Projectile.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = Projectile.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), Projectile.damage / 3, Projectile.knockBack * 2f, Projectile.owner);
                }

                Projectile.position = Projectile.Center;
                Projectile.width = 180;
                Projectile.height = 180;
                Projectile.position.X = Projectile.position.X - 90;
                Projectile.position.Y = Projectile.position.Y - 90;

                //just dusts
                const int num226 = 24;
                float modifier = 4f + 8f * Main.rand.NextFloat();
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * modifier;
                    vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 45, default, 2f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                for (int num193 = 0; num193 < 4; num193++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 174, 0f, 0f, 50, default, 1.5f);

                    int num195 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 174, 0f, 0f, 50, default, 1f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 2f;
                }
                for (int num194 = 0; num194 < 12; num194++)
                {
                    int num195 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 0, default, 3f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 3f;
                }

                Projectile.timeLeft = 0; //should avoid infinite loop if a hit npc calls proj.Kill()
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.damage /= 3;
                Projectile.Damage();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

            if (Projectile.ai[0] != 0f && Projectile.owner == Main.myPlayer) //if empowered
            {
                if (Projectile.timeLeft != 0) //will not be called on npcs hit by explosion (only direct hits)
                {
                    //make exo arrow, make meteor
                    Vector2 vector3 = target.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = target.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                    Vector2 vel = new Vector2(Main.rand.Next(-400, 401), Main.rand.Next(500, 801));
                    Vector2 pos = target.Center - vel;
                    vel.X += Main.rand.Next(-100, 101);
                    vel.Normalize();
                    vel *= 30f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), pos, vel + target.velocity, ModContent.ProjectileType<SkyFlareFriendly>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack * 5f, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 0;

            target.AddBuff(BuffID.Daybreak, 240);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

            if (Projectile.ai[0] != 0f && Projectile.owner == Main.myPlayer) //if empowered
            {
                if (Projectile.timeLeft != 0) //will not be called on npcs hit by explosion (only direct hits)
                {
                    //make exo arrow, make meteor
                    Vector2 vector3 = target.Center + new Vector2(600, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(360)));
                    Vector2 speed = target.Center - vector3;
                    speed /= 30f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), vector3.X, vector3.Y, speed.X, speed.Y, ModContent.ProjectileType<DrataliornusExoArrow>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                    Vector2 vel = new Vector2(Main.rand.Next(-400, 401), Main.rand.Next(500, 801));
                    Vector2 pos = target.Center - vel;
                    vel.X += Main.rand.Next(-100, 101);
                    vel.Normalize();
                    vel *= 30f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), pos, vel + target.velocity, ModContent.ProjectileType<SkyFlareFriendly>(), Projectile.damage * 3, Projectile.knockBack * 5f, Projectile.owner);
                }
            }
        }
    }
}
