using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeSummon : ModProjectile
    {
        private double rotation = 0;
        private double rotationVariation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Probe");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;

                rotationVariation = Main.rand.NextDouble() * 0.015;

                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 dustSource = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    dustSource = dustSource.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = dustSource - Projectile.Center;
                    int astral = Dust.NewDust(dustSource + dustVel, 0, 0, (Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>()), dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[astral].noGravity = true;
                    Main.dust[astral].velocity = dustVel;
                }
                Projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                Projectile.damage = damage2;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<AstralProbeSummon>();
            player.AddBuff(ModContent.BuffType<AstralProbeBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.aProbe = false;
                }
                if (modPlayer.aProbe)
                {
                    Projectile.timeLeft = 2;
                }
            }
            NPC target = Projectile.Center.MinionHoming(1000f, player, true, true);
            Vector2 vector = player.Center - Projectile.Center;
            if (target != null)
            {
                Projectile.spriteDirection = Projectile.direction = ((target.Center.X - Projectile.Center.X) > 0).ToDirectionInt();
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(target.Center) + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi), 0.1f);
            }
            else
            {
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.rotation.AngleLerp(vector.ToRotation() - (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi * Projectile.direction), 0.1f);
            }
            Projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03 + rotationVariation;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            Projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;

            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 3);
            }
            if (Projectile.ai[1] > 80f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            float speedMult = 6f;
            int projType = ModContent.ProjectileType<AstralProbeRound>();
            if (target != null && Projectile.ai[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 12, 0.5f, 0f);
                Projectile.ai[1] += 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = target.Center - Projectile.Center;
                    velocity.Normalize();
                    velocity *= speedMult;
                    Projectile.NewProjectile(Projectile.Center, velocity, projType, Projectile.damage, 0f, Projectile.owner, target.whoAmI, 0f);
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool CanDamage() => false;
    }
}
