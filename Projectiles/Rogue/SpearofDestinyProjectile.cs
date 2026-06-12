using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpearofDestinyProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2") { PitchVariance = 0.3f, Volume = 0.5f };

        public int framesInAir = 0;
        public int SparkChance = 1;

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            framesInAir++;
            if (framesInAir > 90 && !Projectile.Calamity().stealthStrike)
            {
                Projectile.velocity.X *= 0.998f;
                Projectile.velocity.Y += 0.3f;
            }

            if (Projectile.timeLeft % 2 == 0 && Main.rand.NextBool(SparkChance) && Projectile.numHits == 0)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 0.5f, Projectile.velocity * 0.01f, false, 7, 0.7f, Color.PaleGoldenrod * 0.3f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Vector2 center = Projectile.Center;
            float maxDistance = 200f;
            bool homeIn = false;

            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(n.width / 2) + (float)(n.height / 2);
                    bool canHit = Projectile.Calamity().stealthStrike || Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1);

                    if (Vector2.Distance(n.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = n.Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                SparkChance = 2;
                Projectile.extraUpdates = 3;
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 12f) / (21f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Firework_Yellow, Projectile.oldVelocity.X * Main.rand.NextFloat(1.1f, 1.3f), Projectile.oldVelocity.Y * Main.rand.NextFloat(1.1f, 1.3f));
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Hitsound, Projectile.position);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
