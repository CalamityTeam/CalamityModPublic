using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExorcismShockwave : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            float minMult = 0.35f;
            int hitsToMinMult = 15;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center) - Vector2.UnitY;
            float launchPower = (Projectile.Calamity().stealthStrike ? 4f : 1) * 10;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float crossSize = (Projectile.Calamity().stealthStrike ? 4f : 1) * 130;
            float crosThickness = (Projectile.Calamity().stealthStrike ? 4f : 1) * 25;
            float _ = float.NaN;
            bool horizontalHit = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.UnitX * crossSize, Projectile.Center + Vector2.UnitX * crossSize, crosThickness, ref _);
            bool verticalHit = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.UnitY * crossSize, Projectile.Center + Vector2.UnitY * crossSize * 1.7f, crosThickness, ref _);
            return (horizontalHit || verticalHit);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player Owner = Main.player[Projectile.owner];
            float fade = Utils.GetLerpValue(0, 9, Projectile.timeLeft, true);
            Asset<Texture2D> him = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Jesus");

            if (Projectile.Calamity().stealthStrike && Main.zenithWorld)
            {
                Main.EntitySpriteDraw(him.Value, Owner.Center - Main.screenPosition, null, Color.White * 0.45f * fade, 0, him.Size() / 2f, 10, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
