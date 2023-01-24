using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.DataStructures;

namespace CalamityMod.Projectiles.Turret
{
    public class PlagueExplosionGas : ModProjectile, IAdditiveDrawer
    {
        public bool ableToHit = true;
        public float randomRotation1 = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        public float randomRotation2 = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        public override string Texture => "CalamityMod/Projectiles/Summon/SmallAresArms/MinionPlasmaGas";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Gas");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 184;
            Projectile.friendly = true;
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


        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.scale = 0.5f + (Projectile.localAI[0] * 0.01f) ;
            Projectile.velocity *= 0.98f;
            if (Projectile.timeLeft < 40) // Remove hitbox once the projectile is barely visible anymore
                ableToHit = false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
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
            float opacity = 1f;
            opacity *= Projectile.timeLeft / Projectile.localAI[1];
            Color drawColor = new Color(55, 125, 11) * opacity;
            Vector2 scale = Projectile.Size / texture.Size() * Projectile.scale * 1.35f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, randomRotation1, origin, scale, 0, 0f);
            spriteBatch.Draw(texture, drawPosition, null, drawColor, randomRotation2, origin, scale, 0, 0f);
        }
    }
}
