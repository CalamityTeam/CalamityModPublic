using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FetidEmesisHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<FetidEmesis>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public ref float revFrames => ref Projectile.ai[0];
        public ref float cooldownTimer => ref Projectile.ai[1];
        public ref float shootingTimer => ref Projectile.ai[2]; // Dual functions for rapid fire shooting cooldown and recoil
        public bool isTired => cooldownTimer > 0;
        public float revSpeed = 1;
        public int maxFrames = 420;
        public int initialFireTime = 45;
        public float shineScale = 0;
        private bool secondShot = true;

        public override void KillHoldoutLogic()
        {
            if (!isTired && (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type))
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (!isTired && (Owner.CantUseHoldout() || HeldItem.type != Owner.HeldItem.type))
                cooldownTimer = (int)(Utils.Remap(revFrames, 0, 350, 40, 180, true));
            if (isTired)
            {
                PostFiringCooldown();
                return;
            }

            revSpeed = Utils.Remap(revFrames, 0, maxFrames - 120, 1, 20, true);
            if (shootingTimer >= initialFireTime && revFrames < maxFrames && secondShot && Owner.whoAmI == Main.myPlayer)
            {
                Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);
                
                SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotSmallAlt");
                SoundEngine.PlaySound(fire with { Volume = 0.7f, Pitch = -0.1f + revSpeed * 0.01f }, Projectile.Center);

                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;

                if (Owner.whoAmI == Main.myPlayer)
                {
                    float spread = 0.045f * Utils.GetLerpValue(0, maxFrames, revFrames, true);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (shootVelocity).RotatedByRandom(spread), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                
                for (int i = 0; i <= 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * 5, Main.rand.NextBool(5) ? 28 : 215, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.2f, 1.5f), 0, default, Main.rand.NextFloat(0.6f, 1.4f));
                    dust.noGravity = true;
                }

                OffsetLengthFromArm -= 5f;
                shineScale = 1;
                secondShot = false;
            }
            if (shootingTimer >= 60 && revFrames < maxFrames)
            {
                Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;
                if (Owner.whoAmI == Main.myPlayer)
                {
                    float spread = 0.045f * Utils.GetLerpValue(0, maxFrames, revFrames, true);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (shootVelocity).RotatedByRandom(spread).RotatedByRandom(0.04f), bulletAMMO, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Projectile.netUpdate = true;
                }
                shootingTimer = 0;
                secondShot = true;
            }

            shootingTimer += revSpeed;
            revFrames++;
            shineScale *= 0.77f;

            if (revFrames >= maxFrames && !isTired && Owner.whoAmI == Main.myPlayer)
            {
                Owner.SetScreenshake(6.5f);
                OffsetLengthFromArm -= 35f;
                cooldownTimer = (int)MathHelper.Lerp((int)(Utils.Remap(revFrames, 0, 350, 40, 180, true)), 180, 0.7f);
                SoundStyle bigShot = new("CalamityMod/Sounds/Custom/Perforator/PerfHiveIchorShoot");
                SoundStyle bigShotGun = new("CalamityMod/Sounds/Item/FlakKrakenShoot");
                SoundEngine.PlaySound(bigShot with { Pitch = -0.2f, Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(bigShotGun with { Volume = 0.6f }, Projectile.Center);

                int chunkDamage = (int)(Projectile.damage * 2.5f);
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 13;

                Particle pulse3 = new GlowSparkParticle(GunTipPosition - shootVelocity, shootVelocity, false, 12, 0.035f, Color.Chartreuse, new Vector2(2.5f, 0.9f), true);
                GeneralParticleHandler.SpawnParticle(pulse3);

                Particle pulse2 = new CustomPulse(GunTipPosition, Projectile.velocity * 14.5f, Color.Chartreuse * 0.7f, "CalamityMod/Particles/DustyCircleHardEdge", new Vector2(0.4f, 1f), Projectile.velocity.ToRotation(), 0.13f, 0, 34);
                GeneralParticleHandler.SpawnParticle(pulse2);
                Particle pulse = new CustomPulse(GunTipPosition, Projectile.velocity * 9f, Color.Chartreuse * 0.7f, "CalamityMod/Particles/FlameExplosion", new Vector2(0.4f, 1f), Projectile.velocity.ToRotation(), 0.25f, 0, 34);
                GeneralParticleHandler.SpawnParticle(pulse);
                
                if (Main.myPlayer == Projectile.owner)
                {
                    float velBonus = 1.1f;
                    for (int i = 0; i <= 5; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedBy(-0.03f * i * velBonus) * (1 - i * 0.08f) * velBonus, ModContent.ProjectileType<EmesisGore>(), chunkDamage, Projectile.knockBack * 3, Projectile.owner, -i, 0, velBonus);
                    for (int j = 0; j <= 5; j++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedBy(0.03f * j * velBonus) * (1 - j * 0.08f) * velBonus, ModContent.ProjectileType<EmesisGore>(), chunkDamage, Projectile.knockBack * 3, Projectile.owner, j, 0, velBonus);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity * velBonus, ModContent.ProjectileType<EmesisGore>(), chunkDamage, Projectile.knockBack * 3, Projectile.owner, 20, 0, velBonus);
                }

                for (int i = 0; i <= 18; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.RainbowTorch, shootVelocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.5f), 0, default, Main.rand.NextFloat(0.6f, 1.4f));
                    dust.noGravity = true;
                    dust.color = Color.Chartreuse;
                }
            }
        }
        private void PostFiringCooldown()
        {
            Owner.channel = true;
            if (revFrames > 3)
                revFrames *= 0.7f;
            if (cooldownTimer > 1)
            {
                Vector2 smokeVel = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 5;

                if (cooldownTimer % 29 == 0)
                {
                    SoundStyle tired = new("CalamityMod/Sounds/Custom/OldDukeHuff");
                    SoundEngine.PlaySound(tired with { Pitch = -0.2f }, Projectile.Center);
                    for (int i = 0; i <= 12; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.2f, 1f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                        dust.noGravity = false;
                        dust.color = Color.White;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Particle smoke = new HeavySmokeParticle(GunTipPosition, smokeVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.2f, 1f), Color.White, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.2f, 0.4f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
            
            cooldownTimer--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (revFrames < 2)
                return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 1; i <= 24; i++)
            {
                float attackLerp = (float)Math.Pow((double)(Utils.GetLerpValue(60, 200, revFrames, true)), (double)(8));
                float mult = MathHelper.Max(Utils.GetLerpValue(7, 0, i), Utils.GetLerpValue(17, 24, i));
                float outspace = 6 * attackLerp;
                Vector2 drawOffset = (((MathHelper.TwoPi * i / 24f).ToRotationVector2().RotatedBy(Projectile.rotation) * outspace) + Main.rand.NextVector2Circular(2, 2));
                Color auraColor = Color.Chartreuse with { A = 0 } * mult * (0.4f) * Utils.GetLerpValue(90, 135, revFrames, true);
                float aimAngle = drawRotation;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, auraColor, aimAngle, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/FullStar").Value;
            float rot = Main.GlobalTimeWrappedHourly * 4;

            if (!isTired && (revFrames < maxFrames) && revFrames > 20)
            {
                for (int i = -2; i <= 2; i++) // 5 times
                    Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.Chartreuse with { A = 0 } * 0.35f, Projectile.rotation + (shineScale + rot), rechargeTexture.Size() * 0.5f, new Vector2(1 - i * 0.2f, 1 + i * 0.2f) * shineScale * 3, SpriteEffects.None, 0);
                for (int i = -2; i <= 2; i++) // 5 times
                    Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.Chartreuse with { A = 0 } * 0.35f, Projectile.rotation - (shineScale + rot), rechargeTexture.Size() * 0.5f, new Vector2(1 - i * 0.2f, 1 + i * 0.2f) * shineScale * 3, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
