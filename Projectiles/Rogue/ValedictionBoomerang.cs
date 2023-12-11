using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ValedictionBoomerang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Valediction";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 60f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
                }
            }
            else
            {
                float returnSpeed = 30f;
                float acceleration = 5f;
                Vector2 projVector = player.Center - Projectile.Center;
                float playerDist = projVector.Length();
                if (playerDist > 3000f)
                {
                    Projectile.Kill();
                }
                playerDist = returnSpeed / playerDist;
                projVector.X *= playerDist;
                projVector.Y *= playerDist;
                if (Projectile.velocity.X < projVector.X)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f && projVector.X > 0f)
                    {
                        Projectile.velocity.X += acceleration;
                    }
                }
                else if (Projectile.velocity.X > projVector.X)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f && projVector.X < 0f)
                    {
                        Projectile.velocity.X -= acceleration;
                    }
                }
                if (Projectile.velocity.Y < projVector.Y)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f && projVector.Y > 0f)
                    {
                        Projectile.velocity.Y += acceleration;
                    }
                }
                else if (Projectile.velocity.Y > projVector.Y)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f && projVector.Y < 0f)
                    {
                        Projectile.velocity.Y -= acceleration;
                    }
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle projHitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                    Rectangle playerHitbox = new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height);
                    if (projHitbox.Intersects(playerHitbox))
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation += 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            OnHitEffects();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            int typhoonAmt = 3;
            int typhoonDamage = (int)(Projectile.damage * 0.3f);
            if (Projectile.owner == Main.myPlayer && Projectile.numHits < 1 && Projectile.Calamity().stealthStrike)
            {
                SoundEngine.PlaySound(SoundID.Item84, Projectile.position);
                for (int typhoonCount = 0; typhoonCount < typhoonAmt; typhoonCount++)
                {
                    Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (velocity.X == 0f && velocity.Y == 0f)
                    {
                        velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    velocity.Normalize();
                    velocity *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile typhoon = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<NuclearFuryProjectile>(), typhoonDamage, 0f, Projectile.owner, 0f, 1f);
                    if (typhoon.whoAmI.WithinBounds(Main.maxProjectiles))
                        typhoon.DamageType = RogueDamageClass.Instance;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 100;
            Projectile.Center = Projectile.position;
            for (int d = 0; d < 5; d++)
            {
                int water = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[water].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[water].scale = 0.5f;
                    Main.dust[water].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int water = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[water].noGravity = true;
                Main.dust[water].velocity *= 5f;
                water = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[water].velocity *= 2f;
            }
        }
    }
}
