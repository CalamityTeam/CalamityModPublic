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
    public class BlueJellyAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int ShinkGrow = 0;
        public int Framecounter = 0;
        public int CleanseOnce = 1;
        public int PulseOnce = 1;
        public int PulseOnce2 = 1;
        public int PulseOnce3 = 1;
        public static readonly SoundStyle Spawnsound = new("CalamityMod/Sounds/Custom/OrbHeal1") { Volume = 0.5f };
        public ref int CleansingEffect => ref Main.player[Projectile.owner].Calamity().CleansingEffect;
        public List<bool> cleanseList = new List<bool>(new bool[Main.maxPlayers]);
        

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
                if (targetDist < 165f && cleanseList[playerIndex] == false)
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
                        int dust = Dust.NewDust(player.Center, player.width + 4, player.height + 4, 187, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default, 5.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.2f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }

                    SoundEngine.PlaySound(Spawnsound with { Pitch = -0.9f }, Projectile.Center);
                }
            }

            if (ShinkGrow == 0)
            {
                if (PulseOnce == 1)
                {
                    Particle pulse = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.RoyalBlue, new Vector2(1f, 1f), 0f, 0f, 0.152f, 10);
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
                    Particle pulse2 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.RoyalBlue, new Vector2(1f, 1f), 0f, 0.152f, 0.152f, 1790);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                    PulseOnce2 = 0;
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(155f, 155f), 187);
                    dust.scale = Main.rand.NextFloat(2.2f, 3.3f);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150f, 150f), 187);
                    dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
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
                    Particle pulse3 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.RoyalBlue, new Vector2(1f, 1f), 0f, 0.152f, 0f, 10);
                    GeneralParticleHandler.SpawnParticle(pulse3);
                    PulseOnce3 = 0;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
