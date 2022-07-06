using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using Terraria.Audio;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem
    {
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };


        public static int ProtectionMatrixDurabilityMax = 50;
        public static int ProtectionMatrixRechargeTime = 60 * 10;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rover Drive");
            Tooltip.SetDefault("Activates a protective shield that grants 15 defense for 10 seconds\n" +
            //Actually 10.1 seconds at full power with a dissipation across 0.1666 seconds but whatever
            "The shield then dissipates and recharges for 20 seconds before being reactivated\n" +
                "Can also be scrapped at an extractinator");

            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;

            //Needed for extractination
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.roverDrive = true;

            player.GetModPlayer<RoverDrivePlayer>().RoverDriveOn = true;
        }

        //Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
        {
            resultType = ModContent.ItemType<WulfrumMetalScrap>();
            resultStack = Main.rand.Next(3, 6);

            if (Main.rand.NextFloat() > 0.8f)
            {
                resultStack = 1;
                resultType = ModContent.ItemType<EnergyCore>();
            }
        }
    }

    public class RoverDrivePlayer : ModPlayer
    {
        public bool RoverDriveOn;
        public int ProtectionMatrixDurability = 0;
        public int ProtectionMatrixCharge = 0;

        public override void ResetEffects()
        {
            //Turn this into armor health when we can
            if (RoverDriveOn)
                Player.statLifeMax2 += ProtectionMatrixDurability;

            else
                ProtectionMatrixDurability = 0;

            RoverDriveOn = false;
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (RoverDriveOn)
            {
                if (ProtectionMatrixDurability > 0)
                {
                    ProtectionMatrixDurability -= (int)damage;
                    if (ProtectionMatrixDurability <= 0)
                    {
                        ProtectionMatrixDurability = 0;

                        //Switch CD
                        //Sound effect of engine shutting down
                    }
                }

                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability))
                {
                    cdDurability.timeLeft = ProtectionMatrixDurability;
                }


                //Reset recharge time.
                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cd))
                {
                    cd.timeLeft = RoverDrive.ProtectionMatrixRechargeTime;
                }
            }
        }

        public override void UpdateDead()
        {
            ProtectionMatrixDurability = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            if (!RoverDriveOn)
            {
                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability) && !RoverDriveOn)
                    cdDurability.timeLeft = 0;

                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cdRecharge) && !RoverDriveOn)
                    cdRecharge.timeLeft = 0;
            }
            
            else
            {
                if (ProtectionMatrixDurability == 0 && !Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cd))
                {
                    Player.AddCooldown(WulfrumRoverDriveRecharge.ID, RoverDrive.ProtectionMatrixRechargeTime);
                }

                if (ProtectionMatrixDurability > 0 && !Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out cd))
                {
                    CooldownInstance durabilityCooldown = Player.AddCooldown(WulfrumRoverDriveDurability.ID, RoverDrive.ProtectionMatrixDurabilityMax);
                    durabilityCooldown.timeLeft = ProtectionMatrixDurability;

                    SoundEngine.PlaySound(RoverDrive.ActivationSound, Player.Center);
                }

                if (ProtectionMatrixDurability > 0)
                {

                    Player.Calamity().roverDrive = true;
                    Player.Calamity().roverDriveTimer = 2;
                }

                else
                {
                    Player.Calamity().roverDriveTimer = 618;
                    Player.Calamity().roverDrive = false;
                }
            }
        }
    }
}
