using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles.Rogue
{
    public class SnapClamProj : ModProjectile
    {
        public int clamCounter = 0;
        public bool openClam = true;
        public bool onEnemy = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snap Clam");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 13;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (openClam && !onEnemy)
            {
                ++clamCounter;
                if (clamCounter >= 30)
                {
                    openClam = false;
                    projectile.damage = (int)(projectile.damage * 0.8);
                }
            }
            if (projectile.ai[0] == 0f)
            {
                float[] var0 = projectile.ai;
                int var1 = 1;
                float num73 = var0[var1];
                var0[var1] = num73 + 1f;
                projectile.ai[1] = 45f;
                projectile.velocity.X = projectile.velocity.X * 0.99f;
                projectile.velocity.Y = projectile.velocity.Y + 0.15f;
                projectile.rotation += 0.4f * (float)projectile.direction;
                projectile.spriteDirection = projectile.direction;
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                int num988 = 6;
                bool flag54 = false;
                bool flag55 = false;
                float[] var0 = projectile.localAI;
                int var1 = 0;
                float num73 = var0[var1];
                var0[var1] = num73 + 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    flag55 = true;
                }
                int num989 = (int)projectile.ai[1];
                if (projectile.localAI[0] >= (float)(60 * num988))
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
                    if (flag55)
                    {
                        Main.npc[num989].HitEffect(0, 1.0);
                    }
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
            if (openClam && !onEnemy)
            {
                projectile.frame = 1;
            }
            else
            {
                projectile.frame = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (openClam)
            {
                onEnemy = true;
                Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].active && !Main.npc[i].dontTakeDamage &&
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
                                    int num28 = 6;
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
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 13;
            projectile.height = 20;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 0, new Color(0, 255, 255), 1f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SnapClamDebuff>(), 240);
        }
    }
}
