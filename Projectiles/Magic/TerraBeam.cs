using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class TerraBeam : BaseLaserbeamProjectile
    {
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1200f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public ref float ShardCooldown => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            // Generate a star-like and circular burst of terra dust.
            if (!Main.dedServ && Time == 5f)
            {
                int starPoints = 6;
                for (int i = 0; i < starPoints; i++)
                {
                    float angle = MathHelper.TwoPi * i / starPoints;
                    for (int j = 0; j < 12; j++)
                    {
                        float starSpeed = MathHelper.Lerp(1f, 7f, j / 12f);
                        Color dustColor = Color.Lerp(Color.White, Color.YellowGreen, j / 12f);
                        float dustScale = MathHelper.Lerp(1.6f, 0.85f, j / 12f);

                        Dust terraMagic = Dust.NewDustPerfect(projectile.Center, 107);
                        terraMagic.velocity = angle.ToRotationVector2() * starSpeed;
                        terraMagic.color = dustColor;
                        terraMagic.scale = dustScale;
                        terraMagic.noGravity = true;
                    }
                }

                int ovalPoints = 42;
                for (int i = 0; i < ovalPoints; i++)
                {
                    float angle = MathHelper.TwoPi * i / ovalPoints;
                    Dust terraMagic = Dust.NewDustPerfect(projectile.Center, 107);
                    terraMagic.velocity = angle.ToRotationVector2() * 6f;
                    terraMagic.scale = 1.1f;
                    terraMagic.noGravity = true;
                }
            }

            if (ShardCooldown > 0f)
                ShardCooldown--;
        }

        public override void DetermineScale() => projectile.scale = projectile.timeLeft / Lifetime * MaxScale;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawBeamWithColor(spriteBatch, Color.Lime * 1.1f, projectile.scale);
            DrawBeamWithColor(spriteBatch, Color.Yellow * 1.1f, projectile.scale * 0.5f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (ShardCooldown > 0f)
                return;

            // The "Center" of the laser is actually the start of it in this context.
            // Collision is done separately. This might have a slight offset due to collision
            // boxes, but that should be negligible.
            float lengthFromStart = projectile.Distance(target.Center);

            int totalShards = (int)MathHelper.Lerp(1, 4, MathHelper.Clamp(lengthFromStart / MaxLaserLength * 1.4f, 0f, 1f));
            int shardType = ModContent.ProjectileType<TerraShard>();
            int shardDamage = (int)(projectile.damage * 0.6);
            for (int i = 0; i < totalShards; i++)
            {
                int tries = 0;
                Vector2 spawnOffset;
                do
                {
                    spawnOffset = Main.rand.NextVector2CircularEdge(target.width * 0.5f + 40f, target.height * 0.5f + 40f);
                    tries++;
                }
                while (Collision.SolidCollision((target.Center + spawnOffset).ToTileCoordinates().ToVector2(), 4, 4) && tries < 10);

                Projectile.NewProjectile(target.Center + spawnOffset, Main.rand.NextVector2CircularEdge(6f, 6f), shardType, shardDamage, projectile.knockBack, projectile.owner);
            }

            ShardCooldown = 3f;
            projectile.netUpdate = true;
        }
    }
}
