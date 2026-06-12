using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineSecondary : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Magic/RancorFog";

        public ref float ScaleFactor => ref Projectile.ai[0];
        public float LightPower = 0;

        public Color FogColor = Color.Orchid;
        public float FogRotation = 0;
        public int boomTime = PristineFury.boomTime;
        public int fullDamageHitCooldown = 0;

        public bool Ignited = false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 230;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 30;
        }

        public override void AI()
        {
            if (Projectile.ai[2] == boomTime)
                Ignited = true;
            if (Ignited)
            {
                if (Projectile.ai[2] == boomTime)
                {
                    Projectile.damage = (int)(Projectile.damage * 7.2f);
                    FogColor = Color.Lerp(Color.OrangeRed, Color.Goldenrod, Main.rand.NextFloat());

                    Projectile.localNPCHitCooldown = -1;
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;
                    fullDamageHitCooldown = 0;

                    SoundStyle ignite = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact");
                    SoundEngine.PlaySound(ignite with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.5f, 0.6f) }, Projectile.Center);
                }
                FogColor = Color.Lerp(Color.OrangeRed, Color.Goldenrod, Main.rand.NextFloat(0.5f, 1));
                if (Projectile.timeLeft > boomTime + 1)
                    Projectile.timeLeft = (int)(Projectile.timeLeft * 0.95f);
                Projectile.scale *= 1.12f;
                ScaleFactor *= 1.06f;
                if (Projectile.ai[2] > 1)
                    Projectile.ai[2]--;
            }
            // Add some degree of variation to the fog with scale/rotation/color
            if (FogRotation == 0f)
            {
                Projectile.scale = Main.rand.NextFloat(0.62f, 1.15f);
                FogRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                FogColor.G += (byte)Main.rand.Next(10, 80 + 1);
            }
            ScaleFactor += 0.014f;
            ScaleFactor = MathHelper.Clamp(ScaleFactor, 0f, Projectile.scale);
            Lighting.AddLight(Projectile.Center,FogColor.ToVector3() * ScaleFactor);
            Projectile.rotation = Projectile.velocity.ToRotation() + FogRotation;

            Projectile.velocity *= Main.rand.NextFloat(0.95f, 0.99f);
            Projectile.Opacity = Utils.GetLerpValue(280f, 135f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, true);

            if (fullDamageHitCooldown > 0)
                fullDamageHitCooldown--;

            // 08DEC2023: Ozzatron: All below code does not run on dedicated servers as it requires clientside lighting information.
            if (Main.dedServ)
                return;

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);

            if (Projectile.timeLeft < 220)
            {
                Vector2 vel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(7f, 25f) * Projectile.Opacity * Projectile.scale;

                if (Main.rand.NextBool(40))
                {
                    Particle spark = new CustomSpark(Projectile.Center + vel * 4 * Projectile.Opacity, vel * 0.05f, "CalamityMod/Particles/ProvidenceMarkParticle", false, 25, Main.rand.NextFloat(0.9f, 1.2f) * (Ignited ? 1.8f : 1), FogColor * Projectile.Opacity * 0.4f, new Vector2(1.3f, 0.5f), true, false, (Ignited ? 0 : Main.rand.NextFloat(-4, 4)), false, false, (Ignited ? 0.5f : 0));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (Main.rand.NextBool(30))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), vel * 0.25f * (Ignited ? 2f : 1));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.55f, 0.95f) * Projectile.Opacity * (Ignited ? 2f : 1);
                    dust.color = FogColor;
                }
            }
            if (Projectile.Opacity > 0.5f && Ignited)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (Vector2.Distance(Projectile.Center, projectile.Center) <= 200 * Projectile.scale && projectile.active && ((projectile.type == ModContent.ProjectileType<PristineSecondary>() && Projectile.ai[2] == 2 && projectile.ai[2] == 0 && projectile.Opacity > 0.5f)) && projectile != Projectile && Projectile.ai[1] < 100)
                    {
                        projectile.ai[2] = boomTime;
                        Projectile.ai[1] = Projectile.ai[1] + 1;
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.Opacity < 0.3f ? false : CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * ScaleFactor * 0.5f, targetHitbox);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (fullDamageHitCooldown == 0 && !target.Calamity().IsArmored()) // Deal full damage
            {
                fullDamageHitCooldown = Projectile.localNPCHitCooldown;
            }
            else // Deal partial damage
                modifiers.SourceDamage *= 0.06f;

            if (Ignited)
            {

                //Doze - Flamethrowers in vanilla are long debuff infliction tools (20 seconds of their debuff).
                //I am applying this as the base for Cal flamethrowers, with shorter times being the exception instead of the rule
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 1200);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.7f;
            Color drawColor = FogColor * opacity;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, texture.Size() * 0.5f, ScaleFactor, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
