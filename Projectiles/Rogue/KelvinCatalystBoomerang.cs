using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class KelvinCatalystBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/KelvinCatalyst";
        public int AIState = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Catalyst");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
            projectile.coldDamage = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(AIState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            AIState = reader.ReadInt32();
        }

        public override void AI()
        {
            VisualAudioEffects();
            BoomerangAI();
        }

        private void BoomerangAI()
        {
            switch (AIState)
            {
                case 0:
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] >= 75f)
                    {
                        ResetStats(projectile.Calamity().stealthStrike);
                    }
                    break;
                case 1:
                    ReturnToPlayer();
                    break;
                case 2:
                    // Will target the targetted NPC that minions use btw
                    projectile.ChargingMinionAI(1200f, 1500f, 2200f, 150f, 0, 40f, 9f, 4f, new Vector2(0f, -60f), 40f, 9f, true, true);
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] >= 120)
                    {
                        ResetStats(false);
                    }
                    break;
            }
        }

        private void ReturnToPlayer()
        {
            Player player = Main.player[projectile.owner];
            float returnSpeed = 20f;
            float acceleration = 2f;
            Vector2 playerVec = player.Center - projectile.Center;
            float dist = playerVec.Length();

            // Delete the projectile if it's excessively far away.
            if (dist > 3000f)
                projectile.Kill();

            playerVec.Normalize();
            playerVec *= returnSpeed;

            // Home back in on the player.
            if (projectile.velocity.X < playerVec.X)
            {
                projectile.velocity.X += acceleration;
                if (projectile.velocity.X < 0f && playerVec.X > 0f)
                    projectile.velocity.X += acceleration;
            }
            else if (projectile.velocity.X > playerVec.X)
            {
                projectile.velocity.X -= acceleration;
                if (projectile.velocity.X > 0f && playerVec.X < 0f)
                    projectile.velocity.X -= acceleration;
            }
            if (projectile.velocity.Y < playerVec.Y)
            {
                projectile.velocity.Y += acceleration;
                if (projectile.velocity.Y < 0f && playerVec.Y > 0f)
                    projectile.velocity.Y += acceleration;
            }
            else if (projectile.velocity.Y > playerVec.Y)
            {
                projectile.velocity.Y -= acceleration;
                if (projectile.velocity.Y > 0f && playerVec.Y < 0f)
                    projectile.velocity.Y -= acceleration;
            }
            if (Main.myPlayer == projectile.owner)
                if (projectile.Hitbox.Intersects(player.Hitbox))
                    projectile.Kill();
        }

        private void VisualAudioEffects()
        {
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.3f / 255f, Main.DiscoR * 0.4f / 255f, Main.DiscoR * 0.5f / 255f);

            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 67, 0f, 0f, 100, default, 1f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;

            projectile.rotation += 0.25f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ResetStats(projectile.Calamity().stealthStrike);
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        private void ResetStats(bool chaseEnemies)
        {
            AIState = chaseEnemies ? 2 : 1;
            projectile.localAI[0] = 0f;
            projectile.width = projectile.height = 60;
            projectile.tileCollide = false;
            projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            int projType = ModContent.ProjectileType<KelvinCatalystStar>();
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[projType] < 25)
            {
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                for (int i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f), projType, projectile.damage / 6, projectile.knockBack * 0.5f, projectile.owner);

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f), projType, projectile.damage / 6, projectile.knockBack * 0.5f, projectile.owner);
                }
            }
            Main.PlaySound(SoundID.Item30, projectile.position);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
