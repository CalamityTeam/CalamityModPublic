using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class MetalChunk : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MetalMonstrosity";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true; //Its hella heavy so ofc
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Gravity
            Projectile.velocity.Y += 0.11f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            // Rotation
            Projectile.rotation += 0.14f * Projectile.direction;
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 10f)
                {
                    Vector2 speed = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<MetalShard>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 0f, 0f);
                    Projectile.ai[0] = 0f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Vector2 S1 = new Vector2(-Projectile.velocity.X, -Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 46)));
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, S1, ProjectileID.SpikyBall, (int)(Projectile.damage * 0.3), 0f, Projectile.owner, 0f, 0f);
                Main.projectile[proj].DamageType = RogueDamageClass.Instance;
                Main.projectile[proj].timeLeft = 600;
                Main.projectile[proj].usesLocalNPCImmunity = true;
                Main.projectile[proj].localNPCHitCooldown = 20;
                S1 = new Vector2(-Projectile.velocity.X * 0.7f, -Projectile.velocity.Y * 0.7f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 46)));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, S1, ModContent.ProjectileType<MetalShard>(), (int)(Projectile.damage * 0.3), 0f, Projectile.owner, 0f, 0f);
            }
            //Dust
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Lead, 0f, 0f, 0, default, 1.1f);
            }
        }
    }
}
