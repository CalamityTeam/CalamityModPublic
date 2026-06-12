using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.Player;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseGrenadeOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float time => ref Projectile.ai[0];
        public bool onSpawn = true;
        public Vector2 lastPos;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 15;
            Projectile.Calamity().CannotProc = true;
        }
        public override bool ShouldUpdatePosition() => (time < 120);
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Effects.ArsenalEffects.ArsenalPulseColor.ToVector3() * 0.5f);
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (onSpawn)
            {
                lastPos = Projectile.Center;
                Projectile.scale = (Projectile.Calamity().stealthStrike ? 1 : 0.7f);
                for (int i = 0; i <= 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Effects.ArsenalEffects.ArsenalPulseDust);
                    dust.scale = Main.rand.NextFloat(1.2f, 1.9f) * Projectile.scale;
                    dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotateRandom(0.5f) * Main.rand.NextFloat(5f, 7f);
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalPulseColor;
                    dust.fadeIn = 1;
                }
                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>());
                    dust.scale = Main.rand.NextFloat(1.2f, 1.9f) * Projectile.scale;
                    dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotateRandom(0.3f) * Main.rand.NextFloat(7f, 9f);
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalPulseColor;
                    dust.fadeIn = 0.3f;
                }

                onSpawn = false;
            }

            if (time < 120)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.023f * Projectile.ai[1] * (time > 25 ? 1 : 0)) * 0.997f * (time > 25 ? 1 : 0.98f);
            else
            {
                Vector2 goal = Vector2.Lerp(Owner.GetFrontHandPosition(CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation), Projectile.Center, 0.5f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, goal, (float)Math.Pow(Utils.GetLerpValue(120, 250, time, true), 5));

                Vector2 direction = Utils.DirectionTo(Projectile.Center, Owner.Center);
                float power = Utils.Distance(Projectile.Center, lastPos);
                Projectile.velocity = direction * power;
                
                if (time == 250 || Utils.Distance(Projectile.Center, Owner.Center) < 10)
                {
                    if (Projectile.ai[2] == 0 && Owner.HeldItem.type == ModContent.ItemType<PulseGrenade>() && Owner.controlUseItem)
                    {
                        Projectile reformGrenade = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), goal, Vector2.Zero, ModContent.ProjectileType<PulseGrenadeProjectile>(), (int)Owner.GetTotalDamage<RogueDamageClass>().ApplyTo(Owner.HeldItem.damage), 0f, Owner.whoAmI, 0);
                        reformGrenade.ai[1] = 5;
                        reformGrenade.ai[0] = Owner.HeldItem.useTime * 0.7f;
                        reformGrenade.Opacity = 0;
                        //Doze Apr 15 2026 - Because this isn't being spawnwed from the shoot, these bools break. Manually fixing here.
                        reformGrenade.Calamity().extorterBoost = true;
                        reformGrenade.Calamity().stealthStrikeSubProjectile = false;

                        SoundStyle grab = new("CalamityMod/Sounds/Item/LightCatch");
                        SoundEngine.PlaySound(grab with { Volume = 1f, Pitch = 0 }, Projectile.Center);
                        SoundStyle pulse = new("CalamityMod/Sounds/Item/PulseSound");
                        SoundEngine.PlaySound(pulse with { Volume = 0.1f, Pitch = 0.4f }, Projectile.Center);

                        Particle restock = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalPulseColor, "CalamityMod/Items/Weapons/DraedonsArsenal/PulseGrenade", Vector2.One, Main.rand.NextFloat(-0.2f, 0.2f), 1.5f, 0.9f, 45, true);
                        GeneralParticleHandler.SpawnParticle(restock);
                        Particle restock2 = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalPulseColor, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-0.2f, 0.2f), 0.55f, 0.05f, 30, true);
                        GeneralParticleHandler.SpawnParticle(restock2);

                        Projectile.Kill();
                        return;
                    }
                    else
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }

            float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
            if (targetDist < 1400f && squash > 0.2f && time > 5) // The trail
            {
                Particle trail = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/DualTrail", false, 13, 0.075f * Projectile.scale, Effects.ArsenalEffects.ArsenalPulseColor * 0.6f * squash, new Vector2(1 - 0.15f * squash, 1.3f + Utils.GetLerpValue(11, 15, Projectile.velocity.Length(), true) * 1.5f), true, false, shrinkSpeed: 0.2f * squash);
                GeneralParticleHandler.SpawnParticle(trail);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            time++;
            lastPos = Projectile.Center;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.23f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= (Projectile.Calamity().stealthStrike ? 0.57f : 0.33f) * damageMult;
        }
        public override bool? CanHitNPC(NPC target) => null;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> orb = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Vector2 squash = new Vector2(Utils.Remap(Projectile.velocity.Length(), 5, 10, 1, 0.6f), Utils.Remap(Projectile.velocity.Length(), 5, 10, 1, 2f));
            float timeleftFade = (float)Math.Pow(Utils.GetLerpValue(0, 40 * Projectile.extraUpdates, Projectile.timeLeft, true), 5);

            for (int i = 0; i < 6; i++)
            {
                Color orbColor = Color.Lerp(Effects.ArsenalEffects.ArsenalPulseColor, Color.White, i * 0.07f) with { A = 0 } * 0.5f;
                Vector2 scale = Projectile.scale * timeleftFade * squash * (0.05f + i * 0.01f) * 3;
                Main.EntitySpriteDraw(orb.Value, Projectile.Center - Main.screenPosition, null, orbColor, Projectile.rotation, orb.Size() * 0.5f, scale, SpriteEffects.None);
            }

            return false;
        }
    }
}
