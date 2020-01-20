using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class JawsProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                if (projectile.Calamity().stealthStrike)
                {
                    int dustToUse = Main.rand.Next(0, 4);
                    int dustType = 0;
                    switch (dustToUse)
                    {
                        case 0:
                            dustType = 33;
                            break;
                        case 1:
                            dustType = 101;
                            break;
                        case 2:
                            dustType = 111;
                            break;
                        case 3:
                            dustType = 180;
                            break;
                    }

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                int num988 = 15;
                bool flag54 = false;
                bool flag55 = false;
                projectile.localAI[0] += 1f;
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
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 240);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * (player.allDamage + player.Calamity().throwingDamage - 1f)), 10f, projectile.owner, 0, 0);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 240);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * (player.allDamage + player.Calamity().throwingDamage - 1f)), 10f, projectile.owner, 0, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 3);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustToUse = Main.rand.Next(0, 4);
                int dustType = 0;
                switch (dustToUse)
                {
                    case 0:
                        dustType = 33;
                        break;
                    case 1:
                        dustType = 101;
                        break;
                    case 2:
                        dustType = 111;
                        break;
                    case 3:
                        dustType = 180;
                        break;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
