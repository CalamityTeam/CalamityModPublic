using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class AlphaVirusProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AlphaVirus";

        public static int lifetime = 600;
        public static float finalVelocity = 2f;
        public static float decelerationRate = 0.07f;
        private const float radius = 100f;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f * Projectile.direction;

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.ai[0] > finalVelocity)
                {
                    Projectile.ai[0] -= decelerationRate;
                    if (Projectile.ai[0] < finalVelocity)
                    {
                        Projectile.ai[0] = finalVelocity;
                    }

                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Projectile.ai[0];
                }
                if (Projectile.timeLeft < lifetime - 30 && Projectile.timeLeft % 15 == 0 && Projectile.ai[0] <= finalVelocity)
                {
                    int projID = ModContent.ProjectileType<AlphaSeeker>();
                    int damage = (int)(Projectile.damage * 0.125f);
                    float kb = 1f;
                    float ai0 = 1f; // Stealth strike spawned seekers set ai[0] to 1, which makes them cling to the parent projectile.
                    Vector2 vel = Main.rand.NextVector2CircularEdge(5f, 5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, projID, damage, kb, Projectile.owner, ai0, Projectile.identity);
                }
            }
            else
            {
                Projectile.timeLeft--;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float dist1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist <= Projectile.width)
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            float dist1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist <= Projectile.width)
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float dist1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist > Projectile.width)
            {
                modifiers.SourceDamage *= 0.2f;
                modifiers.Knockback *= 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Aura effect
            Texture2D aura = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/AlphaVirusAura").Value;
            float scaleStep = 0.03f;
            float rotationOffset = 0.03f;
            float drawTransparency = 0.1f;

            if (Projectile.timeLeft > lifetime - 10)
            {
                drawTransparency = (lifetime - Projectile.timeLeft) * (drawTransparency / 10);
            }
            else if (Projectile.timeLeft < 25)
            {
                drawTransparency = Projectile.timeLeft * (drawTransparency / 25);
            }

            Color drawCol = Color.White;

            for (int i = 0; i < 10; i++)
            {
                Main.EntitySpriteDraw(aura, Projectile.Center - Main.screenPosition, null, drawCol * drawTransparency, -(Projectile.rotation * 0.2f) + (rotationOffset * i * i), aura.Size() / 2f, Projectile.scale - (i * scaleStep), SpriteEffects.None, 0);
            }

            // Dust
            for (int i = 0; i < (lifetime - Projectile.timeLeft) / 30; i++)
            {
                float min = Projectile.width / 2;
                float max = radius;

                Vector2 pos = new Vector2(0f, -Main.rand.NextFloat(min, max));
                pos = pos.RotatedByRandom(MathHelper.TwoPi);
                Vector2 velocity = -pos * 0.02f;
                pos += Projectile.Center;

                int dust = Dust.NewDust(pos, 1, 1, 89, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velocity + Projectile.velocity;
            }

            // Main sprite
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int numSeekers = 6;
            int damage = Projectile.damage;
            float kb = 1f;
            float speed = 10f;
            for (int i = 0; i < numSeekers; i++)
            {
                Vector2 velocity = (MathHelper.Pi * i / 3f).ToRotationVector2() * speed;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<AlphaSeeker>(), damage, kb, Projectile.owner, 0f, 0f);
            }

            int numDust = 20;
            for (int i = 0; i < numDust; i++)
            {
                float min = Projectile.width / 2;
                float max = radius;

                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat(min, max));
                velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                velocity *= 0.1f;

                int dust = Dust.NewDust(Projectile.Center, 1, 1, 89, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velocity + Projectile.velocity;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
