using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Ammo;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class VanquisherArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/VanquisherArrow";
        public ref float Time => ref Projectile.ai[0];
        public ref float ProjectileSpeed => ref Projectile.ai[1];
        public bool Phase2 = false;
        public float HomingTime = 0;
        public Color MainColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.arrow = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 8;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15 * Projectile.extraUpdates;
        }

        public override void AI()
        {
            if (Time == 0)
            {
                Projectile.velocity *= 0.4f;
                ProjectileSpeed = 30;
                MainColor = Main.rand.NextBool() ? Color.Cyan : Color.Magenta;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Time > 4f && Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 226 : 272, -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.8f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.15f, 0.35f);
            }


            NPC target = Projectile.Center.ClosestNPCAt(65);

            if (target != null)
            {
                //Projectile.velocity = Projectile.velocity.RotatedByRandom(0.1f);
                Phase2 = true;
                HomingTime = 1;
            }

            if (HomingTime == 1)
                CalamityUtils.HomeInOnNPC(Projectile, true, 2000f, 12, 200f);

            Time++;
        }

        public override void PostDraw(Color lightColor)
        {
            Color color = Color.White;
            Rectangle frame = new Rectangle(0, 0, Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width, Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Ammo/VanquisherArrowGlow").Value, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // First hit is 0.85% damage
            if (Projectile.numHits < 1)
                Projectile.damage = (int)(Projectile.damage * .85f);
            // Second hit is 100% base damage, it is the "Slash Hit"
            else
                Projectile.damage = (int)(Projectile.damage * (1f / 0.85f));

            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            VoidSparkParticle spark2 = new VoidSparkParticle(Projectile.Center, new Vector2(0.1f, 0.1f).RotatedByRandom(100), false, 9, Main.rand.NextFloat(0.15f, 0.25f), Main.rand.NextBool() ? Color.Magenta : Color.Cyan);
            GeneralParticleHandler.SpawnParticle(spark2);

            SoundStyle onKill = new("CalamityMod/Sounds/Item/ScorpioHit");
            SoundEngine.PlaySound(onKill with { Volume = 0.25f, Pitch = 0.1f, PitchVariance = 0.3f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { Volume = 0.8f, Pitch = -0.5f, PitchVariance = 0.3f }, Projectile.Center);

        }
        public override bool PreDraw(ref Color lightColor)
        {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarProj").Value;
                if (Time > 6)
                    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], MainColor * 0.3f, 1, texture);
                return true;
        }
    }
}
