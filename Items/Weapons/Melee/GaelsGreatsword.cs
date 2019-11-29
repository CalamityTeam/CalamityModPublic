using CalamityMod.Projectiles.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GaelsGreatsword : ModItem
    {
        //Help, they're forcing me to slave away at Calamity until I die! - Dominic

        public int SwingType;

        //Weapon attribute constants

        public static readonly int BaseDamage = 8;

        public static readonly int HardmodeDamage = 24;   

        public static readonly int PostMoonLordDamage = 170;

        public static readonly int PostYharonDamage = 10000;

        public static readonly float TrueMeleeBoostPreHardmode = 1.75f;

        public static readonly float TrueMeleeBoostHardmode = 2.5f;

        public static readonly float GiantSkullDamageMultiplier = 1.5f;

        //Weapon projectile attribute constants

        public static readonly int BaseSearchDistance = 400;

        public static readonly int HardmodeSearchDistance = 660;

        public static readonly int PostMoonLordSearchDistance = 940;

        public static readonly int PostYharonSearchDistance = 1450;

        public static readonly int BaseImmunityFrames = 11;

        public static readonly int HardmodeImmunityFrames = 7;

        public static readonly int PostMoonLordImmunityFrames = 4;

        public static readonly int PostYharonImmunityFrames = 2;

        public static readonly int SkullsplosionCooldownSeconds = 30;

        public static readonly int PreMoonlordPenetrate = 3; //Infinite after Moon lord is dead

        //Skull ring attribute constants

        public static readonly float MaxRageBoost = 2.5f;

        public static readonly float RageBoostMultiplier = 1.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gael's Greatsword");
            Tooltip.SetDefault("First swing fires homing skulls\n" +
                               "Second swing fires a giant, powerful skull\n" +
                               "Third swing deals massive damage\n" +
                               "Constantly generates rage when in use\n" +
                               "[Swings leave behind exploding blood trails [Below 50% health]]\n" +
                               "[Right click to swipe the sword, reflecting projectiles at a 50% chance]\n" +
                               "[Activating Rage Mode releases an enormous barrage of skulls]\n" +
                               "Upgrades after killing the Wall of Flesh, Moon Lord, and Yharon\n" +
                               "'Give me that thing, your dark soul'");
        }
        //NOTE: GetWeaponDamage is in the CalamityPlayer file
        public override void SetDefaults()
        {
            item.width = 88;
            item.height = 84;
            item.damage = BaseDamage;
            item.melee = true;
            item.useAnimation = 16;
            item.useTime = 16;
            item.useTurn = true;
            item.knockBack = 9;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<GaelSkull>();
            item.shootSpeed = 15f;
            item.Calamity().postMoonLordRarity = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }
        public override void HoldItem(Player player)
        {
            player.endurance += 0.1f; //10% DR boost
        }
        public override bool AltFunctionUse(Player player) => true;
        public override Vector2? HoldoutOffset() => new Vector2(12, 12);
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useAnimation = 46;
                item.useTime = 46;
            }
            else
            {
                item.useAnimation = 12;
                item.useTime = 12;
            }
            return true;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<LightningThing>()) < 3 &&
                player.statLife <= player.statLifeMax2 * 0.5f)
            {
                Point origin = (player.Center + Main.rand.Next(-300, 301) * Vector2.UnitX).ToTileCoordinates();
                Point p;
                if (WorldUtils.Find(origin, Searches.Chain(new Searches.Down(400), new GenCondition[]
                {
                    new Conditions.IsSolid()
                }), out p))
                {
                    Projectile.NewProjectile(p.ToWorldCoordinates(8f, 0f), Vector2.Zero, ModContent.ProjectileType<LightningThing>(), 0, 0f, player.whoAmI);
                }
            }
            if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5))
            {
                player.Calamity().gaelSwipes++;
                if (player.Calamity().gaelSwipes > 3)
                {
                    player.Calamity().gaelSwipes = 0;
                }
                if (player.statLife <= player.statLifeMax2 * 0.5f)
                {
                    for (int i = 0; i < 170; i++)
                    {
                        float r = (float)Math.Sqrt(Main.rand.NextDouble());
                        float t = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 dustSpawn = t.ToRotationVector2() * r * item.Size;
                        if (dustSpawn.X > item.width / 2)
                        {
                            Dust.NewDustPerfect(player.MountedCenter + dustSpawn.RotatedBy(player.itemRotation) * player.direction, 218, Vector2.Zero).noGravity = true;
                        }
                        else
                        {
                            //Don't waste this version of "i" just because we failed. Decrease so that we can try again.
                            i--;
                            continue;
                        }
                        if (Main.rand.NextBool(100))
                        {
                            int damage = BaseDamage;
                            if (Main.hardMode)
                            {
                                damage = HardmodeDamage;
                            }
                            if (NPC.downedMoonlord)
                            {
                                damage = PostMoonLordDamage;
                            }
                            if (CalamityWorld.downedYharon)
                            {
                                damage = PostYharonDamage;
                            }
                            Projectile.NewProjectile(player.MountedCenter + dustSpawn.RotatedBy(player.itemRotation) * player.direction, Vector2.Zero, ModContent.ProjectileType<GaelExplosion>(), damage, 0f, player.whoAmI);
                        }
                    }
                }
            }
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            //True melee boost
            if (player.Calamity().gaelSwipes == 2)
            {
                damage = (int)((Main.hardMode ? TrueMeleeBoostHardmode : TrueMeleeBoostPreHardmode) * damage);
            }
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //This is mostly from a function in Player.cs which handles the Brand of the Inferno alt click effect
            //attackCD = The attack countdown. Can reflect attacks from projectiles and hostile npcs while > 0
            //shield_parry_cooldown = A simple count down variable for effects.
            if (player.altFunctionUse == 2)
            {
                bool shieldRaised = false;
                if (player.selectedItem != 58 && player.controlUseTile && !player.tileInteractionHappened && player.releaseUseItem && !player.controlUseItem && !player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC && !Main.SmartInteractShowingGenuine
                    && player.inventory[player.selectedItem].type == ModContent.ItemType<GaelsGreatsword>() && !player.mount.Active && (player.itemAnimation == 0 || PlayerInput.Triggers.JustPressed.MouseRight))
                {
                    shieldRaised = true;
                }
                if (player.shield_parry_cooldown > 0)
                {
                    player.shield_parry_cooldown--;
                    if (player.shield_parry_cooldown == 1)
                    {
                        Main.PlaySound(25, -1, -1, 1, 1f, 0f);
                        for (int i = 0; i < 10; i++)
                        {
                            int dustIndex = Dust.NewDust(player.Center + new Vector2((float)(player.direction * 6 + ((player.direction == -1) ? -10 : 0)), -14f), 10, 16, 45, 0f, 0f, 255, new Color(255, 100, 0, 127), (float)Main.rand.NextFloat(1f, 1.6f));
                            Main.dust[dustIndex].noLight = true;
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].velocity *= 0.5f;
                        }
                    }
                }
                if (shieldRaised != player.shieldRaised)
                {
                    player.shieldRaised = shieldRaised;
                    if (player.shieldRaised)
                    {
                        player.itemAnimation = 0;
                        player.itemTime = 0;
                        player.reuseDelay = 0;
                    }
                    else
                    {
                        player.shield_parry_cooldown = 15;
                        if (player.attackCD < 30)
                        {
                            player.attackCD = 30;
                        }
                    }
                }
                return false;
            }
            switch (player.Calamity().gaelSwipes)
            {
                //Two small, quick skulls
                case 0:
                    int numProj = 2;
                    float rotation = MathHelper.ToRadians(10f);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                    }
                    break;
                //Giant, slow, fading skull
                case 1:
					int largeSkullDmg = damage * 2;
					if (CalamityWorld.downedYharon)
					{
						largeSkullDmg = (int)((float)damage * 1.5f);
					}
					else if (NPC.downedMoonlord)
					{
						largeSkullDmg = damage * 2;
					}
					else if (Main.hardMode)
					{
						largeSkullDmg = damage * 2;
					}
                    int projectileIndex = Projectile.NewProjectile(position, new Vector2(speedX,speedY) * 0.5f, type, largeSkullDmg, knockBack, player.whoAmI, ai1:1f);
                    Main.projectile[projectileIndex].scale = 1.75f;
                    break;
            }
            return false;
        }
    }
}
