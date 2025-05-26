using System;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ChronoIcicleLarge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public static int HomingSpeed = 16;
        public static int IdleSpeedMax = 7;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.coldDamage = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            switch (Projectile.ai[0])
            {
                // stay in an evenly spaced circle around the player, resembling the 12 ticks on a clock
                case 0:
                    if (Projectile.velocity.Length() > 0.01f)
                    {
                        Projectile.velocity *= 0.001f;
                    }
                    bool isCounter = Projectile.ai[2] < 0;
                    if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
                    {
                        float shardNum = Math.Abs(Projectile.ai[2]) - 1;
                        float aivar = isCounter ? 1 - shardNum - 1 : shardNum;
                        Projectile.position = Main.player[Projectile.owner].Center + (2 * MathHelper.Pi / 12 * aivar - MathHelper.PiOver2).ToRotationVector2() * 160 - Projectile.Size / 2;
                    }
                    if (Projectile.ai[1] >= (12 - Math.Abs(Projectile.ai[2])) * 5 + 2)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
                        for (int i = 0; i < 10; i++)
                        {
                            int dusttype = Main.rand.NextBool() ? 68 : 67;
                            if (Main.rand.NextBool(4))
                            {
                                dusttype = 80;
                            }
                            Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                            int dust = Dust.NewDust(Projectile.Center, 1, 1, dusttype, dspeed.X, dspeed.Y, 50, default, 1.1f);
                            Main.dust[dust].noGravity = true;
                        }
                        Projectile.ai[0] = 1;
                        Projectile.ai[1] = 0;
                    }
                    break;
                // jet out 
                case 1:
                    if (Projectile.velocity.Length() < IdleSpeedMax)
                    {
                        Projectile.velocity *= 3f;
                    }
                    if (Projectile.ai[1] > 20)
                    {
                        Projectile.ai[0] = 2;
                    }
                    break;
                // home if possible
                case 2:
                    // Set amount of extra updates.
                    if (Projectile.Calamity().defExtraUpdates == -1)
                        Projectile.Calamity().defExtraUpdates = Projectile.extraUpdates;

                    Vector2 destination = Projectile.Center;
                    float maxDistance = 1400;
                    bool locatedTarget = false;

                    // Find a target.
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);
                        if (!Main.npc[i].CanBeChasedBy(Projectile, false) || !Projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance))
                            continue;
                        destination = Main.npc[i].Center;
                        locatedTarget = true;
                        break;
                    }

                    if (locatedTarget)
                    {
                        // Increase amount of extra updates to greatly increase homing velocity.
                        Projectile.extraUpdates = Projectile.Calamity().defExtraUpdates + 1;

                        // Home in on the target.
                        Vector2 homeDirection = (destination - Projectile.Center).SafeNormalize(Vector2.UnitY);
                        Projectile.velocity = (Projectile.velocity * 0.2f + homeDirection * HomingSpeed) / (0.2f + 1f);
                    }
                    else
                    {
                        // Set amount of extra updates to default amount.
                        Projectile.extraUpdates = Projectile.Calamity().defExtraUpdates;
                        if (Projectile.velocity.Length() < IdleSpeedMax)
                        {
                            Projectile.velocity *= 3f;
                        }
                    }
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            target.AddBuff(ModContent.BuffType<TimeDistortion>(), 60);
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale = 0.7f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            // the icicles cannot hit while glued to the player
            if (Projectile.ai[0] > 0)
                return null;
            else
                return false;
        }
    }
}
