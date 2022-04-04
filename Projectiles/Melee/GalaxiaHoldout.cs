using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxiaHoldout : ModProjectile //Visuals. Now much simpler than before!
    {
        private Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.ModItem as FourSeasonsGalaxia).CanUseItem(Owner) : false;
        public ref float Initialized => ref Projectile.ai[0];
        public ref float CycleDirection => ref Projectile.ai[1];

        private Item associatedItem;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Formation");
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
        }

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<FourSeasonsGalaxia>())
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.Center = Owner.Center;
                associatedItem = Owner.HeldItem;

                //Switch up the attunements
                Reattune((FourSeasonsGalaxia)associatedItem.ModItem);

                //Do particles around the player

                Color particleColor = (associatedItem.ModItem as FourSeasonsGalaxia).mainAttunement.tooltipColor;
                for (int i = 0; i <= 5; i++)
                {
                    Vector2 displace = Vector2.UnitX * 20 * Main.rand.NextFloat(-1f, 1f);
                    Particle Glow = new GenericBloom(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, 0.02f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i <= 10; i++)
                {
                    Vector2 displace = Vector2.UnitX * 16 * Main.rand.NextFloat(-1f, 1f);
                    Particle Sparkle = new GenericSparkle(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, particleColor, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                Initialized = 1f;
            }
        }

        public void Reattune(FourSeasonsGalaxia item)
        {
            Attunement attunement;
            Vector2[] StarPositions;
            Vector2[] ExtraLines; //Extra lines vector give us an indication of the index between the 2 stars we need to connect
            List<int> IgnoredLines = new List<int>();

            Particle Star;
            Particle Line;
            Color StarColor;

            if (CycleDirection == -1) //Cycles goes Phoenix => Aries => Polaris => Andromeda
            {
                switch (item.mainAttunement.id)
                {
                    case AttunementID.Phoenix: //Switching to the aries attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Aries];
                        break;
                    case AttunementID.Aries: //Switching to the polaris attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Polaris];
                        break;
                    case AttunementID.Polaris: //Switching to the andromeda attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Andromeda];
                        break;
                    case AttunementID.Andromeda: //Switching to the phoenix attunement.
                    default:
                        attunement = Attunement.attunementArray[(int)AttunementID.Phoenix];
                        break;
                }
            }
            else //Cycles goes Phoenix <= Aries <= Polaris <= Andromeda
            {
                switch (item.mainAttunement.id)
                {
                    case AttunementID.Phoenix: //Switching to the andromeda attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Andromeda];
                        break;
                    case AttunementID.Andromeda: //Switching to the polaris attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Polaris];
                        break;
                    case AttunementID.Polaris: //Switching to the aries attunement.
                        attunement = Attunement.attunementArray[(int)AttunementID.Aries];
                        break;
                    case AttunementID.Aries: //Switching to the phoenix attunement.
                    default:
                        attunement = Attunement.attunementArray[(int)AttunementID.Phoenix];
                        break;
                }
            }

            switch (attunement.id)
            {
                case AttunementID.Aries: // Drawing Aries
                    StarPositions = new Vector2[] { new Vector2(-160, -150), new Vector2(45, -170), new Vector2(137, 40), new Vector2(146, 126), new Vector2(129, 151) };
                    ExtraLines = new Vector2[] { };
                    StarColor = Color.Orchid;
                    break;
                case AttunementID.Polaris: //Drawing Ursa Minor
                    StarPositions = new Vector2[] { new Vector2(69, -188), new Vector2(18, -122), new Vector2(-23, -39), new Vector2(-13, 63), new Vector2(42, 147), new Vector2(-8, 184), new Vector2(-61, 83) };
                    ExtraLines = new Vector2[] { new Vector2(3, 6) };
                    StarColor = Color.CornflowerBlue;
                    break;
                case AttunementID.Andromeda: //Drawing Andromeda
                                             //https://media.discordapp.net/attachments/802291445360623686/934200685254291546/unknown.png
                    StarPositions = new Vector2[] {
                        new Vector2(-210, -46), new Vector2(-150, -35), new Vector2(-69, 18), new Vector2(33, 61), new Vector2(127, 72), //The horizontal line
                        new Vector2(-41, -27), new Vector2(-41, -67), new Vector2(-100, -124), new Vector2(-160, -130), //The first branch from the left
                        new Vector2(15, 147), new Vector2(35, 126), new Vector2(37, 23), new Vector2(67, -47), new Vector2(126, -109), new Vector2(146, -136), //The second branch from the left
                        new Vector2(95, -117)
                    };
                    ExtraLines = new Vector2[] { new Vector2(2, 5), new Vector2(13, 15) };
                    IgnoredLines.Add(5);
                    IgnoredLines.Add(9);
                    IgnoredLines.Add(15);
                    StarColor = Color.MediumSlateBlue;
                    break;
                case AttunementID.Phoenix: //Drawing Phoenix
                default:
                    StarPositions = new Vector2[] { new Vector2(-206, -99), new Vector2(-150, -43), new Vector2(-120, -146), new Vector2(-60, -71), new Vector2(-106, 71), new Vector2(-59, 138), new Vector2(116, -22),//The main line
                    new Vector2(246, -26),  new Vector2(192, 36), new Vector2(138, 81), //The side bit
                    new Vector2(88, -107), new Vector2(84, -75) }; //Complete the loop
                    ExtraLines = new Vector2[] { new Vector2(3, 10), new Vector2(11, 6) };
                    IgnoredLines.Add(10);
                    StarColor = Color.OrangeRed;
                    break;
            }

            //Only draw the cool effect if you have enough particle slots left. Itd look really ugly to have a half complete constellation
            if (GeneralParticleHandler.FreeSpacesAvailable() > StarPositions.Length * 2)
            {
                for (int i = 0; i < StarPositions.Length; i++)
                {
                    //The polar star gets bigger than the others. Also since andromeda has so many stars, make em smaller
                    Star = new GenericSparkle(Owner.Center + StarPositions[i], Vector2.Zero, Color.White, StarColor, (attunement.id == AttunementID.Polaris && i == 0) ? 3f : Main.rand.NextFloat(1f, 1.5f) * (attunement.id == AttunementID.Andromeda ? 0.8f : 1f), 20, 0f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);

                    if (i > 0 && !IgnoredLines.Contains(i))
                    {
                        Line = new BloomLineVFX(Owner.Center + StarPositions[i - 1], StarPositions[i] - StarPositions[i - 1], 0.5f, StarColor, 20, true);
                        GeneralParticleHandler.SpawnParticle(Line);
                    }
                }

                for (int i = 0; i < ExtraLines.Length; i++)
                {
                    Line = new BloomLineVFX(Owner.Center + StarPositions[(int)ExtraLines[i].Y], StarPositions[(int)ExtraLines[i].X] - StarPositions[(int)ExtraLines[i].Y], 0.5f, StarColor, 20, true);
                    GeneralParticleHandler.SpawnParticle(Line);
                }
            }

            //Chunger
            var Sound = SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ThunderStrike"), Projectile.Center);
            CalamityUtils.SafeVolumeChange(ref Sound, 0.4f);
            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;
            item.mainAttunement = attunement;
        }
    }
}
