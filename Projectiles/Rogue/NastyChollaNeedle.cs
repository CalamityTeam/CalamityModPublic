using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Rogue
{
    public class NastyChollaNeedle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Needle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            //Rotating 45 degrees if shooting right
            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            //Rotating 45 degrees if shooting right
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.ToRadians(45f);
            }
            Projectile.velocity.X *= 0.9995f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.01f;
        }

        //So you can stick a needle up the Tinkerer's ass
        public override bool? CanHitNPC(NPC target) => target.type != NPCID.DD2EterniaCrystal && !target.immortal && !target.dontTakeDamage;
    }
}
