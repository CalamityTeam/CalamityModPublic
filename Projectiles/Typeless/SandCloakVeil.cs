using CalamityMod.DataStructures;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SandCloakVeil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        private const float Radius = 360f;

        public override void SetDefaults()
        {
            Projectile.width = 450;
            Projectile.height = 450;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = SandCloak.SandVeilDuration;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.025f;
            Player Owner = Main.player[Projectile.owner];
            Player BuffedPlayer = Main.LocalPlayer;

            Vector2 posDiff = BuffedPlayer.Center - Projectile.Center;
            if (posDiff.Length() <= Radius)
            {
                BuffedPlayer.Calamity().getSandCloakAccelBoost = true;
                BuffedPlayer.statDefense += SandCloak.SandVeilDefenseBoost;
            }
            else
                BuffedPlayer.Calamity().getSandCloakAccelBoost = false;
            // Ensure the acceleration boost is removed if the player is still inside the sand veil when it dies
            if (Projectile.timeLeft == 1)
                BuffedPlayer.Calamity().getSandCloakAccelBoost = false;

            // Make the sand veil slowly follow its owner
            float ownerDist = Vector2.Distance(Projectile.Center, Owner.Center);
            if (ownerDist > Radius * 0.2f)
                Projectile.Center += Vector2.Normalize(Owner.Center - Projectile.Center) * (ownerDist > Radius * 0.5f ? 2.5f : 1.25f);

            // Kill the sand veil early if the owner dashes
            if (Owner.dashDelay == -1 && Projectile.timeLeft < SandCloak.SandVeilDuration - 45)
            {
                if (Projectile.timeLeft > 25)
                    Projectile.timeLeft = 25;
            }

            // Dust
            Circle dustCircle = new Circle(Projectile.Center, Radius);
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - Projectile.Center).Length() > 48)
                {
                    Vector2 dustVel = Projectile.SafeDirectionTo(dustPos).RotatedBy(-MathHelper.PiOver4) * Vector2.Distance(Projectile.Center, dustPos) * 0.04f;
                    Dust sand = Dust.NewDustPerfect(dustPos, DustID.Sand, dustVel, Scale: 0.5f);
                    sand.noGravity = true;
                    sand.fadeIn = 1f;
                }
            }
        }

        // Add Sand Cloak cooldown after the sand veil dies
        public override void OnKill(int timeLeft) => Main.player[Projectile.owner].AddCooldown(Cooldowns.SandCloak.ID, SandCloak.SandVeilCooldown);

        public override bool PreDraw(ref Color lightColor)
        {
            // Sprite Circle
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            float scaleStep = 0.05f;
            float rotationOffset = 0.03f;
            Color drawCol = Projectile.GetAlpha(Color.Lerp(lightColor, Color.White, 0.5f));
            float drawTransparency = 0.1f;

            if (Projectile.timeLeft > SandCloak.SandVeilDuration - 10)
                drawTransparency = (SandCloak.SandVeilDuration - Projectile.timeLeft) * 0.01f;
            else if (Projectile.timeLeft < 25)
                drawTransparency = Projectile.timeLeft * 0.004f;

            for (int i = 0; i < 20; i++)
            {
                // Sprite
                float rotation = (Projectile.rotation + rotationOffset * i * i) * (i % 2 == 0).ToDirectionInt();
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawCol * drawTransparency, rotation, tex.Size() / 2f, (Radius * 0.0044f) - (i * scaleStep), SpriteEffects.None, 0);
            }
            return false;
        }

        // CIT 14FEB2025: Replaced old manual knockback code with setting HitDirectionOverride
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.HitDirectionOverride = (target.Center.X > Projectile.Center.X).ToDirectionInt();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Radius, targetHitbox);
        public override bool? CanCutTiles() => false;
    }
}
