using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ObliteratorYoyo : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private const int FramesPerShot = 5;

        // Ensures that the main AI only runs once per frame, despite the projectile's multiple updates
        private int extraUpdateCounter = 0;
        private const int UpdatesPerFrame = 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 720f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 54f / UpdatesPerFrame;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = UpdatesPerFrame;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * UpdatesPerFrame;
        }

        // localAI[1] is the shot counter. Every 5 frames, The Obliterator tries to fire a laser at a nearby target.
        // It has 4 "laser ports" which whirl around in circles with the yoyo. It uses each of these in order.
        // localAI[1] counts up to 19 (4 x 5 - 1), then resets back to 0 for a 20-frame cycle.
        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();

            // Only do stuff once per frame, despite the yoyo's extra updates.
            extraUpdateCounter = (extraUpdateCounter + 1) % UpdatesPerFrame;
            if (extraUpdateCounter != UpdatesPerFrame - 1)
                return;

            Lighting.AddLight(Projectile.Center, 0.8f, 0.3f, 1f);

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= 4 * FramesPerShot)
                Projectile.localAI[1] = 0f;

            // Attempt to fire a laser every 5 frames
            if(Projectile.localAI[1] % FramesPerShot == 0f)
            {
                List<int> targets = new List<int>();
                float laserRange = 300f;
                for (int i = 0; i < Main.npc.Length; ++i)
                {
                    ref NPC n = ref Main.npc[i];
                    if (n is null || !n.active)
                        continue;

                    if (n.CanBeChasedBy(Projectile, false) && (n.Center - Projectile.Center).Length() <= laserRange && Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1))
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
                Vector2 laserSpawnPosition = Projectile.Center;
                Vector2 offset;
                if (Projectile.localAI[1] < FramesPerShot)
                    offset = new Vector2(4, 4);
                else if (Projectile.localAI[1] < 2 * FramesPerShot)
                    offset = new Vector2(-4, 4);
                else if (Projectile.localAI[1] < 3 * FramesPerShot)
                    offset = new Vector2(-4, -4);
                else
                    offset = new Vector2(4, -4);
                laserSpawnPosition += offset.RotatedBy(Projectile.rotation);

                ref NPC target = ref Main.npc[targets[Main.rand.Next(targets.Count)]];
                const float laserSpeed = 6f;
                Vector2 velocity = target.Center - Projectile.Center;
                velocity = velocity.SafeNormalize(Vector2.Zero) * laserSpeed;
                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserSpawnPosition, velocity, ModContent.ProjectileType<NebulaShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = DamageClass.MeleeNoSpeed;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(10f, 10f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/ObliteratorYoyoGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 2f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
    }
}
