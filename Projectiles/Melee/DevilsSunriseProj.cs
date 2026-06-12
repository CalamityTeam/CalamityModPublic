using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseProj : ModProjectile
    {
        public static readonly SoundStyle HitSound = new SoundStyle("CalamityMod/Sounds/Item/MantisSwipe", 2) with { Pitch = 0.8f };
        public static readonly SoundStyle FullChargeSound = new SoundStyle("CalamityMod/Sounds/Item/HellbornImpact");
        public static readonly SoundStyle ThrowSound = new SoundStyle("CalamityMod/Sounds/Item/SwingMid") with { Pitch = 1.2f };
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DevilsSunrise>();

        public ref float Timer => ref Projectile.ai[1];
        private int red;
        private const int greenAndBlue = 100;
        private const float TimerMax = 450f;
        private bool reachedMaxCharge = false;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 28;
        public override void SetDefaults()
        {
            // These shouldn't matter because the projectile uses custom collision
            Projectile.width = 148;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Timer++;
            if (Timer > TimerMax)
            {
                Timer = TimerMax;
                if (!reachedMaxCharge)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 dustVel = Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / 35f) * Main.rand.NextFloat(10f, 12.5f);
                        Dust fullCh = Dust.NewDustPerfect(Owner.Center, ModContent.DustType<BrimstoneFlame>(), dustVel, Scale: 2.25f);
                        fullCh.noGravity = true;
                    }
                    SoundEngine.PlaySound(FullChargeSound, Owner.Center);
                    reachedMaxCharge = true;
                }
            }

            red = 30 + (int)(Timer * 0.5f);
            if (red > 255)
                red = 255;
            if (++Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;

            // Play swoosh sounds, and spawn dust
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
                Projectile.soundDelay = 24;
            }
            Vector2 dustSpawn = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(dustSpawn, red * 0.001f, 0.1f, 0.1f);
            if (Main.rand.NextBool(Timer >= TimerMax ? 2 : 4))
            {
                Dust dust = Dust.NewDustDirect(dustSpawn - Projectile.Size * 0.5f, Projectile.width, Projectile.height, DustID.RainbowTorch, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                dust.noGravity = true;
                dust.position -= Projectile.velocity;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (Owner.CantUseHoldout())
                    Projectile.Kill();
                else
                {
                    float velocityScale = 1f;
                    if (Owner.HeldItem.shoot == Projectile.type)
                        velocityScale = Owner.HeldItem.shootSpeed * Projectile.scale;

                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    Vector2 slashDirection = Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                    slashDirection = slashDirection.SafeNormalize(Vector2.UnitX * Owner.direction) * velocityScale;
                    if (slashDirection.X != Projectile.velocity.X || slashDirection.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = slashDirection;

                    // Spawn the cyclone if you right-click
                    if (Owner.Calamity().mouseRight && Timer >= TimerMax)
                    {
                        // 0.00175 is a carefully chosen constant value so that the deceleration of the cyclone makes it reach about the position of the cursor
                        Vector2 cycloneVel = Projectile.velocity * ((Owner.Calamity().mouseWorld - Owner.Center).Length() * 0.00175f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, cycloneVel, ModContent.ProjectileType<DevilsSunriseCyclone>(), Projectile.damage, 0f, Projectile.owner);
                        SoundEngine.PlaySound(ThrowSound, Owner.Center);
                        Projectile.Kill();
                    }
                }
            }

            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(red, greenAndBlue, greenAndBlue, Projectile.alpha);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center - Projectile.velocity, Projectile.Center + Projectile.velocity * 5.5f, Projectile.height * Projectile.scale * 1.8f, ref _);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound, target.Center);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            // Accelerate the timer on hits
            Timer += 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Owner = Main.player[Projectile.owner];
            // Draws 3 slash textures, each one a different size from the previous
            // The larger ones are also drawn slightly away from the player to look less awkward
            Texture2D slashTex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 mouseDir = Vector2.Normalize(Owner.Calamity().mouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true));
            SpriteEffects sp = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // Second and third slashes have animation offsets from the first slash
            Rectangle frame1 = slashTex.Frame(1, 28, 0, Projectile.frame);
            Rectangle frame2 = slashTex.Frame(1, 28, 0, (Projectile.frame + 9) % 28);
            Rectangle frame3 = slashTex.Frame(1, 28, 0, (Projectile.frame + 18) % 28);

            // Slashes are given a random rotation offset to make it look like they're slashing at the cursor with reckless abandon
            float randRotOffset = Main.rand.NextFloat(-0.25f, 0.25f);
            Main.EntitySpriteDraw(slashTex, drawPos, frame1, Projectile.GetAlpha(lightColor), Projectile.rotation + randRotOffset, frame1.Size() / 2f, Projectile.scale * 1.15f, sp);
            randRotOffset = Main.rand.NextFloat(-0.25f, 0.25f);
            Main.EntitySpriteDraw(slashTex, drawPos + mouseDir.RotatedBy(randRotOffset) * 15f, frame2, Projectile.GetAlpha(lightColor), Projectile.rotation + randRotOffset, frame2.Size() / 2f, Projectile.scale * 1.75f, sp);
            randRotOffset = Main.rand.NextFloat(-0.25f, 0.25f);
            Main.EntitySpriteDraw(slashTex, drawPos + mouseDir.RotatedBy(randRotOffset) * 22.5f, frame3, Projectile.GetAlpha(lightColor), Projectile.rotation + randRotOffset, frame3.Size() / 2f, new Vector2(Projectile.scale * 2f, Projectile.scale * 2.7f), sp);
            return false;
        }
    }
}
