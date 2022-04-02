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
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            //Rotating 45 degrees if shooting right
            if (projectile.spriteDirection == 1)
            {
                projectile.rotation += MathHelper.ToRadians(45f);
            }
            //Rotating 45 degrees if shooting right
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(45f);
            }
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y = projectile.velocity.Y + 0.01f;
        }

        //So you can stick a needle up the Tinkerer's ass
        public override bool? CanHitNPC(NPC target) => target.type != NPCID.DD2EterniaCrystal && !target.immortal && !target.dontTakeDamage;
    }
}
