using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;
using CalamityMod.Particles;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Graphics.Primitives;
using Microsoft.CodeAnalysis;

namespace CalamityMod.Projectiles.Ranged
{
    public class ExoCrystalArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public bool CreateLightning => Projectile.ai[0] == 1f;

        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.arrow = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            // Fade in.
            Projectile.Opacity = Utils.GetLerpValue(0f, 3f, Time, true);

            // Rapidly race towards the nearest target.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(HeavenlyGale.ArrowTargetingRange);
            if (potentialTarget != null && Time >= 10f)
            {
                Vector2 idealVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 33f;
                Projectile.velocity = (Projectile.velocity * 29f + idealVelocity) / 30f;
                Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 3f);
            }

            // Determine rotation.
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Emit light.
            DelegateMethods.v3_1 = Color.Lerp(Color.Lime, Color.White, 0.55f).ToVector3() * 0.65f;
            Utils.PlotTileLine(Projectile.Center - Projectile.velocity * 0.5f, Projectile.Center + Projectile.velocity * 0.5f, 16f, DelegateMethods.CastLightOpen);

            Time++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public float PrimitiveWidthFunction(float completionRatio) => Projectile.scale * 30f;

        public Color PrimitiveColorFunction(float _) => Color.Lime * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            float localIdentityOffset = Projectile.identity * 0.1372f;
            Color mainColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Color secondaryColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset + 0.2f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

            mainColor = Color.Lerp(Color.White, mainColor, 0.85f);
            secondaryColor = Color.Lerp(Color.White, secondaryColor, 0.85f);

            Vector2 trailOffset = Projectile.Size * 0.5f;
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].Apply();
            PrimitiveSet.Prepare(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => trailOffset, shader: GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"]), 53);
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(tex, drawPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Play a loud impact sound.
            SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { Volume = 0.2f }, Projectile.Center);

            // Explode into a bunch of exo particles.
            for (int i = 0; i < 2; i++)
            {
                Vector2 energyVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.6f) * Main.rand.NextFloat(7f, 11f);
                Color exoEnergyColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                SparkParticle exoEnergy = new(Projectile.Center, energyVelocity, false, 30, 1.3f, exoEnergyColor);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Create lightning from above if necessary.
            if (!CreateLightning)
                return;

            int lightningDamage = (int)(Projectile.damage * HeavenlyGale.LightningDamageFactor);
            Vector2 lightningSpawnPosition = target.Center - Vector2.UnitY.RotatedByRandom(0.24f) * Main.rand.NextFloat(960f, 1020f);
            Vector2 lightningShootVelocity = (target.Center - lightningSpawnPosition + target.velocity * 7.5f).SafeNormalize(Vector2.UnitY) * 14f;
            int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<ExoLightningBolt>(), lightningDamage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(lightning))
            {
                Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                Main.projectile[lightning].ai[1] = Main.rand.Next(100);
            }

            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
    }
}
