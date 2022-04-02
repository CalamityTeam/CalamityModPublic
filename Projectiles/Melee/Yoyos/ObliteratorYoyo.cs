using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ObliteratorYoyo : ModProjectile
    {
        private const int FramesPerShot = 5;

        // Ensures that the main AI only runs once per frame, despite the projectile's multiple updates
        private int extraUpdateCounter = 0;
        private const int UpdatesPerFrame = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Obliterator");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 640f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 13f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.6f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = UpdatesPerFrame;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3 * UpdatesPerFrame;
        }

        // localAI[1] is the shot counter. Every 5 frames, The Obliterator tries to fire a laser at a nearby target.
        // It has 4 "laser ports" which whirl around in circles with the yoyo. It uses each of these in order.
        // localAI[1] counts up to 19 (4 x 5 - 1), then resets back to 0 for a 20-frame cycle.
        public override void AI()
        {
            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
                projectile.Kill();

            // Only do stuff once per frame, despite the yoyo's extra updates.
            extraUpdateCounter = (extraUpdateCounter + 1) % UpdatesPerFrame;
            if (extraUpdateCounter != UpdatesPerFrame - 1)
                return;

            Lighting.AddLight(projectile.Center, 0.8f, 0.3f, 1f);
            
            projectile.localAI[1]++;
            if (projectile.localAI[1] >= 4 * FramesPerShot)
                projectile.localAI[1] = 0f;

            // Attempt to fire a laser every 5 frames
            if(projectile.localAI[1] % FramesPerShot == 0f)
            {
                List<int> targets = new List<int>();
                float laserRange = 300f;
                for (int i = 0; i < Main.npc.Length; ++i)
                {
                    ref NPC n = ref Main.npc[i];
                    if (n is null || !n.active)
                        continue;

                    if (n.CanBeChasedBy(projectile, false) && (n.Center - projectile.Center).Length() <= laserRange && Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1))
                    {
                        targets.Add(i);
                        // Bosses are added 5 times instead of 1 so that they are preferentially but not exclusively targeted.
                        if (n.boss)
                            for (int j = 0; j < 4; ++j)
                                targets.Add(i);
                    }
                }
                if (targets.Count == 0)
                    return;

                // Pick which of the four corners the laser is spawning in
                Vector2 laserSpawnPosition = projectile.Center;
                Vector2 offset;
                if (projectile.localAI[1] < FramesPerShot)
                    offset = new Vector2(7, 7);
                else if (projectile.localAI[1] < 2 * FramesPerShot)
                    offset = new Vector2(-7, 7);
                else if (projectile.localAI[1] < 3 * FramesPerShot)
                    offset = new Vector2(-7, -7);
                else
                    offset = new Vector2(7, -7);
                laserSpawnPosition += offset.RotatedBy(projectile.rotation);

                ref NPC target = ref Main.npc[targets[Main.rand.Next(targets.Count)]];
                const float laserSpeed = 6f;
                int laserDamage = (int)(projectile.damage * 0.5f);
                const float laserKB = 3f;
                Vector2 velocity = target.Center - projectile.Center;
                velocity = velocity.SafeNormalize(Vector2.Zero) * laserSpeed;
                if (projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(laserSpawnPosition, velocity, ModContent.ProjectileType<NebulaShot>(), laserDamage, laserKB, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMelee = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(10f, 10f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/Yoyos/ObliteratorYoyoGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 2f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 90);
        }
    }
}
