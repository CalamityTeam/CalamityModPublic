using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MutilatorSwordProj : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 180;
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<TheMutilator>()).Item;
        public override int AfterImageLength => 5;
        public override int OffsetDistance => 60;
        public override bool drawSwordTrail => true;
        public override Color[] trailColors => [Color.Red, Color.DarkRed, Color.Gold];

        public override float trailOffset => 20;

        public override float trailWidth(float completion, Vector2 vertexPos)
        {
            return base.trailWidth(completion, vertexPos);
        }

        public override int trailLength => 5;

        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }

        public override SoundStyle? UseSound => SoundID.Item1;


        public bool hasGivenBlood = false;

        public override void Spawn()
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 15;
            CooldownTime = 5;
            swingTime -= StartupTime + CooldownTime;
            modplayer.swingNum = 0;
        }

        public override void AdditionalAI()
        {
            if (inStartup)
            {
                Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            } else if (inCooldown)
            {
                Projectile.scale = baseScale * MathHelper.Lerp(1,0.5f,MathF.Pow(CooldownCompletion,2));
            } else 
                Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 2, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));
        }

        public override float SwingFunction()
        {
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, -swingWidth * 0.75f, 1-MathF.Pow(1-StartupCompletion,2f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.25f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth *0.75f, swingWidth *0.25f, SwingCompletion));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 60);
            var player = Main.player[Projectile.owner];
            var item = player.HeldItem;
            if (item.type != ModContent.ItemType<TheMutilator>())
            {
                Projectile.Kill();
                return;
            }
            var modItem = item.ModItem as TheMutilator;
            if (!hasGivenBlood)
            {
                modItem.Charge++;
                hasGivenBlood = true;
                if (modItem.Charge > TheMutilator.MaximumCharge)
                {
                    modItem.Charge = 0;
                    int orbAmount = 30;
                    if (orbAmount > 0)
                    {
                        float spreadAmount = MathHelper.ToRadians(360);
                        for (var i = 0; i < orbAmount; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, -angle.RotatedByRandom(spreadAmount) * 3f * Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<BloodstoneHealOrb>(), 20, 0f, player.whoAmI);

                        }
                        Particle bloodsplosion2 = new CustomPulse(target.Center, Vector2.Zero, (!ChildSafety.Disabled ? Color.CornflowerBlue : new Color(255, 32, 32)) * 0.75f, "CalamityMod/Particles/DustyCircleHardEdge", Vector2.One, Main.rand.NextFloat(-15f, 15f), 0.03f, 0.155f, 40);
                        GeneralParticleHandler.SpawnParticle(bloodsplosion2);
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BloodPactCrit") { Volume = 0.5f }, player.Center);
                    }
                }
                modItem.DecayTimer = 180;
            }
        }
    }
}
