using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    internal class PurgeGuzzlerHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<PurgeGuzzler>();
        public override float MaxOffsetLengthFromArm => 35f;
        public override float OffsetYUpwards => base.OffsetYUpwards;
        public override float OffsetXUpwards => base.OffsetXUpwards;
        public override float OffsetXDownwards => base.OffsetXDownwards;
        public override float OffsetYDownwards => base.OffsetYDownwards;

        public override float BaseOffsetY => -5f;
        public override float RecoilResolveSpeed => 0.4f;
        public override float WeaponTurnSpeed => cooldownTimer > 0 ? 0.02f : 0.06f;
        public override string Texture => "CalamityMod/Items/Weapons/Magic/PurgeGuzzler";
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitX.RotatedBy(Projectile.rotation) * 10 + Vector2.UnitY * 3;
        public ref float revFrames => ref Projectile.ai[0];
        public ref float cooldownTimer => ref Projectile.ai[1];
        public ref float shootingTimer => ref Projectile.ai[2]; // Dual functions for rapid fire shooting cooldown and recoil
        public bool isOnCooldown => cooldownTimer > 0;
        public float revSpeed = 1;
        public int shotsFired = 0;

        public Color color1 = Color.Goldenrod;
        public Color color2 = Color.Orange;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (!isOnCooldown && (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type))
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (!isOnCooldown && (Owner.CantUseHoldout() || HeldItem.type != Owner.HeldItem.type))
                cooldownTimer = (int)(Utils.Remap(revFrames, 0, 350, 40, 120, true));
            if (isOnCooldown)
            {
                PostFiringCooldown();
                return;
            }

            if ((shootingTimer >= 10 && revFrames < 150) || (revFrames >= 150 && !isOnCooldown))
            {
                if (!Owner.CheckMana(Owner.HeldItem, -1, true))
                {
                    Projectile.Kill();
                    return;
                }
            }

            revSpeed = Utils.Remap(revFrames, 0, 150, 1, 3, true);
            if (shootingTimer >= 10 && revFrames < 150)
            {
                SoundStyle shot = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianShieldDeactivate");
                SoundEngine.PlaySound(shot with { Pitch = Utils.Remap(revFrames, 0, 150, 0.2f, 0.8f, true), Volume = 0.2f, MaxInstances = -1 }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    float spread = 0.045f * Utils.GetLerpValue(0, 300, revFrames, true);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(spread), ModContent.ProjectileType<HolyLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner, revSpeed * (shotsFired % 2 == 0 ? -1f : 1f) * Utils.Remap(revFrames, 120, 150, 0.1f, 0.45f, true), Projectile.whoAmI);
                }

                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(2f, 5f) * revSpeed, 0, default, Main.rand.NextFloat(0.3f, 0.7f) * revSpeed);
                    dust.noGravity = true;
                    dust.color = Color.Lerp(Color.Orchid, Color.White, Main.rand.NextFloat(0, 0.7f));
                }

                OffsetLengthFromArm -= 7f;
                shootingTimer = 0;
                shotsFired++;
            }

            shootingTimer += revSpeed;
            revFrames++;

            if (revFrames >= 150 && !isOnCooldown)
            {
                revSpeed = 4;
                Owner.SetScreenshake(3.5f);
                OffsetLengthFromArm -= 35f;
                cooldownTimer = 60;
                SoundStyle bigShot = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRay");
                SoundEngine.PlaySound(bigShot with { Pitch = 0.2f, Volume = 0.7f }, Projectile.Center);
                SoundStyle bigShot2 = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyRay");
                SoundEngine.PlaySound(bigShot2 with { Pitch = 0.4f, Volume = 0.8f }, Projectile.Center);

                
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                if (Main.myPlayer == Projectile.owner)
                {
                    int bigBeamDamage = (int)(Projectile.damage * 6.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<HolyLaser>(), bigBeamDamage, Projectile.knockBack * 3, Projectile.owner, 1, Projectile.whoAmI, 1);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<HolyLaser>(), bigBeamDamage, Projectile.knockBack * 3, Projectile.owner, -1, Projectile.whoAmI, 1);
                }

                for (int i = 0; i <= 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, shootVelocity.RotatedByRandom(0.8f) * Main.rand.NextFloat(5f, 30f), 0, default, Main.rand.NextFloat(0.6f, 1.4f));
                    dust.noGravity = true;
                    dust.color = Color.Lerp(Color.Orchid, Color.White, Main.rand.NextFloat(0, 0.7f));
                }
            }
        }
        private void PostFiringCooldown()
        {
            Owner.channel = true;

            if (cooldownTimer <= 1)
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

            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<HolyLaser>()] > 0 && shotsFired > 0)
            {
                float fade = Utils.GetLerpValue(1, 4, revSpeed);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * 5 * fade;
                    Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, Color.Orchid with { A = 0 } * fade, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite, 0f);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            
            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<HolyLaser>()] > 0 && shotsFired > 0)
            {
                float fade = Utils.GetLerpValue(1, 4, revSpeed);
                Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/LargeBloom").Value;
                float rot = Main.GlobalTimeWrappedHourly * 25;

                float sine = MathHelper.Clamp(Math.Abs((float)Math.Sin(rot * 2.275f / MathHelper.Pi)), 0.9f, 1f);

                // Glow Orb
                for (int i = 0; i < 6; i++)
                    Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, (Color.Lerp(Color.Orange, Color.Lerp(Color.Orchid, Color.Khaki, i * 0.1f), fade + 0.3f)) with { A = 0 } * 0.35f, rot * (i * 0.3f), rechargeTexture.Size() * 0.5f, (0.03f + i * 0.007f) * MathHelper.Clamp(revSpeed, 1f, 3.3f) * sine, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
