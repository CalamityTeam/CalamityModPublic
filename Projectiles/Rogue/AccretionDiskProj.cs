using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class AccretionDiskProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AccretionDisk";
        private int Lifetime = 400;
        private int ReboundTime = 30;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Disk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.penetrate = -1;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            SpawnProjectilesNearEnemies();
            BoomerangAI();
            LightingAndDust();
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
            int timeMult = projectile.Calamity().stealthStrike ? 3 : 1;
            if (projectile.timeLeft < Lifetime * timeMult - ReboundTime * timeMult)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] == 1f)
            {
                Player player = Main.player[projectile.owner];
                float returnSpeed = 9f;
                float acceleration = 0.4f;
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

        private void SpawnProjectilesNearEnemies()
        {
            if (!projectile.friendly)
                return;

            float maxDistance = 300f;
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
                int counter = projectile.Calamity().stealthStrike ? 35 : 50;
                if (Main.player[projectile.owner].miscCounter % counter == 0)
                {
                    int splitProj = ModContent.ProjectileType<AccretionDisk2>();
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[splitProj] < 25)
                    {
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int disk = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage, projectile.knockBack, projectile.owner);
                            int disk2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage, projectile.knockBack, projectile.owner);
                            if (projectile.Calamity().stealthStrike)
                            {
                                Main.projectile[disk].idStaticNPCHitCooldown = Main.projectile[disk2].idStaticNPCHitCooldown = 6;
                                Main.projectile[disk].usesIDStaticNPCImmunity = Main.projectile[disk2].usesIDStaticNPCImmunity = true;
                                Main.projectile[disk].timeLeft = Main.projectile[disk2].timeLeft = 90;
                            }
                        }
                    }
                }
            }
        }

        private void LightingAndDust()
        {
            if (Main.rand.NextBool(3))
            {
                int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                Main.dust[rainbow].noGravity = true;
                Main.dust[rainbow].velocity *= 0f;
            }

            Lighting.AddLight(projectile.Center, 0.15f, 1f, 0.25f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
            if (!projectile.Calamity().stealthStrike)
                projectile.ai[0] = 1f;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
            if (!projectile.Calamity().stealthStrike)
                projectile.ai[0] = 1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
