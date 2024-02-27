using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivAura : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public readonly int SwordsAverageDelay = 40;
        public readonly int SwordsRandomOffset = 15;
        public int CurrentSwordTimer;
        public NPC target;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.npc[(int)Projectile.ai[0]] is not null || Main.npc[(int)Projectile.ai[0]].active)
                target = Main.npc[(int)Projectile.ai[0]];       // Convert target.whoAmI to object
        }

        // ai[0] for ID of target to follow until the end of projectile's lifespan or the target cannot be found
        // ai[1] for sword timer
        public override void AI()
        {
            // Unexist with target
            if (target is null || !target.active) {
                Projectile.Kill();
            }
 
            Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;     // Stay with target

            if (Projectile.ai[1] == CurrentSwordTimer)
            {
                Vector2 randomDirection = Main.rand.NextFloat(0, MathHelper.TwoPi).ToRotationVector2();
                randomDirection.Normalize();
                int randomDistance = Main.rand.Next(200, 426);
                Vector2 spawnPos = Projectile.Center + (randomDirection * randomDistance);
                Vector2 velocity = Vector2.Normalize(Projectile.Center - spawnPos) * Utils.GetLerpValue(-100, 426, randomDistance, clamped: true) * 20f;        // Sword moves a bit slower if spawned closer, moves faster if spawned further
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos.X, spawnPos.Y, velocity.X, velocity.Y, ModContent.ProjectileType<CosmicShivBlade>(), Projectile.damage, Projectile.knockBack * 0.8f, Projectile.owner, randomDistance);
                Projectile.ai[1] = 0;       // Reset timer
                CurrentSwordTimer = Main.rand.Next(SwordsAverageDelay - SwordsRandomOffset, SwordsAverageDelay + SwordsRandomOffset);       // Determine when next sword will spawn
            }

            Projectile.ai[1]++;
        }

        // The aura itself should not do contact damage
        public override bool? CanDamage() => false;
    }
}
