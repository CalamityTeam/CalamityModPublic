using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Healing
{
    public class AbsorberAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        private int AbDust = ModContent.DustType<AbsorberDust>();
        public int ShinkGrow = 0;
        public int Framecounter = 0;
        public int CleanseOnce = 1;
        public int PulseOnce = 1;
        public int PulseOnce2 = 1;
        public int PulseOnce3 = 1;
        public static readonly SoundStyle Spawnsound = new("CalamityMod/Sounds/Custom/OrbHeal3") { Volume = 0.5f };
        public List<bool> cleanseList = new List<bool>(new bool[Main.maxPlayers]);
        public ref int CleansingEffect => ref Main.player[Projectile.owner].Calamity().CleansingEffect;

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = Projectile.height = 336;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1810;
        }

        public override void AI()
        {
            Framecounter++;
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                Player player = Main.player[playerIndex];
                float targetDist = Vector2.Distance(player.Center, Projectile.Center);

                //Remove the players debuffs and defense damage, but only once per aura
                if (targetDist < 310f)
                {
                    player.AddBuff(ModContent.BuffType<AbsorberRegen>(), 600);
                    if (cleanseList[playerIndex] == false)
                    {
                        cleanseList[playerIndex] = true;
                        CleansingEffect = 1;
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int hasBuff = player.buffType[l];
                            if (player.buffTime[l] > 2 && CalamityLists.debuffList.Contains(hasBuff))
                            {
                                player.buffTime[l] *= 0;
                            }
                        }
                        for (int i = 0; i < 55; i++)
                        {
                            int dust = Dust.NewDust(player.Center, player.width + 4, player.height + 4, AbDust, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, Color.DarkSeaGreen, 5.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 1.5f;
                            Main.dust[dust].velocity.Y -= 0.5f;
                        }
                        SoundEngine.PlaySound(Spawnsound with { Pitch = -0.9f }, Projectile.Center);
                    }
                }
            }

            if (ShinkGrow == 0)
            {
                if (PulseOnce == 1)
                {
                    Particle pulse = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.DarkSeaGreen, new Vector2(1f, 1f), 0f, 0f, 0.2925f, 10);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    PulseOnce = 0;
                }

                if (Framecounter == 10)
                {
                    ShinkGrow = 1;
                }
            }
            if (ShinkGrow == 1)
            {
                if (PulseOnce2 == 1)
                {
                    Particle pulse2 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.DarkSeaGreen, new Vector2(1f, 1f), 0f, 0.2925f, 0.2925f, 1790);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                    PulseOnce2 = 0;
                }

                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(301.6f, 301.6f), AbDust, null, 0, Color.DarkSeaGreen);
                    dust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(292.5f, 292.5f), AbDust, null, 0, Color.DarkSeaGreen);
                    dust.scale = Main.rand.NextFloat(0.3f, 0.9f);
                    dust.noGravity = true;
                }

                if (Framecounter == 1800)
                {
                    ShinkGrow = 2;
                }
            }
            if (ShinkGrow == 2)
            {
                if (PulseOnce3 == 1)
                {
                    Particle pulse3 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.DarkSeaGreen, new Vector2(1f, 1f), 0f, 0.2925f, 0f, 10);
                    GeneralParticleHandler.SpawnParticle(pulse3);
                    PulseOnce3 = 0;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
