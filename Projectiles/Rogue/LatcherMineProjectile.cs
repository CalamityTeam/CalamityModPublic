using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LatcherMineProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Latcher Mine");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 27;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1.5f;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation += (projectile.velocity.X > 0) ? 0.3f : -0.3f;
                if (projectile.velocity.Y < 7f)
                {
                    projectile.velocity.Y += 0.24f;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
                projectile.tileCollide = false;
                int secondsToLive = (projectile.Calamity().stealthStrike ? 14 : 6);
                bool readyToKillSelf = false;
                //There was something along the lines of "var_2_2CB4E_cp_0" doing this exact iterative expression, but in 4 times as many lines and barely decipherable variables and copies of arrays.
                //*shudder*
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= (float)(60 * secondsToLive))
                {
                    readyToKillSelf = true;
                }
                else if ((int)projectile.ai[1] < 0 || (int)projectile.ai[1] >= 200)
                {
                    readyToKillSelf = true;
                }
                else if (Main.npc[(int)projectile.ai[1]].active && !Main.npc[(int)projectile.ai[1]].dontTakeDamage)
                {
                    projectile.Center = Main.npc[(int)projectile.ai[1]].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[(int)projectile.ai[1]].gfxOffY;
                    projectile.timeLeft = (int)MathHelper.Min(projectile.timeLeft, 120);
                }
                else
                {
                    readyToKillSelf = true;
                }
                if (readyToKillSelf)
                {
                    projectile.Kill();
                }
                if (projectile.timeLeft == 1)
                {
                    projectile.Kill();
                }
            }
            if (projectile.ai[0] == 2f)
            {
                projectile.velocity = Vector2.UnitY * 3f;
                projectile.rotation = 0f;
            }
            if (projectile.timeLeft == 110 * (projectile.Calamity().stealthStrike ? 2 : 1) ||
                projectile.timeLeft == 60 * (projectile.Calamity().stealthStrike ? 2 : 1) ||
                projectile.timeLeft == 24 * (projectile.Calamity().stealthStrike ? 2 : 1))
            {
                projectile.frame++;
            }
            if (projectile.timeLeft < 24 * (projectile.Calamity().stealthStrike ? 2 : 1))
            {
                projectile.frameCounter += 1;
                if (projectile.frameCounter % 2 == 1)
                {
                    projectile.frame += 1;
                    if (projectile.frame >= 4)
                    {
                        projectile.frame = 0;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.Calamity().stealthStrike)
            {
                projectile.height = 10;
                if (projectile.localAI[1] == 0f)
                {
                    projectile.timeLeft = 14 * 60;
                    projectile.localAI[1] = 1f;
                }
                projectile.ai[0] = 2f;
            }
            return !projectile.Calamity().stealthStrike;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == 22 && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == 54 && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool colliding;
                            if (Main.npc[i].type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                rect.X -= 8;
                                rect.Y -= 8;
                                rect.Width += 16;
                                rect.Height += 16;
                                colliding = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                colliding = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (colliding)
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
                                const int maxNumberOfMines = 6;
                                Point[] otherMinesArray = new Point[maxNumberOfMines];
                                int otherMinesIndex = 0;
                                for (int l = 0; l < Main.projectile.Length; l++)
                                {
                                    if (l != projectile.whoAmI && Main.projectile[l].active && 
                                        Main.projectile[l].owner == Main.myPlayer && 
                                        Main.projectile[l].type == projectile.type && 
                                        Main.projectile[l].ai[0] == 1f && 
                                        Main.projectile[l].ai[1] == (float)i)
                                    {
                                        otherMinesArray[otherMinesIndex++] = new Point(l, Main.projectile[l].timeLeft);
                                        if (otherMinesIndex >= otherMinesArray.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (otherMinesIndex >= otherMinesArray.Length)
                                {
                                    int idx = 0;
                                    for (int m = 1; m < otherMinesArray.Length; m++)
                                    {
                                        if (otherMinesArray[m].Y < otherMinesArray[idx].Y)
                                        {
                                            idx = m;
                                        }
                                    }
                                    Main.projectile[otherMinesArray[idx].X].Kill();
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
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = (projectile.Calamity().stealthStrike ? 240 : 100);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            for (int i = 0; i < 6; i++)
            {
                if (Main.rand.NextBool(2) && projectile.Calamity().stealthStrike)
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    Projectile.NewProjectile(projectile.Top, shrapnelVelocity,
                        ModContent.ProjectileType<BarrelShrapnel>(), projectile.damage, 3f, projectile.owner);
                }
                else
                {
                    Vector2 fireVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int fireIndex = Projectile.NewProjectile(projectile.Top, fireVelocity,
                        Main.rand.Next(ProjectileID.MolotovFire, ProjectileID.MolotovFire3 + 1),
                        projectile.damage, 1f, projectile.owner);
                    Main.projectile[fireIndex].Calamity().forceRogue = true;
                    Main.projectile[fireIndex].penetrate = -1;
                    Main.projectile[fireIndex].usesLocalNPCImmunity = true;
                    Main.projectile[fireIndex].localNPCHitCooldown = 8;
                }
            }
        }
    }
}
