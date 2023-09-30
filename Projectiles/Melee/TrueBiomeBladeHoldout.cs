using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueBiomeBladeHoldout : ModProjectile, ILocalizedModType //Visuals
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.ModItem as OmegaBiomeBlade).CanUseItem(Owner) : false;
        public bool OwnerMayChannel => Owner.itemAnimation == 0 && OwnerCanUseItem && Owner.Calamity().mouseRight && Owner.active && !Owner.dead;
        public ref float ChanneledState => ref Projectile.ai[0];
        public ref float ChannelTimer => ref Projectile.ai[1];
        public ref float Initialized => ref Projectile.localAI[0];

        private Item associatedItem;
        const int ChannelTime = 120;

        public override string Texture => "CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade";
        public bool drawIndrawHeldProjInFrontOfHeldItemAndArms = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
        }

        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.3f);
        public CurveSegment rise = new CurveSegment(EasingType.ExpIn, 0f, 0f, 1f);
        public CurveSegment overshoot = new CurveSegment(EasingType.SineBump, 0.80f, 1f, 0.1f);
        internal float SwordHeight() => PiecewiseAnimation(ChannelTimer / (float)ChannelTime, new CurveSegment[] { rise, overshoot });

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<OmegaBiomeBlade>())
                {
                    Projectile.Kill();
                    return;
                }

                if (Owner.whoAmI == Main.myPlayer)
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);

                associatedItem = Owner.HeldItem;
                //Switch up the attunements
                Attunement temporaryAttunementStorage = (associatedItem.ModItem as OmegaBiomeBlade).mainAttunement;
                (associatedItem.ModItem as OmegaBiomeBlade).mainAttunement = (associatedItem.ModItem as OmegaBiomeBlade).secondaryAttunement;
                (associatedItem.ModItem as OmegaBiomeBlade).secondaryAttunement = temporaryAttunementStorage;
                Initialized = 1f;
            }

            if (!OwnerMayChannel && ChanneledState == 0f) //IF the channeling gets interrupted for any reason
            {
                Projectile.Center = Owner.Top + new Vector2(18f, 0f);
                ChanneledState = 1f;
                Projectile.timeLeft = 60;
                return;
            }

            if (ChanneledState == 0f)
            {
                Owner.heldProj = Projectile.whoAmI;
                Owner.itemRotation = (-Vector2.UnitY).ToRotation();

                Projectile.Center = Owner.Top + new Vector2(0f, -20f * SwordHeight() - 50f);
                Projectile.rotation = -MathHelper.PiOver4; // No more silly turnaround with the repaired one?
                ChannelTimer++;
                Projectile.timeLeft = 60;

                if (ChannelTimer == ChannelTime - 15)
                {
                    Attune((OmegaBiomeBlade)associatedItem.ModItem);
                    Color particleColor = (associatedItem.ModItem as OmegaBiomeBlade).mainAttunement.tooltipColor;

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
                }

                if (ChannelTimer >= ChannelTime - 15)
                {
                    Vector2 Shake = Main.rand.NextVector2Circular(6f, 6f);
                    Projectile.Center += Shake;

                    Vector2 Bottom = Projectile.Center + Vector2.UnitY * 20f;
                    Particle Sparkle = new ElectricSpark(Bottom, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(10f, 20f), Color.White, Main.rand.NextBool() ? Main.rand.NextBool() ? Color.Goldenrod : Color.GreenYellow : Main.rand.NextBool() ? Color.Cyan : Color.Magenta, 1f + Main.rand.NextFloat(0f, 1f), 34, rotationSpeed: 0.1f, bloomScale: 4f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                if (ChannelTimer >= ChannelTime)
                {
                    Projectile.timeLeft = 60;
                    ChanneledState = 2f; //State where it stays invisible doing nothing. Acts as a cooldown
                }
            }

            if (ChanneledState == 1f)
                Projectile.position += Vector2.UnitY * -0.3f * (1f + Projectile.timeLeft / 60f);
        }

        public void Attune(OmegaBiomeBlade item)
        {
            bool jungle = Owner.ZoneJungle;
            bool snow = Owner.ZoneSnow;
            bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
            bool desert = Owner.ZoneDesert;
            bool hell = Owner.ZoneUnderworldHeight;
            bool ocean = Owner.ZoneBeach || Owner.Calamity().ZoneSulphur;
            bool hallow = Owner.ZoneHallow;
            bool astral = Owner.Calamity().ZoneAstral;
            bool marine = Owner.Calamity().ZoneAbyss || Owner.Calamity().ZoneSunkenSea;

            Attunement attunement = Attunement.attunementArray[(int)AttunementID.Whirlwind];
            if (desert || hell)
                attunement = Attunement.attunementArray[(int)AttunementID.SuperPogo];
            if (jungle || ocean || snow) //Check put after the desert check so ocean doesnt get overriden as desert
                attunement = Attunement.attunementArray[(int)AttunementID.FlailBlade];
            if (evil) //Evil check separated so that it overrides corrupted beach & snow biomes
                attunement = Attunement.attunementArray[(int)AttunementID.SuperPogo];
            if (astral || marine)
                attunement = Attunement.attunementArray[(int)AttunementID.Shockwave];
            if (hallow)
                attunement = Attunement.attunementArray[(int)AttunementID.Whirlwind]; //Putting holy check  at the end so it may override hallowed variants of biomes

            //If the owner already had the attunement , break out of it (And unswap)
            if (item.secondaryAttunement == attunement)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
                item.secondaryAttunement = item.mainAttunement;
                item.mainAttunement = attunement;
                return;
            }
            //Chunger
            var Sound = SoundEngine.PlaySound(CommonCalamitySounds.LightningSound with { Volume = CommonCalamitySounds.LightningSound.Volume * 0.4f }, Projectile.Center);

            Particle thunder = new ThunderBoltVFX(Projectile.Center + Vector2.UnitY * 20f, Main.rand.NextBool() ? Main.rand.NextBool() ? Color.Goldenrod : Color.GreenYellow : Main.rand.NextBool() ? Color.Cyan : Color.Magenta, 0f, 1.5f, Vector2.One, 1f, 15f, Projectile, 20f);
            GeneralParticleHandler.SpawnParticle(thunder);

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;

            item.mainAttunement = attunement;
        }

        public override void OnKill(int timeLeft)
        {
            if (associatedItem == null)
            {
                return;
            }
            //If we swapped out the main attunement for the second one despite the second attunement being empty at the time, unswap them.
            if ((associatedItem.ModItem as OmegaBiomeBlade).mainAttunement == null && (associatedItem.ModItem as OmegaBiomeBlade).secondaryAttunement != null)
            {
                (associatedItem.ModItem as OmegaBiomeBlade).mainAttunement = (associatedItem.ModItem as OmegaBiomeBlade).secondaryAttunement;
                (associatedItem.ModItem as OmegaBiomeBlade).secondaryAttunement = null;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {


            if (ChanneledState == 0f && ChannelTimer > 10f)
            {
                Texture2D tex = Request<Texture2D>(Texture).Value;
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, 1, 0, 0);

                return false;

            }
            else if (ChanneledState == 1f)
            {
                Texture2D tex = Request<Texture2D>(Texture).Value;
                Vector2 squishyScale = new Vector2(Math.Abs((float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * Projectile.timeLeft / 30f)), 1f);
                SpriteEffects flip = (float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * Projectile.timeLeft / 30f) > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition, null, lightColor * (Projectile.timeLeft / 60f), 0, tex.Size() / 2, squishyScale * (2f - (Projectile.timeLeft / 60f)), flip, 0);

                return false;
            }

            else
                return false;
        }
    }
}
