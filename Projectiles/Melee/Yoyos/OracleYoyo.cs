using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class OracleYoyo : ModProjectile
    {
        public int AuraFrame;

        // projectile.localAI[1] is the Aura Charge of the red lightning aura
        // Minimum value is zero. Maximum value is 200.
        // The aura turns on and begins damaging enemies at 20 charge.
        // The yoyo "supercharges" at 50 charge.
        // Its size caps out at 100 charge.
        public ref float AuraCharge => ref projectile.localAI[1];

        private const float MaxCharge = 200f;
        private const float MinAuraRadius = 20f;
        private const float SuperchargeThreshold = 50f;
        private const float MaxAuraRadius = 100f;
        private const float MinDischargeRate = 0.05f;
        private const float MaxDischargeRate = 0.53f;
        private const float DischargeRateScaleFactor = 0.003f;
        private const float ChargePerHit = 3f;
        private const int HitsPerOrbVolley = 3;

        // Ensures that the main AI only runs once per frame, despite the projectile's multiple updates
        private const int UpdatesPerFrame = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Oracle");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 800f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
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
            projectile.aiStyle = 99;
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.2f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = UpdatesPerFrame;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3 * UpdatesPerFrame;
        }

        public override void AI()
        {
			if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
				projectile.Kill();

            // Only do stuff once per frame, despite the yoyo's extra updates.
            if (!projectile.FinalExtraUpdate())
                return;

            // Produces golden dust constantly while in flight. This helps light the yoyo.
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool(3) ? 244 : 246;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
                Main.dust[idx].scale = scale;
            }

            // The yoyo makes its own faint yellow light (unnoticeable once the lightning aura gets going)
            Lighting.AddLight(projectile.Center, 0.6f, 0.42f, 0.1f);

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

                if (projectile.soundDelay == 0)
                {
                    projectile.soundDelay = 22;
                    Main.PlaySound(SoundID.Item93, (int)projectile.Center.X, (int)projectile.Center.Y);
                }

                if (AuraFrame % 5 == 4)
                {
                    // The aura's direct damage scales with its charge and with melee stats.
                    float chargeRatio = AuraCharge / MaxCharge;
                    int auraDamage = Oracle.AuraBaseDamage + (int)(chargeRatio * (Oracle.AuraMaxDamage - Oracle.AuraBaseDamage));
                    DealAuraDamage(auraRadius, auraDamage);
                }
            }
            else
                projectile.soundDelay = 2;
            AuraFrame++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Charge up the red lightning aura with every hit
            AuraCharge += ChargePerHit;

            // Fire Auric orbs every few hits while supercharged.
            if (AuraCharge > SuperchargeThreshold && projectile.numHits % HitsPerOrbVolley == 0)
                FireAuricOrbs();
        }

        // Uses dust type 260, which lives for an extremely short amount of time
        private void DrawRedLightningAura(float radius)
        {
            // Light emits from the yoyo itself while the aura is active, eventually becoming insanely bright
            float brightness = radius * 0.03f;
            Lighting.AddLight(projectile.Center, brightness, 0.2f * brightness, 0.1f * brightness);

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
                int idx = Dust.NewDust(projectile.Center, 1, 1, dustType);
                Main.dust[idx].position = projectile.Center + dustOffset;
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
                        int idx = Dust.NewDust(projectile.Center, 1, 1, dustType);
                        Main.dust[idx].position = projectile.Center + partialRadius;
                        Main.dust[idx].noGravity = true;
                        Main.dust[idx].noLight = true;
                        Main.dust[idx].velocity *= 0.3f;
                        Main.dust[idx].scale = scale;
                    }
                }

                // Make extra sound when these arcs happen
                Main.PlaySound(SoundID.NPCHit53, (int)projectile.Center.X, (int)projectile.Center.Y);
            }
        }

        private void DealAuraDamage(float radius, int baseDamage)
        {
            if (projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[projectile.owner];

            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage || target.friendly)
                    continue;

                // Shock any valid target within range. Check all four corners of their hitbox.
                float d1 = Vector2.Distance(projectile.Center, target.Hitbox.TopLeft());
                float d2 = Vector2.Distance(projectile.Center, target.Hitbox.TopRight());
                float d3 = Vector2.Distance(projectile.Center, target.Hitbox.BottomLeft());
                float d4 = Vector2.Distance(projectile.Center, target.Hitbox.BottomRight());
                float dist = MathHelper.Min(d1, d2);
                dist = MathHelper.Min(dist, d3);
                dist = MathHelper.Min(dist, d4);

                if (dist <= radius)
                {
                    int finalDamage = (int)(baseDamage * owner.MeleeDamage());
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile p = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), finalDamage, 0f, projectile.owner, i);
						if (p.whoAmI.WithinBounds(Main.maxProjectiles))
						{
							p.melee = true;
							p.Calamity().forceMelee = true;
						}
                    }
                }
            }
        }

        private void FireAuricOrbs()
        {
            // Play a sound when orbs are fired
            Main.PlaySound(SoundID.Item92, (int)projectile.Center.X, (int)projectile.Center.Y);

            int numOrbs = 3;
            int orbID = ModContent.ProjectileType<Orbacle>();
            int orbDamage = projectile.damage * 3;
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
                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(projectile.Center + posVec, velocity, orbID, orbDamage, orbKB, Main.myPlayer, 0.0f, 0.0f);
            }
        }
    }
}
