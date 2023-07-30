using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ArcherfishShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            // Bubbles
            if (Main.rand.NextBool(2))
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 411);
                bubble.timeLeft = 9 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }

            // Water trail
            for (int i = 0; i < 6; i++)
            {
                Dust water = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 211, 0f, 0f, 100);
                water.noGravity = true;
                water.velocity = Projectile.velocity * 0.5f;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Bubbles
            for (int i = 0; i < 10; i++)
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60f)) * 0.3f, 411);
                bubble.timeLeft = 9 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Wet, 240);
    }
}