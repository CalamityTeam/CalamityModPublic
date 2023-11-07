using System;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DragonsBreath : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        
        public static readonly SoundStyle FireballSound = new("CalamityMod/Sounds/Custom/Yharon/YharonFireball", 3) { PitchVariance = 0.3f, Volume = 0.75f };
        public static readonly SoundStyle WeldingBurn = new("CalamityMod/Sounds/Item/WeldingBurn") { Volume = 0.65f };
        public static readonly SoundStyle WeldingShoot = new("CalamityMod/Sounds/Item/WeldingShoot") { Volume = 0.45f };

        public SlotId WeldSoundSlot;

        public int BetweenShotsPause = 14;
        public float Counter = 2;
        public float MaxFirerateShots = 20;
        public float WeldingShots = 50;
        public bool StrongShotMode = false;
        public int DragonsBreathSetUseTime = 5;
        public int DragonsBreathSetUseAnimation = 9;

        public override void SetDefaults()
        {
            Item.damage = 493;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 94;
            Item.height = 72;
            Item.useTime = DragonsBreathSetUseTime;
            Item.useAnimation = DragonsBreathSetUseAnimation;
            Item.reuseDelay = BetweenShotsPause;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType<DragonsBreathFlames>();
            Item.shootSpeed = 3.5f;
            Item.useAmmo = AmmoID.Gel;

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => !StrongShotMode && Main.rand.NextFloat() > 0.80f;

        public override Vector2? HoldoutOffset() => new Vector2(18, 10.5f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!StrongShotMode)
            {
                if (Counter == 2)
                    SoundEngine.PlaySound(FireballSound, player.Center);
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
                Projectile.NewProjectile(source, position, newVel * Main.rand.NextFloat(1.2f, 0.8f), type, damage, knockback, player.whoAmI);
                Counter--;
                if (MaxFirerateShots > 0 && BetweenShotsPause == 4)
                    MaxFirerateShots--;

                if (Counter <= 0)
                {
                    if (BetweenShotsPause > 4)
                    {
                        BetweenShotsPause -= 1;
                        Item.reuseDelay = BetweenShotsPause;
                    }
                    Counter = 2;
                }
                if (BetweenShotsPause <= 4 && MaxFirerateShots == 0)
                {
                    BetweenShotsPause = 0;
                    Item.reuseDelay = BetweenShotsPause;
                    StrongShotMode = true;
                    DragonsBreathSetUseTime = 2;
                    DragonsBreathSetUseAnimation = 2;
                    SoundEngine.PlaySound(ScorchedEarth.ShootSound, player.Center);
                }
            }
            else if (StrongShotMode)
            {
                if (SoundEngine.TryGetActiveSound(WeldSoundSlot, out var WeldSound) && WeldSound.IsPlaying)
                    WeldSound.Position = player.Center;
                if (player.Calamity().DragonsBreathAudioCooldown2 == 0)
                {
                    player.Calamity().DragonsBreathAudioCooldown2 = 30;
                    WeldSoundSlot = SoundEngine.PlaySound(WeldingShoot, player.Center);
                }
                Projectile.NewProjectile(source, position, velocity * 1.5f, ModContent.ProjectileType<DragonsBreathFlames>(), damage, knockback, player.whoAmI, 1);
                WeldingShots--;
                if (WeldingShots <= 0)
                {
                    Counter = 2;
                    BetweenShotsPause = 14;
                    Item.reuseDelay = BetweenShotsPause;
                    SoundEngine.PlaySound(SpeedBlaster.Empty, player.Center);
                    Projectile.NewProjectile(source, player.Center, new Vector2(5 * -player.direction, -5), ModContent.ProjectileType<DragonsBreathMag>(), Main.zenithWorld ? 250000 : 1, knockback, player.whoAmI);
                    MaxFirerateShots = 20;
                    WeldingShots = 50;
                    WeldSound?.Stop();
                    StrongShotMode = false;
                    DragonsBreathSetUseTime = 5;
                    DragonsBreathSetUseAnimation = 9;
    }
            }
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(94, 72);
            Vector2 itemOrigin = new Vector2(-17, 3);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += StrongShotMode && WeldingShots == 1 ? -0.25f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction : 0;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
