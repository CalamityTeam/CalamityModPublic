using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SulphuricAcidCannon2 : ModProjectile
    {
        public float counter = 0f;
        public float counter2 = 0f;
        public int killCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 6)
            {
                projectile.frame = 0;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (counter >= 120f)
                {
                    counter = 0f;
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector15.X, vector15.Y, mod.ProjectileType("SulphuricAcidMist2"), (int)(250f * Main.player[projectile.owner].rangedDamage), 1f, projectile.owner, 0f, 0f);
                }
                else
                    counter += 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 6f)
                {
                    int num982 = 20;
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= num982;
                    }
                    if (projectile.alpha < 80)
                    {
                        projectile.alpha = 80;
                    }
                }
                if (projectile.ai[1] >= 45f)
                {
                    projectile.ai[1] = 45f;
                    if (counter2 < 1f)
                    {
                        counter2 += 0.002f;
                        projectile.scale += 0.002f;
                        projectile.width = (int)(30f * projectile.scale);
                        projectile.height = (int)(30f * projectile.scale);
                    }
                    else
                    {
                        projectile.width = 60;
                        projectile.height = 60;
                    }
                    if (projectile.wet)
                    {
                        if (projectile.velocity.Y > 0f)
                        {
                            projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                        }
                        if (projectile.velocity.Y > -1f)
                        {
                            projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                        }
                    }
                    else if (projectile.velocity.Y > -2f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                    }
                }
                killCounter++;
                if (killCounter >= 200)
                {
                    projectile.Kill();
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                bool flag54 = false;
                projectile.localAI[0] += 1f;
                int num989 = (int)projectile.ai[1];
                if (projectile.localAI[0] >= 360f)
                {
                    flag54 = true;
                }
                else if (num989 < 0 || num989 >= 200)
                {
                    flag54 = true;
                }
                else if (Main.npc[num989].active && !Main.npc[num989].dontTakeDamage)
                {
                    projectile.Center = Main.npc[num989].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[num989].gfxOffY;
                }
                else
                {
                    flag54 = true;
                }
                if (flag54)
                {
                    projectile.Kill();
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (((Main.npc[i].active && !Main.npc[i].dontTakeDamage)) &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == 22 && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == 54 && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool flag3;
                            if (Main.npc[i].type == 414)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (flag3)
                            {
                                if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
                                {
                                    Main.npc[i].ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                projectile.damage = 0;
                                int num28 = 3;
                                Point[] array2 = new Point[num28];
                                int num29 = 0;
                                for (int l = 0; l < 1000; l++)
                                {
                                    if (l != projectile.whoAmI && Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type && Main.projectile[l].ai[0] == 1f && Main.projectile[l].ai[1] == (float)i)
                                    {
                                        array2[num29++] = new Point(l, Main.projectile[l].timeLeft);
                                        if (num29 >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (num29 >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.buffImmune[BuffID.Venom] && target.aiStyle != 6)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 600);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = (projectile.height = 64);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            Main.PlaySound(SoundID.Item54, projectile.Center);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = projectile.alpha;
                num3 = num246;
            }
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }
    }
}
