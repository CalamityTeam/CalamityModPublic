using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;

namespace CalamityMod.Projectiles.Turret
{
    public class IceExplosion : ModProjectile, IAdditiveDrawer, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Misc";
        public bool ableToHit = true;
        public float randomRotation1 = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        public float randomRotation2 = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        public override string Texture => "CalamityMod/Projectiles/Summon/SmallAresArms/MinionPlasmaGas";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 184;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.localAI[1] = Projectile.timeLeft;
            Projectile.ArmorPenetration = 10;
        }

        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;
            return true;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.scale = 0.5f + (Projectile.localAI[0] * 0.01f) ;
            if (Projectile.timeLeft < 30) // Remove hitbox once the projectile is barely visible anymore
                ableToHit = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn2, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.scale * 92f, targetHitbox);
        }

        public override bool? CanDamage() => ableToHit ? null : false;

        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            if (Projectile.localAI[0] == 0f)
                return;

            
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = 0.9f;
            opacity *= Projectile.timeLeft / Projectile.localAI[1];
            Color drawColor = new Color(118, 217, 222) * opacity;
            Vector2 scale = Projectile.Size / texture.Size() * Projectile.scale * 1.35f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, randomRotation1, origin, scale, 0, 0f);
            spriteBatch.Draw(texture, drawPosition, null, drawColor, randomRotation2, origin, scale, 0, 0f);
        }
    }
}
