using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class OmicronWingman : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Wingman>();
        public override string Texture => "CalamityMod/Items/Weapons/Magic/Wingman";

        public Color StaticEffectsColor = Color.MediumVioletRed;
        private ref float ShootingTimer => ref Projectile.ai[0];
        private float FiringTime = 10;
        private float PostFireCooldown = 0;
        public bool MovingUp = true;
        public float xOffset = 1;
        public float yOffset = 0;
        public int time = 0;
        public int firingDelay = 15;
        public int launchDelay = 0;

        private ref float OffsetLength => ref Projectile.localAI[0];

        private Player Owner;

        private float MaxOffsetLength = 5f;

        public bool recharging = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 142;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Owner ??= Main.player[Projectile.owner];

            if (time > 1 && Owner.ownedProjectileCounts[ModContent.ProjectileType<OmicronHoldout>()] < 1 && PostFireCooldown <= 0)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, StaticEffectsColor.ToVector3() * 0.2f);

            if (time == 0)
                MovingUp = Projectile.ai[2] == 1;

            firingDelay--;
            Item heldItem = Owner.HeldItem;

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Projectile.damage = heldItem is null ? 0 : Owner.GetWeaponDamage(heldItem);

            // If there's no player, or the player is the server, or the owner's stunned, there'll be no holdout.
            if (PostFireCooldown == 0 && launchDelay == 0 && Owner.CantUseHoldout() || heldItem.type != ModContent.ItemType<Omicron>())
            {
                if (PostFireCooldown <= 0)
                    Projectile.Kill();
            }

            if (PostFireCooldown > 0)
                PostFiringCooldown();

            if (launchDelay > 0 || (PostFireCooldown <= 0 && (Owner.Calamity().mouseRight || (firingDelay <= 0 && Projectile.ai[2] == 1 || Projectile.ai[2] == -1))))
            {
                // If the player's pressing RMB, it'll shoot the grenade.
                if (launchDelay > 0 || Owner.Calamity().mouseRight)
                {
                    if (launchDelay < 50)
                        launchDelay++;

                    if (launchDelay >= 50 && (Owner.CheckMana(Owner.HeldItem, (int)(heldItem.mana * Owner.manaCost) * 2, true, false)))
                    {
                        Shoot(true);
                        PostFireCooldown = 50;
                        ShootingTimer = 0;
                        launchDelay = 0;
                    }
                }
                else if (ShootingTimer >= FiringTime)
                {
                    if (Owner.CheckMana(Owner.HeldItem, -1, false, false))
                    {
                        Shoot(false);
                        ShootingTimer = 0;
                    }
                    else if (PostFireCooldown <= 0)
                        Projectile.Kill();
                }
            }

            // The center of the player, taking into account if they have a mount or not.
            Vector2 ownerPosition = Owner.MountedCenter;

            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - ownerPosition;

            // Deals with the holdout's rotation and direction, the owner's arms, etc.
            ManagePlayerProjectileMembers(ownerToMouse);

            // When we change the distance of the gun from the arms for the recoil,
            // recover to the original position smoothly.
            if (OffsetLength != MaxOffsetLength)
                OffsetLength = MathHelper.Lerp(OffsetLength, MaxOffsetLength, 0.1f);

            ShootingTimer++;
            time++;
            Projectile.soundDelay--;

            Projectile.ForceNetUpdate();
        }

        private void ManagePlayerProjectileMembers(Vector2 ownerToMouse)
        {
            Vector2 rotationVector = Projectile.rotation.ToRotationVector2();
            float velocityRotation = Projectile.velocity.ToRotation();
            int direction = MathF.Sign(ownerToMouse.X);
            Vector2 lengthOffset = rotationVector * OffsetLength;

            if (time % 30 == 0)
                MovingUp = !MovingUp;

            //xOffset = MathHelper.Lerp(xOffset, placementOffset.X, 0.01f);
            yOffset = MathHelper.Lerp(yOffset, 150 * (MovingUp ? -1 : 1), (0.085f - (FiringTime * 0.0012f)));

            Vector2 placementOffset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * (yOffset);
            Vector2 location = Owner.MountedCenter + placementOffset;
            Projectile.Center = lengthOffset + location;
            Projectile.velocity = velocityRotation.AngleTowards(ownerToMouse.ToRotation(), 0.2f).ToRotationVector2();
            Projectile.rotation = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX).ToRotation();
            Projectile.timeLeft = 2;
            Projectile.spriteDirection = Projectile.direction = direction;
        }

        private void Shoot(bool isGrenade)
        {
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 shootDirection = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);

            // The position of the tip of the gun.
            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 12f;

            Vector2 firingVelocity = shootDirection * 10;
            if (isGrenade)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/DeadSunExplosion");
                SoundEngine.PlaySound(fire with { Volume = 0.2f, Pitch = -0.4f, PitchVariance = 0.2f }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile bomb = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), tipPosition, firingVelocity, ModContent.ProjectileType<WingmanGrenade>(), Projectile.damage * 14, Projectile.knockBack * 5, Projectile.owner, 0, 2);
                    bomb.timeLeft = 530;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, firingVelocity * 1.2f, ModContent.ProjectileType<WingmanGrenade>(), Projectile.damage * 14, Projectile.knockBack * 5, Projectile.owner, 0, 2);
                }
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/MagnaCannonShot");
                SoundEngine.PlaySound(fire with { Volume = 0.25f, Pitch = 1f, PitchVariance = 0.35f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, firingVelocity, ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, firingVelocity.RotatedBy(-0.05) * 0.85f, ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, firingVelocity.RotatedBy(0.05) * 0.85f, ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
                }
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            for (int k = 0; k < 6; k++)
            {
                Vector2 shootVel = (shootDirection * 10).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                Dust dust2 = Dust.NewDustPerfect(tipPosition, Main.rand.NextBool(4) ? 264 : 66, shootVel);
                dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.Lerp(StaticEffectsColor, Color.White, 0.5f) : StaticEffectsColor;
            }

            Particle pulse = new GlowSparkParticle((tipPosition - shootDirection * 14), shootDirection * 20, false, Main.rand.Next(7, 11 + 1), 0.035f, StaticEffectsColor, new Vector2(1.5f, 0.9f), true);
            GeneralParticleHandler.SpawnParticle(pulse);

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            if (isGrenade)
                OffsetLength -= 27f;
            else
                OffsetLength -= 5f;
        }

        private void PostFiringCooldown()
        {
            Owner.channel = true;
            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 12f;

            if (PostFireCooldown > 0 && Main.rand.NextBool())
            {
                Vector2 smokeVel = new Vector2(0, -8) * Main.rand.NextFloat(0.1f, 1.1f);
                Particle smoke = new HeavySmokeParticle(tipPosition, smokeVel, StaticEffectsColor, Main.rand.Next(30, 50 + 1), Main.rand.NextFloat(0.1f, 0.4f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust dust = Dust.NewDustPerfect(tipPosition, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.1f), 80, default, Main.rand.NextFloat(0.2f, 0.8f));
                dust.noGravity = false;
                dust.color = StaticEffectsColor;
            }

            ShootingTimer = 0;
            firingDelay = 15;
            PostFireCooldown--;
        }

        public override void OnSpawn(IEntitySource source) => OffsetLength = MaxOffsetLength;

        // Because we use the velocity as a direction, we don't need it to change its position.
        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (time <= 0)
                return false;

            Texture2D texture;
            if (Projectile.ai[2] == 1)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/OmicronWingman").Value;
            else
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/OmicronWingmanAlt").Value;


            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Projectile.spriteDirection == -1 ? MovingUp : !MovingUp)
                flipSprite |= SpriteEffects.FlipVertically;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.Lerp(StaticEffectsColor, Color.White, 0.5f) * 0.2f, 1, texture);
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.spriteDirection);
            writer.Write(OffsetLength);
            writer.Write(time);
            writer.Write(firingDelay);
            writer.Write(launchDelay);
            writer.Write(MovingUp);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
            Projectile.spriteDirection = reader.ReadInt32();
            OffsetLength = reader.ReadSingle();
            time = reader.ReadInt32();
            firingDelay = reader.ReadInt32();
            launchDelay = reader.ReadInt32();
            MovingUp = reader.ReadBoolean();
        }
    }
}
