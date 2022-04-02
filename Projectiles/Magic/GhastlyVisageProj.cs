using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyVisageProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Visage");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.65f, 0f, 0.1f);
            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.spriteDirection == -1)
            {
                num = MathHelper.Pi;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.ai[0] += 1f;
            int num39 = 0;
            if (projectile.ai[0] >= 240f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 480f)
            {
                num39++;
            }
            int num40 = 40;
            int num41 = 2;
            projectile.ai[1] -= 1f;
            bool flag15 = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = (float)(num40 - num41 * num39);
                flag15 = true;
                int arg_1EF4_0 = (int)projectile.ai[0] / (num40 - num41 * num39);
            }
            bool flag16 = player.channel && !player.noItems && !player.CCed;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            int manaCost = (int)(20f * player.manaCost);
            if (projectile.localAI[1] == 0f)
            {
                if (player.statMana < manaCost)
                {
                    if (player.manaFlower)
                    {
                        player.QuickMana();
                        if (player.statMana >= (int)(float)manaCost)
                        {
                            player.manaRegenDelay = (int)player.maxRegenDelay;
                            player.statMana -= manaCost;
                        }
                        else
                        {
                            projectile.Kill();
                        }
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
                else
                {
                    if (player.statMana >= (int)(float)manaCost)
                    {
                        player.statMana -= manaCost;
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                    }
                }
                projectile.localAI[1] += 1f;
                Main.PlaySound(SoundID.Item117, projectile.position);
            }
            if (projectile.soundDelay <= 0 && flag16)
            {
                if (player.statMana < manaCost)
                {
                    if (player.manaFlower)
                    {
                        player.QuickMana();
                        if (player.statMana >= (int)(float)manaCost)
                        {
                            player.manaRegenDelay = (int)player.maxRegenDelay;
                            player.statMana -= manaCost;
                        }
                        else
                        {
                            projectile.Kill();
                        }
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
                else
                {
                    if (player.statMana >= (int)(float)manaCost)
                    {
                        player.statMana -= manaCost;
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                    }
                }
                projectile.soundDelay = num40 - num41 * num39;
                if (projectile.ai[0] != 1f)
                {
                    Main.PlaySound(SoundID.Item117, projectile.position);
                }
                projectile.localAI[0] = 12f;
            }
            if (flag15 && Main.myPlayer == projectile.owner)
            {
                int num42 = ModContent.ProjectileType<GhastlyBlast>();
                float coreVelocity = 11.5f;
                int weaponDamage2 = player.GetWeaponDamage(player.ActiveItem());
                float weaponKnockback2 = player.ActiveItem().knockBack;
                if (flag16)
                {
                    weaponKnockback2 = player.GetWeaponKnockback(player.ActiveItem(), weaponKnockback2);
                    float scaleFactor12 = player.ActiveItem().shootSpeed * projectile.scale;
                    Vector2 vector19 = vector;
                    Vector2 value18 = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - vector19;
                    if (player.gravDir == -1f)
                    {
                        value18.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - vector19.Y;
                    }
                    Vector2 value19 = Vector2.Normalize(value18);
                    if (float.IsNaN(value19.X) || float.IsNaN(value19.Y))
                    {
                        value19 = -Vector2.UnitY;
                    }
                    value19 *= scaleFactor12;
                    if (value19.X != projectile.velocity.X || value19.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = value19 * 0.55f;
                    Vector2 vector20 = Vector2.Normalize(projectile.velocity) * coreVelocity;
                    if (float.IsNaN(vector20.X) || float.IsNaN(vector20.Y))
                    {
                        vector20 = -Vector2.UnitY;
                    }
                    Vector2 vector21 = vector19 + Utils.RandomVector2(Main.rand, -10f, 10f);
                    int num44 = Projectile.NewProjectile(vector21.X, vector21.Y, vector20.X, vector20.Y, num42, weaponDamage2, weaponKnockback2, projectile.owner, 0f, 0f);
                }
                else
                {
                    projectile.Kill();
                }
            }
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + num;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Vector2 origin = new Vector2(13f, 16f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Magic/GhastlyVisageProjGlow"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
        }

        public override bool CanDamage() => false;
    }
}
