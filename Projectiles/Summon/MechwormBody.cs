using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.minionSlots = 0.5f;
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
            CalamityPlayer modPlayer = player9.Calamity();
            if ((int)Main.time % 120 == 0)
            {
                projectile.netUpdate = true;
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
            int chase = Projectile.GetByUUID(projectile.owner, (int)projectile.ai[0]);
            float num1064;
            float scaleFactor17;
            float scaleFactor18;
            if (chase >= 0 && Main.projectile[chase].active)
            {
                //Delete the player's mechworm if it's attaching to something weird
                if (Main.projectile[chase].type != ModContent.ProjectileType<MechwormBody2>() &&
                    Main.projectile[chase].type != ModContent.ProjectileType<MechwormBody>() &&
                    Main.projectile[chase].type != ModContent.ProjectileType<MechwormHead>())
                {
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner &&
                            (Main.projectile[i].type == ModContent.ProjectileType<MechwormBody2>() ||
                             Main.projectile[i].type == ModContent.ProjectileType<MechwormBody>() ||
                             Main.projectile[i].type == ModContent.ProjectileType<MechwormHead>() ||
                             Main.projectile[i].type == ModContent.ProjectileType<MechwormTail>()))
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }
                value68 = Main.projectile[chase].Center;
                num1064 = Main.projectile[chase].rotation;
                scaleFactor18 = MathHelper.Clamp(Main.projectile[chase].scale, 0f, 50f);

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
                Main.projectile[chase].localAI[0] = projectile.localAI[0] + 1f;
            }
            else
            {
                //Maybe add some kill code here ?
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
                projectile.Center = value68 - Vector2.Normalize(vector134) * projectile.Size.Length() * scaleFactor18;
            }
            projectile.spriteDirection = (vector134.X > 0f) ? 1 : -1;
            projectile.netSpam = 5;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
