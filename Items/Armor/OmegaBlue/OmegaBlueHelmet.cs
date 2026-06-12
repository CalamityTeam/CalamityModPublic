using System;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.OmegaBlue
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaBlueHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        internal static string TentacleEntitySourceContext => "SetBonus_Calamity_OmegaBlue";

        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/OmegaBlueAbility");

        public static float DamageBoost = 0.12f;
        public static int CritBoost = 14;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), CritBoost);

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 2;
        public static int TentacleDamage = 350;
        public static float MadnessDamageBoost = 0.1f;
        public static int MadnessCritBoost = 10;
        public static int MadnessDuration = CalamityUtils.SecondsToFrames(5);
        public static int MadnessCooldown = CalamityUtils.SecondsToFrames(25);

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/OmegaBlue/OmegaBlueHelmet_HeadMadness", EquipType.Head, name: "OmegaBlueTransformation");
            }
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ)
                return;
            var equipSlotHead = EquipLoader.GetEquipSlot(Mod, "OmegaBlueTransformation", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.defense = 15;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;

            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<OmegaBlueChestplate>() && legs.type == ModContent.ItemType<OmegaBlueTentacles>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().omegaBlueTransformation = true;
            player.Calamity().omegaBlueTransformationForce = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            Color AbilityBriefColor = Color.Lerp(new Color(255, 229, 61), new Color(110, 173, 237), 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 3f));
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, AbilityBriefColor.Hex3(), CalamityUtils.GetArmorSetBonusKey(), MadnessDuration.FramesToSeconds(), MadnessDamageBoost.ToPercent(), MadnessCooldown.FramesToSeconds());

            var mp = player.Calamity();
            player.maxMinions += SetBonusMinionSlotBoost;
            mp.wearingRogueArmor = true;
            mp.omegaBlueSet = true;
            mp.WearingPostMLSummonerSet = true;

            var hasOmegaBlueCooldown = mp.cooldowns.TryGetValue(Cooldowns.OmegaBlue.ID, out var cd);
            if (hasOmegaBlueCooldown && cd.timeLeft > 1500)
            {
                var d = Dust.NewDust(player.position, player.width, player.height, DustID.PurificationPowder, 0, 0, 100, Color.Transparent, 1.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ReaperTooth>(3).
                AddIngredient<DepthCells>(12).
                AddIngredient<RuinousSoul>().
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<OmegaBlueChestplate>()).
                Register();
        }
    }
}
