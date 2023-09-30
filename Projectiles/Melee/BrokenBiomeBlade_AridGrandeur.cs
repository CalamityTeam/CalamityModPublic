using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;


namespace CalamityMod.Projectiles.Melee
{
    public class AridGrandeur : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref Projectile.ai[1]; //Cooldown for the pogo
        public Player Owner => Main.player[Projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = BrokenBiomeBlade.HotAttunement_LocalIFrames;
            Projectile.timeLeft = BrokenBiomeBlade.HotAttunement_LocalIFrames;
        }

        public override bool? CanDamage()
        {
            return Projectile.timeLeft <= 2; //Prevent spam click abuse
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 94 * Projectile.scale;
            float bladeWidth = 76 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLength), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
        {
            if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);

                Vector2 hitPosition = Owner.Center + (direction * 84 * Projectile.scale);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    Particle smoke = new SmallSmokeParticle(hitPosition + hitPositionDisplace, flyDirection * 9f, Color.OrangeRed, new Color(130, 130, 130), Main.rand.NextFloat(1.8f, 2.6f), 115 - Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(smoke);

                    Particle Glow = new StrongBloom(hitPosition - hitPositionDisplace * 3, -direction * 6 * Main.rand.NextFloat(0.5f, 1f), Color.Orange * 0.5f, 0.01f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(40));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(hitPosition - hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Beige, 1f + Main.rand.NextFloat(0f, 0.4f), 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }

                if (Owner.HeldItem.type == ItemType<BrokenBiomeBlade>())
                    (Owner.HeldItem.ModItem as BrokenBiomeBlade).CanLunge = 1; // Reset the lunge counter on pogo. This should make for more interesting and fun synergies
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.56f, 0.56f) * ShredRatio);

            //Manage position and rotation
            direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
            direction.Normalize();
            Projectile.rotation = direction.ToRotation();
            Projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            Projectile.localNPCHitCooldown = BrokenBiomeBlade.HotAttunement_LocalIFrames - (int)(MathHelper.Lerp(0, BrokenBiomeBlade.HotAttunement_LocalIFrames - BrokenBiomeBlade.HotAttunement_LocalIFramesCharged, ShredRatio)); //Increase the hit frequency
            Projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if (Collision.SolidCollision(Owner.Center + (direction * 84 * Projectile.scale) - Vector2.One * 5f, 10, 10))
            {
                Pogo();
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Shred -= BrokenBiomeBlade.HotAttunement_ShredDecayRate;
            PogoCooldown--;
            if (Projectile.timeLeft <= 2)
                Projectile.timeLeft = 2;
        }

        //Since the iframes vary, adjust the damage to be consistent no matter the iframes. The true scaling happens between the BaseDamage and the FulLChargeDamage
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float maxMultiplier = BrokenBiomeBlade.HotAttunement_FullChargeDamage / (float)BrokenBiomeBlade.HotAttunement_BaseDamage;
            float damageMultiplier = MathHelper.Lerp(1f, maxMultiplier, ShredRatio);
            //Adjust the damage to make it constant based on the local iframes
            float damageReduction = Projectile.localNPCHitCooldown / (float)BrokenBiomeBlade.HotAttunement_LocalIFrames;

            modifiers.SourceDamage *= damageMultiplier * damageReduction;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float maxMultiplier = BrokenBiomeBlade.HotAttunement_FullChargeDamage / (float)BrokenBiomeBlade.HotAttunement_BaseDamage;
            float damageMultiplier = MathHelper.Lerp(1f, maxMultiplier, ShredRatio);
            //Adjust the damage to make it constant based on the local iframes
            float damageReduction = Projectile.localNPCHitCooldown / (float)BrokenBiomeBlade.HotAttunement_LocalIFrames;

            modifiers.SourceDamage *= damageMultiplier * damageReduction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ShredTarget();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;
            // get lifted up
            if (PogoCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit30, Projectile.Center); //Sizzle
                Shred += 62; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(BrokenBiomeBlade.HotAttunement_ShredIFrames); // i framez.
                PogoCooldown = 20;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit43, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/BrokenBiomeBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur").Value;

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            Main.EntitySpriteDraw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);
            drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 32f * Projectile.scale) - Main.screenPosition;

            Main.EntitySpriteDraw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra").Value;

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 7f) * ShredRatio);

                drawOrigin = new Vector2(0f, blade.Height);


                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                Main.EntitySpriteDraw(blade, drawOffsetStraight + drawDisplacementAngle, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }
}
