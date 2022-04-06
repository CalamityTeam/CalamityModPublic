using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using System.Linq;

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
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 35, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 19;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;

            player.GetDamage<GenericDamageClass>() += 0.12f;
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
            string hotkey = CalamityKeybinds.SetBonusHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            player.setBonus = "Increases armor penetration by 15\n" +
                "10% increased damage and critical strike chance and +2 max minions\n" +
                "Short-ranged tentacles heal you by sucking enemy life\n" +
                "Press " + hotkey + " to activate abyssal madness for 5 seconds\n" +
                "Abyssal madness increases damage, critical strike chance, and tentacle aggression/range\n" +
                "This effect has a 25 second cooldown";

            CalamityPlayer mp = player.Calamity();
            player.armorPenetration += 15;
            player.maxMinions += 2;
            mp.wearingRogueArmor = true;
            mp.omegaBlueSet = true;
            mp.WearingPostMLSummonerSet = true;

            bool hasOmegaBlueCooldown = mp.cooldowns.TryGetValue(OmegaBlue.ID, out CooldownInstance cd);
            if (hasOmegaBlueCooldown && cd.timeLeft > 1500)
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
            CreateRecipe()
                .AddIngredient<ReaperTooth>(8)
                .AddIngredient<Lumenite>(5)
                .AddIngredient<Tenebris>(5)
                .AddIngredient<RuinousSoul>(2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
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
