using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class FearmongerGreathelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Greathelm");
            Tooltip.SetDefault("+60 max mana\n" + "15% increased minion damage and +2 max minions\n" + "Pure terror radiates from your eyes");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(gold: 75);
            item.defense = 38; // 132 total
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateEquip(Player player)
        {
            // Don't override the Yoraiz0r's Eye effect if the accessory itself is equipped
            if(player.yoraiz0rEye == 0)
                player.yoraiz0rEye = 3;

            player.statManaMax2 += 60;
            player.maxMinions += 2;
            player.minionDamage += 0.15f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FearmongerPlateMail>() && legs.type == ModContent.ItemType<FearmongerGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = @"45% increased minion damage
Minions deal full damage while wielding weaponry
Immunity to all forms of frost and flame
All minion attacks grant colossal life regeneration
Panic Necklace effect
15% increased damage reduction during the Pumpkin and Frost Moons
This extra damage reduction ignores the soft cap";

            // This bool encompasses cross-class nerf immunity, colossal life regen on minion attack, and the holiday moon DR
            player.Calamity().fearmongerSet = true;

            // All-class armors count as rogue sets, but don't grant stealth bonuses
            player.Calamity().wearingRogueArmor = true;
            player.minionDamage += 0.45f;
            player.panic = true;

            int[] immuneDebuffs = {
                BuffID.OnFire,
                BuffID.Frostburn,
                BuffID.CursedInferno,
                BuffID.ShadowFlame,
                BuffID.Daybreak,
                BuffID.Burning,
                ModContent.BuffType<Shadowflame>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<AbyssalFlames>(),
                ModContent.BuffType<HolyFlames>(),
                ModContent.BuffType<GodSlayerInferno>(),
                BuffID.Chilled,
                BuffID.Frozen,
                ModContent.BuffType<GlacialState>(),
            };
            for (int i = 0; i < immuneDebuffs.Length; ++i)
                player.buffImmune[immuneDebuffs[i]] = true;

            // Constantly emit dim orange light
            Lighting.AddLight(player.Center, 0.3f, 0.18f, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpookyHelmet);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 8);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddIngredient(ItemID.SoulofFright, 8);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}