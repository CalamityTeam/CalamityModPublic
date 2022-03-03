using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Buffs.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
    public class OmegaBlueHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Blue Helmet");
            Tooltip.SetDefault(@"You can move freely through liquids
12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 35, 0, 0);
            item.rare = ItemRarityID.Red;
            item.defense = 19;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;

            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(8);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<OmegaBlueChestplate>() && legs.type == ModContent.ItemType<OmegaBlueLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
			player.Calamity().omegaBlueTransformation = true;
            player.Calamity().omegaBlueTransformationForce = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "Increases armor penetration by 15\n" +
				"10% increased damage and critical strike chance and +2 max minions\n" +
				"Short-ranged tentacles heal you by sucking enemy life\n" +
				"Press " + hotkey + " to activate abyssal madness for 5 seconds\n" +
				"Abyssal madness increases damage, critical strike chance, and tentacle aggression/range\n" +
				"This effect has a 25 second cooldown";

            player.armorPenetration += 15;
            player.Calamity().wearingRogueArmor = true;

			player.maxMinions += 2;

			//raise rev caps
			player.Calamity().omegaBlueSet = true;

            if (player.Calamity().Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(OmegaBlueCooldown) && cooldown.TimeLeft > 1500))
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 1.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class OmegaBlueTransformationHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }
}
