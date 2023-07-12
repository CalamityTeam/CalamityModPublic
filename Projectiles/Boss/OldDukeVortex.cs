using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeVortex : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/OldDukeVortex");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 408;
            Projectile.height = 408;
            Projectile.scale = 0.004f;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Main.zenithWorld)
            {
                if (Projectile.scale < 2f)
                {
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 1;

                    Projectile.scale += 0.004f;
                    if (Projectile.scale > 2f)
                        Projectile.scale = 2f;
                }
                else
                {
                    if (Projectile.timeLeft <= 85)
                    {
                        if (Projectile.alpha < 255)
                            Projectile.alpha += 3;

                        Projectile.scale += 0.012f;
                    }
                }
            }
            else
            {
                if (Projectile.scale < 1f)
                {
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 1;

                    Projectile.scale += 0.004f;
                    if (Projectile.scale > 1f)
                        Projectile.scale = 1f;

                    Projectile.width = Projectile.height = (int)(408f * Projectile.scale);
                }
                else
                {
                    if (Projectile.timeLeft <= 85)
                    {
                        if (Projectile.alpha < 255)
                            Projectile.alpha += 3;

                        Projectile.scale += 0.012f;
                        Projectile.width = Projectile.height = (int)(408f * Projectile.scale);
                    }
                    else
                        Projectile.width = Projectile.height = 408;
                }
            }

            Projectile.velocity = Vector2.Normalize(new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center) * 1.5f;

            Projectile.rotation -= 0.1f * (float)(1D - (Projectile.alpha / 255D));

            float lightAmt = 2f * Projectile.scale;
            Lighting.AddLight(Projectile.Center, lightAmt, lightAmt * 2f, lightAmt);

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 174;
                SoundEngine.PlaySound(SpawnSound, Projectile.Center);
            }

            if (Projectile.timeLeft > 85)
            {
                int numDust = (int)(0.2f * MathHelper.TwoPi * 408f * Projectile.scale);
                float angleIncrement = MathHelper.TwoPi / (float)numDust;
                Vector2 dustOffset = new Vector2(408f * Projectile.scale, 0f);
                dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);

                int var = (int)(408f * Projectile.scale);
                for (int i = 0; i < numDust; i++)
                {
                    if (Main.rand.NextBool(var))
                    {
                        dustOffset = dustOffset.RotatedBy(angleIncrement);
                        int dust = Dust.NewDust(Projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid);
                        Main.dust[dust].position = Projectile.Center + dustOffset;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity = Vector2.Normalize(Projectile.Center - Main.dust[dust].position) * 24f;
                        Main.dust[dust].scale = Projectile.scale * 3f;
                    }
                }

                float distanceRequired = 800f * Projectile.scale;
                float succPower = Main.zenithWorld ? 1f : 0.5f;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];

                    float distance = Vector2.Distance(player.Center, Projectile.Center);
                    if (distance < distanceRequired && player.grappling[0] == -1)
                    {
                        if (Collision.CanHit(Projectile.Center, 1, 1, player.Center, 1, 1))
                        {
                            float distanceRatio = distance / distanceRequired;

                            float wingTimeSet = (float)Math.Ceiling((float)player.wingTimeMax * 0.5f * distanceRatio);
                            if (player.wingTime > wingTimeSet)
                                player.wingTime = wingTimeSet;

                            float multiplier = 1f - distanceRatio;
                            if (player.Center.X < Projectile.Center.X)
                                player.velocity.X += succPower * multiplier;
                            else
                                player.velocity.X -= succPower * multiplier;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft <= 1680 && Projectile.timeLeft > 85;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 210f * Projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.timeLeft <= 1680 && Projectile.timeLeft > 85)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 600);
        }
    }
}
