using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class WitherBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Bolt");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 0.8f;
            Projectile.width = Projectile.height = (int)(72 * Projectile.scale);
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.DarkGreen.ToVector3() * 1.25f);
            float oldSpeed = Projectile.velocity.Length();

            NPC potentialTarget = Projectile.Center.MinionHoming(800f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 3f + Projectile.SafeDirectionTo(potentialTarget.Center) * oldSpeed) / 4f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed * 1.01f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D boltTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(lightColor, Color.Olive, 0.6f);
                drawColor = Color.Lerp(drawColor, Color.Black, completionRatio);
                drawColor = Color.Lerp(drawColor, Color.Transparent, completionRatio);

                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(boltTexture, drawPosition, null, Projectile.GetAlpha(drawColor), Projectile.oldRot[i], boltTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < Projectile.oldPos.Length / 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust plague = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width / 2, Projectile.height / 2, 107);
                        plague.velocity = (Projectile.oldRot[i] - MathHelper.PiOver2).ToRotationVector2() * 4.5f + Main.rand.NextVector2Circular(2f, 2f);
                        plague.color = Color.Olive;
                        plague.noGravity = true;
                    }
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(8f, 8f);
                    int seeker = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), spawnPosition, Main.rand.NextVector2Circular(12f, 12f), ModContent.ProjectileType<PlagueSeeker>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[seeker].Calamity().forceMinion = true;
                }
            }
        }
    }
}
