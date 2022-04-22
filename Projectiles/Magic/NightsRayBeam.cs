using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NightsRayBeam : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public bool HasFiredSideBeams
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            Time++;
            if (Time >= 10f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustSpawnPos = Projectile.position - Projectile.velocity * i / 2f;
                    Dust corruptMagic = Dust.NewDustPerfect(dustSpawnPos, 27);
                    corruptMagic.color = Color.Lerp(Color.Fuchsia, Color.Magenta, Main.rand.NextFloat(0.6f));
                    corruptMagic.scale = Main.rand.NextFloat(0.96f, 1.04f);
                    corruptMagic.noGravity = true;
                    corruptMagic.velocity *= 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!HasFiredSideBeams && Projectile.owner == Main.myPlayer)
            {
                Vector2 baseSpawnPositionOffset = Main.rand.NextVector2CircularEdge(40f, 40f);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPosition = target.Center + baseSpawnPositionOffset.RotatedBy(MathHelper.TwoPi * i / 4f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<NightOrb>(), (int)(Projectile.damage * 0.8), Projectile.knockBack, Projectile.owner);
                }
                HasFiredSideBeams = true;
                Projectile.netUpdate = true;
            }
        }
    }
}
