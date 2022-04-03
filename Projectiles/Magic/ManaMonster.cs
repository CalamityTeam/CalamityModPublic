using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class ManaMonster : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public Player Target => Main.player[Projectile.owner];
        public const int NPCAttackTime = 50;
        public const int PlayerAttackRedirectTime = 45;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monster");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.Opacity = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Time < NPCAttackTime)
            {
                Projectile.Opacity = Utils.InverseLerp(0f, 30f, Time, true);
                if (Projectile.velocity.Length() < 27f)
                    Projectile.velocity *= 1.05f;
            }
            else
            {
                // Make an alert sound to indicate that the monster has become enraged and will now attack its caster.
                if (Time == NPCAttackTime)
                {
                    int ambientNoiseID = Main.rand.Next(39, 41 + 1);
                    SoundEngine.PlaySound(SoundID.Zombie, Target.Center, ambientNoiseID);
                    SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Target.Center);
                    CreateTransitionBurstDust();
                }

                if (Time < NPCAttackTime + PlayerAttackRedirectTime)
                {
                    float idealMovementDirection = Projectile.AngleTo(Target.Center);
                    float angularTurnSpeed = 0.09f;
                    float newSpeed = MathHelper.Lerp(Projectile.velocity.Length(), 22f, 0.1f);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(idealMovementDirection, angularTurnSpeed).ToRotationVector2() * newSpeed;
                }
                else if (Projectile.velocity.Length() < 35f)
                    Projectile.velocity *= 1.04f;

                // Fade out.
                Projectile.Opacity = Utils.InverseLerp(0f, 15f, Projectile.timeLeft, true);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public void CreateTransitionBurstDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 75; i++)
            {
                Dust brimstone = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Square(-25f, 25f), (int)CalamityDusts.Brimstone);
                brimstone.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 3.1f) * MathHelper.Lerp(1f, 2.175f, i / 75f);
                brimstone.velocity = Vector2.Lerp(brimstone.velocity, -Vector2.UnitY * brimstone.velocity.Length(), 0.5f);
                brimstone.scale = MathHelper.Lerp(1f, 1.875f, i / 75f) * Main.rand.NextFloat(0.8f, 1f);
                brimstone.fadeIn = 0.4f;
                brimstone.noGravity = true;
            }
        }

        // Ensure damage is not absolutely obscene when hitting players.
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 102;

        public override bool CanDamage() => Projectile.Opacity >= 1f;


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
