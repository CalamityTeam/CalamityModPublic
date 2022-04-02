using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TerraDiskProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TerraDisk";

        private bool initialized = false;
        private int Lifetime = 180;
        private int ReboundTime = 45;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Disk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 46;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Initialize();
            BoomerangAI();
            StealthStrikeAI();
            SpawnProjectilesNearEnemies();
            LightingandDust();
        }

        private void Initialize()
        {
            if (initialized)
                return;

            Lifetime = projectile.Calamity().stealthStrike ? 360 : 180;
            ReboundTime = projectile.Calamity().stealthStrike ? 90 : 45;
            projectile.timeLeft = Lifetime;
            initialized = true;
        }

        private void BoomerangAI()
        {
            // Boomerang rotation
            projectile.rotation += 0.4f * projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Returns after some number of frames in the air
            if (projectile.timeLeft < Lifetime - ReboundTime)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] == 1f)
            {
                //Ignore tiles and fly faster
                projectile.tileCollide = false;
                projectile.extraUpdates = 1;

                Player player = Main.player[projectile.owner];
                float returnSpeed = 16f;
                float acceleration = 1.4f;
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

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                        projectile.Kill();
            }
        }

        private void StealthStrikeAI()
        {
            if (!projectile.Calamity().stealthStrike)
                return;

            if (projectile.timeLeft % 8f == 0f && Main.myPlayer == projectile.owner)
            {
                int disk = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerraDiskProjectile2>(), projectile.damage / 4, projectile.knockBack / 4f, projectile.owner, projectile.identity, Main.rand.NextFloat(0.02f, 0.1f));
                Main.projectile[disk].timeLeft *= 2;
                Main.projectile[disk].idStaticNPCHitCooldown = 8;
                Main.projectile[disk].usesIDStaticNPCImmunity = true;
                Main.projectile[disk].aiStyle = -1;
            }
        }

        private void SpawnProjectilesNearEnemies()
        {
            if (!projectile.friendly)
                return;

            const float maxDistance = 300f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = npc.width / 2 + npc.height / 2;

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                    if (Vector2.Distance(npc.Center, projectile.Center) < maxDistance + extraDistance && canHit)
                    {
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                if (Main.player[projectile.owner].miscCounter % 50 == 0)
                {
                    int splitProj = ModContent.ProjectileType<TerraDiskProjectile2>();
                    if (projectile.owner == Main.myPlayer)
                    {
                        float spread = 60f * 0.0174f;
                        double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 6f;
                        double offsetAngle;
                        for (int i = 0; i < 3; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage / 2, projectile.knockBack / 2f, projectile.owner);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage / 2, projectile.knockBack / 2f, projectile.owner);
                        }
                    }
                }
            }
        }

        private void LightingandDust()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.75f, 0f);
            if (!Main.rand.NextBool(5))
                return;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Return to the player after striking an enemy. Stealth strikes don't return immediately.
            if (!projectile.Calamity().stealthStrike || projectile.ai[1] > 3)
                projectile.ai[0] = 1f;
            projectile.ai[1]++;
        }

        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            Main.PlaySound(SoundID.Dig, projectile.Center);
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 1f;
            return false;
        }
    }
}
