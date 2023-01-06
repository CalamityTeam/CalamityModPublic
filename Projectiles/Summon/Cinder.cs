using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Cinder : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            SpawnDust();
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }

        public void SpawnDust()
        {
            int bootlegTexture = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
            Main.dust[bootlegTexture].position += new Vector2(2f);
            Main.dust[bootlegTexture].scale += 0.3f + Main.rand.NextFloat(0.5f);
            Main.dust[bootlegTexture].noGravity = true;
            Main.dust[bootlegTexture].velocity.Y -= 2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.OnFire, 180);
    }
}
