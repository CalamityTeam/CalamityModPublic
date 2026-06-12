using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ChronomancersScytheHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Items/Weapons/Magic/ChronomancersScythe";
        public static int ClockChance = 30;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ownerHitCheck = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 130 * Projectile.scale;
            float bladeWidth = 90 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation.ToRotationVector2() * bladeLength), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            Owner.ChangeDir((int)Projectile.ai[2]);

            Player player = Main.player[Projectile.owner];
            if (player is null || player.dead)
                Projectile.Kill();

            Projectile.velocity = Vector2.Zero;

            Projectile.rotation += (2 * MathHelper.Pi / 60) * Projectile.ai[2];

            UpdateOwnerVars();

            Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * 10f - Vector2.UnitX * 4 * Owner.direction;

            // once the scythe has completed a rotation, die
            if ((Projectile.rotation > 2 * MathHelper.Pi - MathHelper.PiOver2 + MathHelper.PiOver4 && Projectile.ai[2] == 1) || (Projectile.rotation < -(2 * MathHelper.Pi - MathHelper.PiOver2 + MathHelper.PiOver4) && Projectile.ai[2] == -1))
            {
                Projectile.Kill();
            }

            // spawn 12 clock hand icicles
            Projectile.ai[0]++;
            float HandInterval = 60 / 12;
            if (Projectile.ai[0] >= HandInterval)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    // Math is beautiful
                    float aivar = Projectile.ai[2] == -1 ? 1 - Projectile.ai[1] - 1 : Projectile.ai[1];
                    Vector2 spawnPos = Main.player[Projectile.owner].Center + (2 * MathHelper.Pi / 12 * aivar - MathHelper.PiOver2).ToRotationVector2() * 160;
                    Vector2 rotationVector = spawnPos - Main.player[Projectile.owner].Center;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, rotationVector * 5, ModContent.ProjectileType<ChronoIcicleLarge>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: Projectile.ai[2] * (Projectile.ai[1] + 1));
                    Main.projectile[p].rotation = (spawnPos - Main.player[Projectile.owner].Center).ToRotation() + MathHelper.PiOver2;
                }
                Projectile.ai[1]++;
                Projectile.ai[0] = 0;
            }
        }

        public void UpdateOwnerVars()
        {
            float rotOffset = Projectile.ai[2] == -1 ? 3 * MathHelper.Pi : MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - rotOffset);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<TimeDistortion>(), 60);
            if (Main.rand.NextBool(ClockChance))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2Circular(-2, 2), ModContent.ProjectileType<ChronoClock>(), 0, 0, Projectile.owner);
            }
        }

        //If we don't do that, the hit enemies get knocked back towards you if you hit them from the right??
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // This would knock enemies away consistently, but i'm choosing to go with the other option
            //hitDirection = Math.Sign(target.Center.X - Owner.Center.X);

            //Doing it this way lets the player choose if they want to knockback enemies towards them by pointing away from them
            modifiers.HitDirectionOverride = Owner.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Owner.ChangeDir((int)Projectile.ai[2]);

            Texture2D scytheTexture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            Vector2 handleOrigin = Projectile.ai[2] == -1 ? new Vector2(scytheTexture.Width, scytheTexture.Height) : new Vector2(0, scytheTexture.Height);
            float scytheRotation = Projectile.rotation;
            Main.EntitySpriteDraw(scytheTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), scytheRotation, handleOrigin, Projectile.scale, Projectile.ai[2] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }
    }
}
