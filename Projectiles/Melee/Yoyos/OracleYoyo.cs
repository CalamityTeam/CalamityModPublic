using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class OracleYoyo : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Oracle>();
        public int AuraFrame;

        // projectile.localAI[1] is the Aura Charge of the red lightning aura
        // Minimum value is zero. Maximum value is 200.
        // The aura turns on and begins damaging enemies at 20 charge.
        // The yoyo "supercharges" at 50 charge.
        // Its size caps out at 100 charge.
        public ref float AuraCharge => ref Projectile.localAI[1];

        private const float MaxCharge = 200f;
        private const float MinAuraRadius = 20f;
        private const float SuperchargeThreshold = 50f;
        private const float MaxAuraRadius = 100f;
        private const float MinDischargeRate = 0.05f;
        private const float MaxDischargeRate = 0.53f;
        private const float DischargeRateScaleFactor = 0.003f;
        private const float ChargePerHit = 4f;
        private const int HitsPerOrbVolley = 3;

        // The aura hits once per this many frames.
        private const int AuraLocalIFrames = 12;

        // Ensures that the main AI only runs once per frame, despite the projectile's multiple updates
        private const int UpdatesPerFrame = 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 800f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 60f / UpdatesPerFrame;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AuraFrame);
            writer.Write(AuraCharge);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AuraFrame = reader.ReadInt32();
            AuraCharge = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = UpdatesPerFrame;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * UpdatesPerFrame;
        }

        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();

            // Only do stuff once per frame, despite the yoyo's extra updates.
            if (!Projectile.FinalExtraUpdate())
                return;

            // Produces golden dust constantly while in flight. This helps light the yoyo.
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool(3) ? 244 : 246;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
                Main.dust[idx].scale = scale;
            }

            // The yoyo makes its own faint yellow light (unnoticeable once the lightning aura gets going)
            Lighting.AddLight(Projectile.Center, 0.6f, 0.42f, 0.1f);

            // The aura discharges over time based on its current charge.
            float discharge = MinDischargeRate + DischargeRateScaleFactor * AuraCharge;
            if (discharge > MaxDischargeRate)
                discharge = MaxDischargeRate;
            AuraCharge -= discharge;

            // Boundary checks on aura charge
            if (AuraCharge < 0f)
                AuraCharge = 0f;
            if (AuraCharge > MaxCharge)
                AuraCharge = MaxCharge;

            // If the aura is large enough to be considered "on", draw it, make sound and damage enemies
            if (AuraCharge > MinAuraRadius)
            {
                float auraRadius = AuraCharge > MaxAuraRadius ? MaxAuraRadius : AuraCharge;
                DrawRedLightningAura(auraRadius);

                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 22;
                    SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
                }

                if (AuraFrame % AuraLocalIFrames == 0)
                {
                    // The aura's direct damage scales with its charge and with melee stats.
                    float chargeRatio = AuraCharge / MaxCharge;
                    int auraDamage = Oracle.AuraBaseDamage + (int)(chargeRatio * (Oracle.AuraMaxDamage - Oracle.AuraBaseDamage));
                    DealAuraDamage(auraRadius, auraDamage);
                }
            }
            else
                Projectile.soundDelay = 2;

            AuraFrame = (AuraFrame + 1) % AuraLocalIFrames;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // On hit effects do not apply if no damage was done.
            if (hit.Damage <= 0)
                return;

            // Charge up the red lightning aura with every hit.
            AuraCharge += ChargePerHit;

            // Fire Auric orbs every few hits while supercharged.
            if (AuraCharge > SuperchargeThreshold && Projectile.numHits % HitsPerOrbVolley == 0)
                FireAuricOrbs();
        }

        // Uses dust type 260, which lives for an extremely short amount of time
        private void DrawRedLightningAura(float radius)
        {
            // Light emits from the yoyo itself while the aura is active, eventually becoming insanely bright
            float brightness = radius * 0.03f;
            Lighting.AddLight(Projectile.Center, brightness, 0.2f * brightness, 0.1f * brightness);

            // Number of particles on the circumference scales directly with the circumference
            float dustDensity = 0.2f;
            int numDust = (int)(dustDensity * MathHelper.TwoPi * radius);
            float angleIncrement = MathHelper.TwoPi / numDust;

            // Incrementally rotate the vector as a ring of dust is drawn
            Vector2 dustOffset = new Vector2(radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; ++i)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = 260;
                float scale = 1.6f + Main.rand.NextFloat(0.9f);
                int idx = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                Main.dust[idx].position = Projectile.Center + dustOffset;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].noLight = true;
                Main.dust[idx].velocity *= 0.5f;
                Main.dust[idx].scale = scale;
            }

            // Rarely, draw some "arcs" which are lines of dust to the edge
            if (Main.rand.NextBool(30))
            {
                int numArcs = 3;
                float arcDustDensity = 0.6f;
                if (Main.rand.NextBool())
                    ++numArcs;
                if (Main.rand.NextBool())
                    ++numArcs;

                Vector2 radiusVec = new Vector2(radius, 0f);
                int dustPerArc = (int)(arcDustDensity * radius);
                for (int i = 0; i < numArcs; ++i)
                {
                    radiusVec = radiusVec.RotatedByRandom(MathHelper.TwoPi);
                    for (int j = 0; j < dustPerArc; ++j)
                    {
                        Vector2 partialRadius = (float)j / dustPerArc * radiusVec;
                        int dustType = 260;
                        float scale = 1.6f + Main.rand.NextFloat(0.9f);
                        int idx = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                        Main.dust[idx].position = Projectile.Center + partialRadius;
                        Main.dust[idx].noGravity = true;
                        Main.dust[idx].noLight = true;
                        Main.dust[idx].velocity *= 0.3f;
                        Main.dust[idx].scale = scale;
                    }
                }

                // Make extra sound when these arcs happen
                SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
            }
        }

        private void DealAuraDamage(float radius, int baseDamage)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];

            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage || target.friendly)
                    continue;

                // Shock any valid target within range. Check all four corners of their hitbox.
                float d1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
                float d2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
                float d3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
                float d4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());
                float dist = MathHelper.Min(d1, d2);
                dist = MathHelper.Min(dist, d3);
                dist = MathHelper.Min(dist, d4);

                if (dist <= radius)
                {
                    int finalDamage = (int)owner.GetTotalDamage<MeleeDamageClass>().ApplyTo(baseDamage);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), finalDamage, 0f, Projectile.owner, i);
                        if (p.whoAmI.WithinBounds(Main.maxProjectiles))
                            p.DamageType = DamageClass.MeleeNoSpeed;
                    }
                }
            }
        }

        private void FireAuricOrbs()
        {
            // Play a sound when orbs are fired
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

            int numOrbs = 3;
            int orbID = ModContent.ProjectileType<Orbacle>();
            int orbDamage = Projectile.damage * 2;
            float orbKB = 8f;
            float angleVariance = MathHelper.TwoPi / numOrbs;
            float spinOffsetAngle = MathHelper.Pi / (2f * numOrbs);
            Vector2 posVec = new Vector2(2f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numOrbs; ++i)
            {
                posVec = posVec.RotatedBy(angleVariance);
                Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                velocity.Normalize();
                velocity *= 18f;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + posVec, velocity, orbID, orbDamage, orbKB, Main.myPlayer, 0.0f, 0.0f);
            }
        }
    }
}
