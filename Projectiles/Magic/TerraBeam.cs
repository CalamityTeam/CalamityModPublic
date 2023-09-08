using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class TerraBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1200f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;

        public ref float ShardCooldown => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
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

                        Dust terraMagic = Dust.NewDustPerfect(Projectile.Center, 107);
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
                    Dust terraMagic = Dust.NewDustPerfect(Projectile.Center, 107);
                    terraMagic.velocity = angle.ToRotationVector2() * 6f;
                    terraMagic.scale = 1.1f;
                    terraMagic.noGravity = true;
                }
            }

            if (ShardCooldown > 0f)
                ShardCooldown--;
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(Color.Lime * 1.1f, Projectile.scale);
            DrawBeamWithColor(Color.Yellow * 1.1f, Projectile.scale * 0.5f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ShardCooldown > 0f)
                return;

            // The "Center" of the laser is actually the start of it in this context.
            // Collision is done separately. This might have a slight offset due to collision
            // boxes, but that should be negligible.
            float lengthFromStart = Projectile.Distance(target.Center);

            int totalShards = (int)MathHelper.Lerp(1, 3, MathHelper.Clamp(lengthFromStart / MaxLaserLength * 1.5f, 0f, 1f));
            int shardType = ModContent.ProjectileType<TerraShard>();
            int shardDamage = (int)(Projectile.damage * 0.5);
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

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + spawnOffset, Main.rand.NextVector2CircularEdge(6f, 6f), shardType, shardDamage, Projectile.knockBack, Projectile.owner);
            }

            ShardCooldown = 3f;
            Projectile.netUpdate = true;
        }
    }
}
