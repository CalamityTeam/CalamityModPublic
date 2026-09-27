using System;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Armor.Wulfrum
{
    #region Armor pieces

    //Datsuzei / Moonstone armor from starlight river was a nice jumping off point for this kind of set.

    [AutoloadEquip(EquipType.Head)]
    [LegacyName("WulfrumHelmet")]
    [LegacyName("WulfrumHeadSummon")]
    public class WulfrumHat : ModItem, IExtendedHat, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        #region big hat
        public string ExtensionTexture => "CalamityMod/Items/Armor/Wulfrum/WulfrumHat_HeadExtension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => -Vector2.UnitY * 2f;
        public string EquipSlotName(Player drawPlayer) => drawPlayer.Male ? Name : "WulfrumHatFemale";
        #endregion

        public static readonly SoundStyle SetActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/WulfrumBastionActivate");
        public static readonly SoundStyle SetBreakSound = new("CalamityMod/Sounds/Custom/AbilitySounds/WulfrumBastionBreak");
        public static readonly SoundStyle SetBreakSoundSafe = new("CalamityMod/Sounds/Custom/AbilitySounds/WulfrumBastionBreakSafely");

        public static float SummonDamageBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int BastionDefenseBoost = 12;
        public static float BastionDRBoost = 0.1f;
        public static int BastionBuildTime = CalamityUtils.SecondsToFrames(0.55f);
        public static int BastionTime = CalamityUtils.SecondsToFrames(30);
        public static int TimeLostPerHit = CalamityUtils.SecondsToFrames(2);
        public static int BastionCooldown = CalamityUtils.SecondsToFrames(20);

        internal static Item DummyCannon = new Item(); //Used for the attack swap. Basically we force the player to hold a fake item.

        public static bool PowerModeEngaged(Player player, out CooldownInstance cd)
        {
            cd = null;
            bool hasWulfrumBastionCD = player.Calamity().cooldowns.TryGetValue(WulfrumBastion.ID, out cd);
            return (hasWulfrumBastionCD && cd.timeLeft > BastionCooldown);
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Wulfrum/WulfrumHat_FemaleHead", EquipType.Head, name: "WulfrumHatFemale");
            }

            On_Main.DrawPendingMouseText += SpoofMouseItem;
        }

        public override void Unload()
        {
            DummyCannon.TurnToAir();
            DummyCannon = null;
        }

        //Replaces the tooltip of the armor set with the fusion cannon if the player holds shift
        private void SpoofMouseItem(On_Main.orig_DrawPendingMouseText orig)
        {
            var player = Main.LocalPlayer;

            if (DummyCannon.IsAir && !Main.gameMenu)
                DummyCannon.SetDefaults(ItemType<WulfrumFusionCannon>());

            if (IsPartOfSet(Main.HoverItem) && HasArmorSet(player) && Main.keyState.PressingShift())
            {
                Main.HoverItem = DummyCannon.Clone();
                Main.hoverItemName = DummyCannon.Name;
            }

            orig();
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ItemType<WulfrumJacket>() && legs.type == ItemType<WulfrumOveralls>();
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<WulfrumHat>() && player.armor[1].type == ItemType<WulfrumJacket>() && player.armor[2].type == ItemType<WulfrumOveralls>();
        public bool IsPartOfSet(Item item) => item.type == ItemType<WulfrumHat>() ||
                item.type == ItemType<WulfrumJacket>() ||
                item.type == ItemType<WulfrumOveralls>();

        public override void UpdateArmorSet(Player player)
        {
            WulfrumArmorPlayer armorPlayer = player.GetModPlayer<WulfrumArmorPlayer>();
            WulfrumTransformationPlayer transformationPlayer = player.GetModPlayer<WulfrumTransformationPlayer>();

            armorPlayer.wulfrumSet = true;

            Color AbilityBriefColor = Color.Lerp(new Color(194, 255, 67), new Color(112, 244, 244), 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 3f));
            player.setBonus = this.GetLocalization("SetBonus").Format(AbilityBriefColor.Hex3(), CalamityUtils.GetArmorSetBonusKey(), BastionTime.FramesToSeconds(), TimeLostPerHit.FramesToSeconds());

            if (PowerModeEngaged(player, out var cd))
            {
                if (cd.timeLeft == BastionCooldown + BastionTime)
                {
                    ActivationEffects(player);
                }

                //Stats
                player.statDefense += BastionDefenseBoost;
                player.endurance += BastionDRBoost;

                //Can't account for previous fullbody transformations but at this point, whatever
                bool transformed = player.Transformation().Type == ItemType<AbandonedWulfrumHelmet>();
                Item headItem = player.armor[10].type != ItemID.None ? player.armor[10] : player.armor[0];
                bool hatVisible = !transformed && headItem.type == ItemType<WulfrumHat>();

                //Spawn the hat
                if (cd.timeLeft == BastionCooldown + BastionTime - (int)(BastionBuildTime * 0.9f) && hatVisible)
                {
                    Particle leftoverHat = new WulfrumHatParticle(player, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 7f), 25);
                    GeneralParticleHandler.SpawnParticle(leftoverHat);
                }

                //Visuals
                if (cd.timeLeft < BastionCooldown + BastionTime - BastionBuildTime)
                    player.Transformation().Type = ItemType<AbandonedWulfrumHelmet>();

                //Swapping the arm
                if (DummyCannon.IsAir)
                    DummyCannon.SetDefaults(ItemType<WulfrumFusionCannon>());

                if (Main.myPlayer == player.whoAmI)
                {
                    //Drop the player's held item if they were holding something before
                    if (!(Main.mouseItem.type == DummyCannon.type) && !Main.mouseItem.IsAir)
                        Main.LocalPlayer.QuickSpawnItem(null, Main.mouseItem, Main.mouseItem.stack);

                    Main.mouseItem = DummyCannon;
                }

                //Slot 58 is the "fake" slot thats used for the item the player is holding in their mouse
                player.inventory[58] = DummyCannon;
                player.selectedItem = 58;
            }

            else if (Main.myPlayer == player.whoAmI)
            {
                //Clear the player's hand
                if (Main.mouseItem.type == ItemType<WulfrumFusionCannon>())
                    Main.mouseItem = new Item();

                DummyCannon.TurnToAir();

                if (player.Transformation().Type == ItemType<AbandonedWulfrumHelmet>() && player.Transformation().currentTransformation.IsForced)
                    player.Transformation().currentTransformation.IsForced = false;
            }
        }

        public void ActivationEffects(Player player)
        {
            SoundEngine.PlaySound(SetActivationSound);

            //Don't do the effect if the player is already using the wulfrum vanity lol
            bool transformedAlready = player.Transformation().Type == ItemType<AbandonedWulfrumHelmet>();

            if (!transformedAlready)
            {
                player.controlUseItem = false;
                player.controlUseTile = false;
                player.controlThrow = false;
                for (int i = 0; i < 5; i++)
                {
                    Particle part = new WulfrumBastionPartsParticle(player, i, BastionBuildTime + 2);
                    GeneralParticleHandler.SpawnParticle(part);
                }
            }

            //Do spawn the cannon always though
            Particle gun = new WulfrumBastionPartsParticle(player, 5, BastionBuildTime + 2);

            if (transformedAlready)
            {
                (gun as WulfrumBastionPartsParticle).TimeOffset = 0;
                (gun as WulfrumBastionPartsParticle).AnimationTime = BastionBuildTime + 2;
            }
            GeneralParticleHandler.SpawnParticle(gun);
        }

        public override void UpdateEquip(Player player) => player.GetDamage<SummonDamageClass>() += SummonDamageBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(10).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    [LegacyName("WulfrumArmor")]
    public class WulfrumJacket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static int MinionSlotBoost = 1;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost);

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                var equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
                ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player) => player.maxMinions += MinionSlotBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(20).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    [LegacyName("WulfrumLeggings")]
    public class WulfrumOveralls : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static float SummonDamageBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;

            if (!Main.dedServ)
            {
                var equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlot] = true;
            }
        }

        public override void UpdateEquip(Player player) => player.GetDamage<SummonDamageClass>() += SummonDamageBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(15).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
    #endregion

    public class WulfrumArmorPlayer : ModPlayer
    {
        public bool wulfrumSet = false;

        public override void ResetEffects()
        {
            wulfrumSet = false;
        }

        public override void UpdateDead()
        {
            wulfrumSet = false;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (WulfrumHat.PowerModeEngaged(Player, out var cd) && !Main.dedServ)
            {
                SetBonusEndEffect(true);
                if (!Player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquipped)
                    Player.Transformation().currentTransformation = null;
            }
        }

        public override void PostUpdate()
        {
            if (wulfrumSet && Player.Calamity().cooldowns.TryGetValue(WulfrumBastion.ID, out var cd) && cd.timeLeft == WulfrumHat.BastionCooldown)
            {
                if (!Main.dedServ)
                    SetBonusEndEffect(false);
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (WulfrumHat.PowerModeEngaged(Player, out var cd))
            {
                cd.timeLeft -= WulfrumHat.TimeLostPerHit;
                if (cd.timeLeft < WulfrumHat.BastionCooldown)
                {
                    cd.timeLeft = WulfrumHat.BastionCooldown - 1;
                    if (!Main.dedServ)
                        SetBonusEndEffect(true);
                }
            }
        }

        public void SetBonusEndEffect(bool violent)
        {
            SoundStyle breakSound = WulfrumHat.SetBreakSoundSafe;
            float goreSpeed = 3f;
            int goreCount = 4;
            int goreIncrement = 2;

            if (violent)
            {
                breakSound = WulfrumHat.SetBreakSound;
                goreSpeed = 9f;
                goreCount = 9;
                goreIncrement = 1;
            }

            SoundEngine.PlaySound(breakSound, Player.Center);
            //Only spawn the cannon gore if the player already has the vanity on.
            if (Player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquipped)
            {
                Vector2 shrapnelVelocity = Main.rand.NextVector2Circular(goreSpeed, goreSpeed);
                Gore.NewGore(Player.GetSource_Death(), Player.Center, shrapnelVelocity, Mod.Find<ModGore>("WulfrumPowerSuit1").Type);
            }

            else
            {
                int j = 1;

                for (int i = 1; i < goreCount; i++)
                {
                    Vector2 shrapnelVelocity = Main.rand.NextVector2Circular(goreSpeed, goreSpeed);
                    string goreType = "WulfrumPowerSuit" + j.ToString();
                    Gore.NewGore(Player.GetSource_Death(), Player.Center, shrapnelVelocity, Mod.Find<ModGore>(goreType).Type);

                    j += Main.rand.Next(1, goreIncrement);
                }
            }
        }


        public override void PostUpdateMiscEffects()
        {
            //This is important. Prevents ppl cheat sheeting the item in from softlocking their mouse button lol
            if (Main.mouseItem.ModItem is WulfrumFusionCannon && !WulfrumHat.PowerModeEngaged(Player, out _))
            {
                Main.mouseItem.TurnToAir();
            }

            //This shouldn't ever be possible since the power mode prevents you from using or moving items around
            if (!wulfrumSet && WulfrumHat.PowerModeEngaged(Player, out var cd))
            {
                cd.timeLeft = WulfrumHat.BastionCooldown;
            }
        }

        public override void FrameEffects()
        {
            //Give the braids variant to w*men
            if (!Player.Male && Player.head == EquipLoader.GetEquipSlot(Mod, "WulfrumHat", EquipType.Head))
                Player.head = EquipLoader.GetEquipSlot(Mod, "WulfrumHatFemale", EquipType.Head);
        }
    }
}
