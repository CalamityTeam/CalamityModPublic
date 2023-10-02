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
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SpearofDestiny";
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2") { PitchVariance = 0.3f, Volume = 0.5f };

        public int framesInAir = 0;
        private bool initialized = false;
        public int SparkChance = 8;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            AIType = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
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

            if (!initialized)
            {
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile.penetrate++;
                    Projectile.tileCollide = false;
                }
                initialized = true;
            }
            if (Main.rand.NextBool(SparkChance))
            {
                Vector2 SparkVelocity1 = Projectile.velocity.RotatedBy(-3, default) * 0.1f - Projectile.velocity / 8f;
                Vector2 SparkPosition1 = Projectile.velocity.RotatedBy(-0.8, default);
                SparkParticle spark = new SparkParticle(Projectile.Center + SparkPosition1, SparkVelocity1, false, Main.rand.Next(6, 8), Main.rand.NextFloat(0.7f, 1f), Color.PaleGoldenrod);
                GeneralParticleHandler.SpawnParticle(spark);

            }
            if (Main.rand.NextBool(SparkChance))
            {
                Vector2 SparkVelocity2 = Projectile.velocity.RotatedBy(3, default) * 0.1f - Projectile.velocity / 8f;
                Vector2 SparkPosition2 = Projectile.velocity.RotatedBy(0.8, default);
                SparkParticle spark2 = new SparkParticle(Projectile.Center + SparkPosition2, SparkVelocity2, false, Main.rand.Next(6, 8), Main.rand.NextFloat(0.7f, 1f), Color.PaleGoldenrod);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Vector2 center = Projectile.Center;
            float maxDistance = 200f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);
                    bool canHit = Projectile.Calamity().stealthStrike || Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                    if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                SparkChance = 16;
                Projectile.extraUpdates = 3;
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 12f) / (21f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 133, Projectile.oldVelocity.X * Main.rand.NextFloat(1.1f, 1.3f), Projectile.oldVelocity.Y * Main.rand.NextFloat(1.1f, 1.3f));
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
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
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        //public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Ichor, 120);
    }
}
