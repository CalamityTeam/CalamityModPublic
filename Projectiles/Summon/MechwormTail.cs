using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormTail : ModProjectile
    {
        private int playerMinionSlots = 0;
        private bool runCheck = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.netImportant = true;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), 0.15f, 0.01f, 0.15f);
            Player player9 = Main.player[projectile.owner];
            if (player9.maxMinions > playerMinionSlots)
            {
                playerMinionSlots = player9.maxMinions;
            }
            if (runCheck)
            {
                runCheck = false;
                playerMinionSlots = player9.maxMinions;
            }
            CalamityPlayer modPlayer = player9.Calamity();
            if ((int)Main.time % 120 == 0)
            {
                projectile.netUpdate = true;
            }
            int byUUID = Projectile.GetByUUID(projectile.owner, (int)projectile.ai[0]);
            if (!player9.active || player9.maxMinions < playerMinionSlots)
            {
                int lostSlots = playerMinionSlots - player9.maxMinions;
                while (lostSlots > 0)
                {
                    Projectile ahead = Main.projectile[byUUID];
                    // Each body slot is actually 0.5 slots. Kill two segments to lose 1 "true" slot.
                    for (int i = 0; i < 2; i++)
                    {
                        if (ahead.type != ModContent.ProjectileType<MechwormHead>())
                        {
                            projectile.localAI[1] = ahead.localAI[1];
                        }
                        projectile.ai[0] = ahead.ai[0];
                        projectile.ai[1] = 1f;
                        projectile.netUpdate = true;
                        ahead.Kill();
                        byUUID = Projectile.GetByUUID(projectile.owner, (int)projectile.ai[0]);
                        ahead = Main.projectile[byUUID];
                    }
                    lostSlots--;
                }
                playerMinionSlots = player9.maxMinions;
            }
            int num1051 = 10;
            if (player9.dead)
            {
                modPlayer.mWorm = false;
            }
            if (modPlayer.mWorm)
            {
                projectile.timeLeft = 2;
            }
            Vector2 value68 = Vector2.Zero;
            if (projectile.ai[1] == 1f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            float num1064;
            float scaleFactor17;
            float scaleFactor18;
            if (byUUID >= 0 && Main.projectile[byUUID].active && (Main.projectile[byUUID].type == ModContent.ProjectileType<MechwormHead>() ||
                                                                                                  Main.projectile[byUUID].type == ModContent.ProjectileType<MechwormBody>() ||
                                                                                                  Main.projectile[byUUID].type == ModContent.ProjectileType<MechwormBody2>()))
            {
                value68 = Main.projectile[byUUID].Center;
                num1064 = Main.projectile[byUUID].rotation;
                float num1063 = MathHelper.Clamp(Main.projectile[byUUID].scale, 0f, 50f);

                // Calculations have a safeguard against division by 0.
                float speed = 0f;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<MechwormHead>())
                    {
                        speed = Main.projectile[i].velocity.Length();
                        break;
                    }
                }
                scaleFactor17 = MechwormHead.ComputeDistance(16f, speed);
                scaleFactor18 = num1063;
                Main.projectile[byUUID].localAI[0] = projectile.localAI[0] + 1f;
                if (Main.projectile[byUUID].type != ModContent.ProjectileType<MechwormHead>())
                {
                    Main.projectile[byUUID].localAI[1] = (float)projectile.whoAmI;
                }
                if (projectile.owner == Main.myPlayer && Main.projectile[byUUID].type == ModContent.ProjectileType<MechwormHead>() && projectile.type == ModContent.ProjectileType<MechwormTail>())
                {
                    Main.projectile[byUUID].Kill();
                    projectile.Kill();
                    return;
                }
            }
            else
            {
                projectile.Kill();
                return;
            }
            projectile.alpha -= 42;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            projectile.velocity = Vector2.Zero;
            Vector2 vector134 = value68 - projectile.Center;
            if (num1064 != projectile.rotation)
            {
                float num1068 = MathHelper.WrapAngle(num1064 - projectile.rotation);
                vector134 = vector134.RotatedBy((double)(num1068 * 0.1f), default);
            }
            projectile.rotation = vector134.ToRotation() + 1.57079637f;
            projectile.position = projectile.Center;
            projectile.netSpam = 5;
            projectile.scale = scaleFactor18;
            projectile.width = projectile.height = (int)((float)num1051 * projectile.scale);
            projectile.Center = projectile.position;
            projectile.netSpam = 5;
            if (vector134 != Vector2.Zero)
            {
                projectile.Center = value68 - Vector2.Normalize(vector134) * projectile.Size.Length() * scaleFactor18 * 0.2f;
            }
            projectile.spriteDirection = (vector134.X > 0f) ? 1 : -1;
            projectile.netSpam = 5;
            return;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
