using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AngelicAllianceArchangel : ModProjectile
    {
        private int lifeSpan = 900;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archangel");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 68;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(lifeSpan);

        public override void ReceiveExtraAI(BinaryReader reader) => lifeSpan = reader.ReadInt32();

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.divineBless || player.dead || !player.active)
            {
                lifeSpan = 0;
            }

            // Initialization and dust
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            // Rotate around the player
            double deg = projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 300;
            projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;
            projectile.ai[1] += 1f;

            if (!projectile.FinalExtraUpdate())
                return;

            lifeSpan--;
            if (lifeSpan <= 0)
            {
                projectile.alpha += 30;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                    return;
                }
            }

            // Give off some light
            float lightScalar = Main.rand.NextFloat(0.9f, 1.1f) * Main.essScale;
            Lighting.AddLight(projectile.Center, 0.3f * lightScalar, 0.26f * lightScalar, 0.15f * lightScalar);

            // Get a target
            NPC target = projectile.Center.MinionHoming(2000f, player, false, true);

            // Shoot the target
            if (target != null)
            {
                Vector2 direction = target.Center - projectile.Center;
                direction.Normalize();
                direction *= 6f;
                if (direction.X >= 0.25f)
                {
                    projectile.direction = -1;
                }
                else if (direction.X < -0.25f)
                {
                    projectile.direction = 1;
                }
                projectile.ai[0]++;
                int timerLimit = 120;
                if (projectile.ai[0] > timerLimit && projectile.alpha < 50)
                {
                    if (Main.myPlayer == projectile.owner)
                    {
                        int type = ModContent.ProjectileType<AngelRay>();
                        Projectile.NewProjectile(projectile.Center, direction * 0.5f, type, projectile.damage, projectile.knockBack, projectile.owner);
                    }
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                Vector2 direction = player.Center - projectile.Center;
                direction.Normalize();
                direction *= 6f;
                if (direction.X >= 0.25f)
                {
                    projectile.direction = -1;
                }
                else if (direction.X < -0.25f)
                {
                    projectile.direction = 1;
                }
            }
            projectile.spriteDirection = projectile.direction;

            // Frames
            projectile.frameCounter++;
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
