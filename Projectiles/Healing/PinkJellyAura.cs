using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typless
{
    public class PinkJellyAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        private static float Radius = 160f;
        public int ShinkGrow = 0;
        public int Framecounter = 0;
        public int PulseOnce = 1;
        public int PulseOnce2 = 1;
        public int PulseOnce3 = 1;
        public static readonly SoundStyle Spawnsound = new("CalamityMod/Sounds/Custom/OrbHeal1") { Volume = 0.5f };

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = Projectile.height = 336;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2710;
        }

        public override void AI()
        {
            Framecounter++;
            
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                Player player = Main.player[playerIndex];
                float targetDist = Vector2.Distance(player.Center, Projectile.Center);
                if (targetDist < 155f)
                {
                    player.AddBuff(ModContent.BuffType<PinkJellyRegen>(), 300);
                }
            }

            if (ShinkGrow == 0)
            {
                if (PulseOnce == 1)
                {
                    Particle pulse = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.HotPink, new Vector2(1f, 1f), 0f, 0f, 0.152f, 10);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    SoundEngine.PlaySound(Spawnsound with { Pitch = -0.9f }, Projectile.Center);
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
                    Particle pulse2 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.HotPink, new Vector2(1f, 1f), 0f, 0.152f, 0.152f, 2700);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                    PulseOnce2 = 0;
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(155f, 155f), 242);
                    dust.scale = Main.rand.NextFloat(2.2f, 3.3f);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150f, 150f), 242);
                    dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
                    dust.noGravity = true;
                }

                if (Framecounter == 2710)
                {
                    ShinkGrow = 2;
                }
            }
            if (ShinkGrow == 2)
            {
                if (PulseOnce3 == 1)
                {
                    Particle pulse3 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.HotPink, new Vector2(1f, 1f), 0f, 0.152f, 0f, 10);
                    GeneralParticleHandler.SpawnParticle(pulse3);
                    PulseOnce3 = 0;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
