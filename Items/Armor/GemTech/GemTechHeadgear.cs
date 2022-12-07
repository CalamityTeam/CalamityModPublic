using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Microsoft.Xna.Framework.Input.Keys;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Armor.GemTech
{
    [AutoloadEquip(EquipType.Head)]
    public class GemTechHeadgear : ModItem
    {
        public const int GemBreakDamageLowerBound = 100;
        public const int GemDamage = 40000;
        public const int GemDamageSoftcapThreshold = 100000;
        public const int GemRegenTime = 1800;

        public const int MeleeShardBaseDamage = 825;
        public const int MeleeShardDelay = 330;
        public const float MeleeDamageBoost = 0.45f;
        public const float MeleeCritBoost = 0.12f;
        public const float MeleeSpeedBoost = 0.26f;

        public const int MaxFlechettes = 8;
        public const float RangedDamageBoost = 0.5f;
        public const float RangedCritBoost = 0.16f;

        public const int MagicManaBoost = 100;
        public const int NonMagicItemManaRegenBoost = 8;
        public const float MagicDamageBoost = 0.5f;
        public const float MagicCritBoost = 0.16f;

        public const int SummonMinionCountBoost = 4;
        public const float SummonDamageBoost = 0.72f;

        public const int RogueStealthBoost = 130;
        public const float RogueDamageBoost = 0.5f;
        public const float RogueCritBoost = 0.16f;

        public const int BaseGemDefenseBoost = 75;
        public const int BaseGemLifeRegenBoost = 2;
        public const float BaseGemDRBoost = 0.06f;
        public const float BaseGemMovementSpeedBoost = 0.4f;
        public const float BaseGemJumpSpeedBoost = 0.4f;

        public const int AllGemsWeaponUseLifeRegenBoost = 2;
        public const int AllGemsMultiWeaponUseLifeRegenBoost = 3;
        public const int AllGemsLifeRegenBoostTime = 480;
        public const int AllGemsMultiWeaponLifeRegenBoostTime = 150;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Gem Tech Headgear");
            Tooltip.SetDefault("The Devil said: Revel in your victory; You've earned your damning. Pack your things and leave.");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 32;
            Item.defense = 14;

            // Exact worth of the armor piece's constituents.
            Item.value = Item.sellPrice(platinum: 6, gold: 14, silver: 88);
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GemTechBodyArmor>() && legs.type == ModContent.ItemType<GemTechSchynbaulds>();
        }
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<GemTechHeadgear>() && player.armor[1].type == ItemType<GemTechBodyArmor>() && player.armor[2].type == ItemType<GemTechSchynbaulds>();

        public override void UpdateArmorSet(Player player)
        {
            player.Calamity().GemTechSet = true;
            player.Calamity().wearingRogueArmor = true;
            if (player.Calamity().GemTechState.IsRedGemActive)
                player.Calamity().rogueStealthMax += RogueStealthBoost * 0.01f;

            player.setBonus = "Mucho Texto";
        }

        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    tooltips[setBonusIndex].Text = "Power Gems - Six gem fragments idly orbit you; one for each class, and a base gem";
                        tooltips[setBonusIndex].OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, 0.3f);


                    if (!Main.keyState.IsKeyDown(LeftShift))
                    {
                        TooltipLine briefDescription = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", 
                            "Each active gem provides a bonus for its respective class, while the defensive gem grants defensive boosts\n" +
                            "Powerful enemy hits will dislodge gems, launching them into the nearest enemy for huge damage\n" +
                            "Lost gems regenerate after a while\n" +
                            "The lost gem is the same class as the weapon you are using, so better prepare for some weapon switching action!");
                        briefDescription.OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, 0.5f);
                        tooltips.Insert(setBonusIndex + 1, briefDescription);

                        TooltipLine itemDisplay = new TooltipLine(item.Mod, "CalamityMod:ExpandedDisplay", "Hold SHIFT to see an expanded description");
                        itemDisplay.OverrideColor = new Color(190, 190, 190);
                        tooltips.Add(itemDisplay);
                    }

                    else
                    {
                        setBonusIndex++;
                        TooltipLine largerDescription = new TooltipLine(item.Mod, "CalamityMod:SetBonus1",
                            $"A gem is lost when you take more than { GemBreakDamageLowerBound } damage in a single hit. The type of gem lost is the same as the class of the previous when you used\n" +
                            "If said gem has already been lost, the base gem is lost instead\n" +
                            $"When a gem is lost, it breaks off and homes towards the nearest enemy or boss, if one is present, dealing a base of {GemDamage} damage\n" +
                            $"Gems have a {GemRegenTime / 60} second delay before they appear again");
                        largerDescription.OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, 0.5f);
                        tooltips.Insert(setBonusIndex, largerDescription);

                        setBonusIndex++;
                        TooltipLine redGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus2", "[i:" + ItemID.Ruby + "]" + $"The red gem grants { RogueStealthBoost } maximum stealth, increased rogue stats, and makes stealth only consumable by rogue weapons");
                        redGemTooltip.OverrideColor = new Color(224, 24, 0);
                        tooltips.Insert(setBonusIndex, redGemTooltip);

                        setBonusIndex++;
                        TooltipLine yellowGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus3", "[i:" + ItemID.Topaz + "]" + $"The yellow gem provides increased melee stats and makes melee attacks release shards on hit with a cooldown. This cooldown is shorter for true melee attacks");
                        yellowGemTooltip.OverrideColor = new Color(237, 170, 43);
                        tooltips.Insert(setBonusIndex, yellowGemTooltip);

                        setBonusIndex++;
                        TooltipLine greenGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus4", "[i:" + ItemID.Emerald + "]" + $"The green gem provides increased ranged stats and causes flechettes to fly swiftly towards targets when they are damaged by a ranged projectile");
                        greenGemTooltip.OverrideColor = new Color(37, 188, 108);
                        tooltips.Insert(setBonusIndex, greenGemTooltip);

                        setBonusIndex++;
                        TooltipLine blueGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus5", "[i:" + ItemID.Sapphire + "]" + $"The blue gem grants {SummonMinionCountBoost} extra maximum minions, increased minion damage, and reduces the penalty for summoner items while holding a non-summoner weapon");
                        blueGemTooltip.OverrideColor = new Color(37, 119, 206);
                        tooltips.Insert(setBonusIndex, blueGemTooltip);

                        setBonusIndex++;
                        TooltipLine purpleGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus6", "[i:" + ItemID.Amethyst + "]" + $"The violet gem grants {MagicManaBoost} extra maximum mana, increased magic stats, and makes mana rapidly regenerate when holding a non-magic weapon");
                        purpleGemTooltip.OverrideColor = new Color(200, 58, 209);
                        tooltips.Insert(setBonusIndex, purpleGemTooltip);

                        setBonusIndex++;
                        TooltipLine pinkGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus7", "[i:" + ItemID.Diamond + "]" + $"The pink base gem grants {BaseGemDefenseBoost} extra defense, extra damage reduction, increased movement speed, jump speed, and +{BaseGemLifeRegenBoost} life regen");
                        pinkGemTooltip.OverrideColor = new Color(255, 115, 206);
                        tooltips.Insert(setBonusIndex, pinkGemTooltip);

                        setBonusIndex++;
                        TooltipLine liferegenTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus8", 
                            $"When all gems exist simultaneously, hitting a target with any weapon grants you +{AllGemsWeaponUseLifeRegenBoost} life regen for {AllGemsLifeRegenBoostTime / 60} seconds\n" +
                            $"This is increased to +{AllGemsMultiWeaponUseLifeRegenBoost} life regen if a weapon of another class is used during that {AllGemsLifeRegenBoostTime / 60} second period for {AllGemsMultiWeaponLifeRegenBoostTime / 60f} seconds");
                        liferegenTooltip.OverrideColor = new Color(230, 230, 230);
                        tooltips.Insert(setBonusIndex, liferegenTooltip);
                    }
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => ModifySetTooltips(this, tooltips);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExoPrism>(10).
                AddIngredient<GalacticaSingularity>(3).
                AddIngredient<CoreofCalamity>(2).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
