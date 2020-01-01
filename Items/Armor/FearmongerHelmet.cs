using CalamityMod.CalPlayer;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class FearmongerHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Helmet");
            Tooltip.SetDefault(@"+2 max minions and +50% minion knockback
Immunity to On Fire, Frostburn, Chilled, Frozen, Glacial State, and God Slayer Inferno");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.defense = 36; //139
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateEquip(Player player)
        {
			player.buffImmune[BuffID.Frozen] = true;
			player.buffImmune[BuffID.Chilled] = true;
			player.buffImmune[BuffID.Frostburn] = true;
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
			player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            player.maxMinions += 2;
            player.minionKB += 0.5f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FearmongerChestplate>() && legs.type == ModContent.ItemType<FearmongerGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = @"Boosted defensive stats during the Blood Moon, Pumpkin Moon, Frost Moon, and Solar Eclipse
Your horizontal movement is slowed
All player attacks deal 10% less damage
Your eyes glow with the essence of fear";

            player.Calamity().wearingRogueArmor = true; //all class
            player.Calamity().fearmonger = true;
			player.yoraiz0rEye = 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 11);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddIngredient(ItemID.SoulofFright, 8);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}