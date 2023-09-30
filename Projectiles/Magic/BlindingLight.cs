using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BlindingLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float Radius = 1400f;
        private const int Lifetime = 45;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Radius, targetHitbox);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SetCrit();

        public override void AI()
        {
            if (Projectile.timeLeft == Lifetime)
            {
                ConsumeNearbyBlades();
                DivideDamageAmongstTargets();
            }

            Projectile.ai[0]++;
            float progress = (float)Math.Sin(Projectile.ai[0] / Lifetime * MathHelper.Pi);
            if (Projectile.ai[0] > 55f)
                progress = MathHelper.Lerp(progress, 0f, (Projectile.ai[0] - 55f) / 5f);
            if (Main.netMode != NetmodeID.Server && Projectile.ai[0] > 15f) // Otherwise a white flash appears, but it quickly disappears.
            {
                if (!Filters.Scene["CalamityMod:LightBurst"].IsActive())
                    Filters.Scene.Activate("CalamityMod:LightBurst", Projectile.Center).GetShader().UseTargetPosition(Projectile.Center).UseProgress(0f);

                Filters.Scene["CalamityMod:LightBurst"].GetShader().UseProgress(progress);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
                Filters.Scene.Deactivate("CalamityMod:LightBurst");
        }

        private void ConsumeNearbyBlades()
        {
            int lightBlade = ModContent.ProjectileType<LightBlade>();
            int extraDamage = 0;
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile otherProj = Main.projectile[i];
                if (otherProj is null || !otherProj.active || otherProj.owner != Projectile.owner || otherProj.type != lightBlade)
                    continue;

                // Can only consume blades within the flash radius (which should be most if not all of them anyway)
                if (Projectile.Distance(otherProj.Center) > Radius)
                    continue;
                extraDamage += otherProj.damage / 2;
                otherProj.Kill();
            }
            Projectile.damage += extraDamage;
        }

        private void DivideDamageAmongstTargets()
        {
            int numTargets = 0;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc is null || !npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                    continue;
                if (Projectile.Colliding(default, npc.Hitbox))
                    ++numTargets;
            }

            // The number of targets is minimum one to prevent dividing by zero.
            if (numTargets <= 0)
                numTargets = 1;

            // Divide damage by the square root of nearby targets. 25 targets = 1/5th damage, for example.
            Projectile.damage = (int)(Projectile.damage / Math.Sqrt(numTargets));
        }
    }
}
