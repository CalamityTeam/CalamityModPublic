using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MineralMortarHoldout : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<MineralMortar>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/MineralMortar";

        public Player Owner { get; set; }
        public ref float Time => ref Projectile.ai[0];
        public ref float OffsetLength => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 68;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            bool cantUse = Owner == null || !Owner.active || Owner.dead || !Owner.channel || Owner.CCed || Owner.noItems;
            if (cantUse)
                Projectile.Kill();

            Vector2 mouse = Owner.Calamity().mouseWorld;

            Time++;

            // If the time reaches Item.useTime, it'll shoot the projectile, consume ammo and reset the timer.
            if (Time >= Owner.itemTimeMax)
            {
                Owner.PickAmmo(Owner.ActiveItem(), out _, out float speed, out int damage, out float knockback, out int usedItemAmmoId);

                Vector2 position = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.Size.Length() * 0.5f;

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, Owner.SafeDirectionTo(mouse) * speed, ModContent.ProjectileType<MineralMortarProjectile>(), damage, knockback, Projectile.owner, usedItemAmmoId);

                // Doesn't sync this to the server, it's just effects.
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 randomDirectionToMouse = Owner.SafeDirectionTo(mouse).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(8f, 10f);
                        Dust shootDust = Dust.NewDustPerfect(position, DustID.Torch, randomDirectionToMouse, Scale: Main.rand.NextFloat(1.5f, 2.5f));
                        shootDust.noGravity = true;
                        shootDust.noLight = true;
                        shootDust.noLightEmittence = true;
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 randomDirectionToMouse = Owner.SafeDirectionTo(mouse).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(8f, 10f);
                        Particle shootMist = new MediumMistParticle(position, randomDirectionToMouse, new Color(255, 100, 0), Color.Transparent, Main.rand.NextFloat(.3f, 1f), Main.rand.NextFloat(200f, 400f));
                        GeneralParticleHandler.SpawnParticle(shootMist);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 randomDirectionToMouse = Owner.SafeDirectionTo(mouse).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2f, 6f);
                        Particle shootDebri = new StoneDebrisParticle(position, randomDirectionToMouse, Color.Lerp(Color.White, Color.LightGray, Main.rand.NextFloat()), Main.rand.NextFloat(.6f, .8f), Main.rand.Next(30, 45 + 1));
                        GeneralParticleHandler.SpawnParticle(shootDebri);
                    }

                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/ScorchedEarthShot", 3) with { Volume = .2f, Pitch = 1.2f, PitchVariance = 1.1f }, Projectile.Center);
                }

                // Makes the distance between the projectile and the player 0, this gives a recoil effect.
                OffsetLength = 0f;

                Time = 0f;
            }

            // If the recoil happened, reset smoothly to default position.
            if (OffsetLength != 15f)
                OffsetLength = MathHelper.SmoothStep(OffsetLength, 15f, .2f);

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 offset = Projectile.rotation.ToRotationVector2() * OffsetLength;
            Vector2 armOffset = -Vector2.UnitY.RotatedBy(Projectile.rotation) * Utils.Remap(Vector2.Dot(Projectile.rotation.ToRotationVector2(), -Vector2.UnitY), 0f, -1f, 10f, 3f) * Projectile.direction;
            Projectile.Center = armPosition + offset + armOffset;

            Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, (mouse - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * Projectile.Size.Length(), .4f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.timeLeft = 2;

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            int direction = MathF.Sign(mouse.X - Owner.MountedCenter.X);
            Projectile.spriteDirection = Projectile.direction = direction;
            Owner.ChangeDir(direction);

            float armRotation = Projectile.rotation - MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation);

            // Multiplayer syncing every second.
            if (Time % 60 == 0)
            {
                Projectile.netUpdate = true;
                if (Projectile.netSpam >= 10)
                    Projectile.netSpam = 9;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Owner = Main.player[Projectile.owner];
            OffsetLength = 15f;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 origin = texture.Size() * 0.5f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float shake = Utils.Remap(Time, Owner.itemTimeMax * 0.33f, Owner.itemTimeMax, 0f, 3f);
            position += Main.rand.NextVector2Circular(shake, shake);

            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects);

            return false;
        }
    }
}
