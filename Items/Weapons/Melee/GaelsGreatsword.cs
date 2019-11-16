using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GaelsGreatsword : ModItem
    {
        //Help, they're forcing me to slave away at Calamity until I die! - Dominic

        public int SwingType;

        //Weapon attribute constants

        public static readonly int BaseDamage = 8;

        public static readonly int HardmodeDamage = 24;   

        public static readonly int PostMoonLordDamage = 270;

        public static readonly int PostYharonDamage = 10000;

        public static readonly float TrueMeleeBoostPreHardmode = 1.75f;

        public static readonly float TrueMeleeBoostHardmode = 2.5f;

        //Weapon projectile attribute constants

        public static readonly int BaseSearchDistance = 400;

        public static readonly int HardmodeSearchDistance = 660;

        public static readonly int PostMoonLordSearchDistance = 940;

        public static readonly int PostYharonSearchDistance = 1450;

        public static readonly int BaseImmunityFrames = 11;

        public static readonly int HardmodeImmunityFrames = 7;

        public static readonly int PostMoonLordImmunityFrames = 4;

        public static readonly int PostYharonImmunityFrames = 2;

        public static readonly int SkullsplosionCooldownSeconds = 10;

        public static readonly int PreMoonlordPenetrate = 3; //Infinite after Moon lord is dead

        //Skull ring attribute constants

        public static readonly float MaxRageBoost = 2.5f;

        public static readonly float RageBoostMultiplier = 1.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gael's Greatsword");
            Tooltip.SetDefault("Raises the player's damage reduction by 10% when wielding this sword\n" +
                               "There are three random attacks that can occur when the blade is swung\n" +
                               "The first attack releases two blood-skulls that home in\n" +
                               "The second attack releases an enormous slow-moving skull\n" +
                               "This slow-moving skull becomes weaker with time and lasts for around 4 seconds seconds\n" +
                               "The second attack causes a true melee strike with increased damage\n" +
                               "If you're below 50% life, swings release blood droplets\n" +
                               "Pressing the Rage Key while holding this weapon resets Rage to 0 and releases a circle of\n" +
                               "Slow-moving blood skulls. If Rage is full, the skulls do way more damage. Otherwise, the skull's damage is\n" +
                               "related to the amount of rage sacrificed\n" +
                               "This effect has a " + SkullsplosionCooldownSeconds + " second cooldown.\n" +
                               "The user can right click with the sword to deflect projectiles and enemy attacks 50% of the time");
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
            SwingType = Main.rand.Next(3);

            //Almost entirely Ultimus Cleaver code. Feel free to clean it up if you wish.
            if (player.whoAmI == Main.myPlayer &&
                player.statLife < player.statLifeMax * 0.5 &&
                player.Calamity().gaelBloodShotCooldown == 0)
            {
                float velocityY = 0f;
                float velocityX = 0f;
                float spawnAdditiveY = 0f;
                float spawnAdditiveX = 0f;
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.8))
                {
                    velocityY = -7f;
                }
                if (player.direction == -1)
                {
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                    {
                        spawnAdditiveX -= 8f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                    {
                        spawnAdditiveX -= 6f;
                    }
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                {
                    spawnAdditiveX = 26f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.6))
                {
                    velocityY = -6f;
                    velocityX = 2f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.4))
                {
                    velocityY = -4f;
                    velocityX = 4f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                {
                    spawnAdditiveX -= 4f;
                    spawnAdditiveY -= 20f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.2))
                {
                    velocityY = -2f;
                    velocityX = 6f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                {
                    velocityX = 7f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                {
                    spawnAdditiveY += 6f;
                }
                velocityY *= 1.35f;
                velocityX *= 1.35f;
                spawnAdditiveX *= (float)player.direction * Main.rand.NextFloat(0.9f, 1.1f);
                spawnAdditiveY *= player.gravDir;
                Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + spawnAdditiveX, (float)(hitbox.Y + hitbox.Height / 2) + spawnAdditiveY,
                    (float)player.direction * velocityX, velocityY * player.gravDir, ModContent.ProjectileType<GaelSpark>(), (int)((float)item.damage * 0.35f * player.meleeDamage), 0f, player.whoAmI, 0f, 0f);

                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                {
                    player.Calamity().gaelBloodShotCooldown = 30;
                }
            }
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            //True melee boost
            if (SwingType == 2)
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
            switch (SwingType)
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
                    int projectileIndex = Projectile.NewProjectile(position, new Vector2(speedX,speedY) * 0.5f, type, damage * 2, knockBack, player.whoAmI, ai1:1f);
                    Main.projectile[projectileIndex].scale = 1.75f;
                    break;
            }
            return false;
        }
    }
}
