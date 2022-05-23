using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class AuroradicalStar : ModProjectile
    {
        public int[] dustTypes = new int[]
        {
            ModContent.DustType<AstralBlue>(),
            ModContent.DustType<AstralOrange>()
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auroradical Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 100;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 485;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            //Rotation
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            //Lighting
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.1f);

            //sound effects
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 9);
                }
            }

            //Change the scale size a little bit to make it pulse in and out
            float scaleAmt = (float)Main.mouseTextColor / 200f - 0.35f;
            scaleAmt *= 0.2f;
            Projectile.scale = scaleAmt + 0.95f;

            //Spawn dust
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 15f)
            {
                int astral = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 100, default, 0.8f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 0f;
            }

            //Home in
            float maxDistance = 500f;
            int targetIndex = -1;
            int i;
            for (i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }
            if (targetIndex != -1)
            {
                Vector2 targetVec = Main.npc[i].Center - Projectile.Center;
                if (targetVec.Length() < 60f)
                {
                    Projectile.Kill();
                    return;
                }
                if (Projectile.ai[0] >= 120f)
                {
                    Projectile.ai[1] += 1f;
                    if (Projectile.ai[1] < 120f)
                    {
                        float speedMult = 25f;
                        targetVec.Normalize();
                        targetVec *= speedMult;
                        Projectile.velocity = (Projectile.velocity * 15f + targetVec) / 16f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= speedMult;
                    }
                    else if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 18f)
                    {
                        Projectile.velocity.X *= 1.01f;
                        Projectile.velocity.Y *= 1.01f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            OnHitEffect(target.Center, target.width);
        }

        private void OnHitEffect(Vector2 targetPos, int width)
        {
            if (Projectile.Calamity().stealthStrike && Main.myPlayer == Projectile.owner)
            {
                Vector2 pos = new Vector2(targetPos.X + width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                Vector2 velocity = (targetPos - pos) / 40f;
                int dmg = Projectile.damage / 2;
                int comet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity, ModContent.ProjectileType<CometQuasherMeteor>(), dmg, Projectile.knockBack, Projectile.owner);
                if (comet.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[comet].Calamity().forceRogue = true;
                    Main.projectile[comet].Calamity().lineColor = Main.rand.Next(3);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            OnHitEffect(target.Center, target.width);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 9, 1f, 0f);
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 96);
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Damage();
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 50, default, 1f);
            }
            for (int d = 0; d < 20; d++)
            {
                int astral = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 0, default, 1.5f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 3f;
                astral = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 50, default, 1f);
                Main.dust[astral].velocity *= 2f;
                Main.dust[astral].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int g = 0; g < 3; g++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                }
            }
        }
    }
}
