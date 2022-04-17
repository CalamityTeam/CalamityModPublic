using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Tools
{
    public class Grax : ModItem
    {
        private const int HammerPower = 110;
        private const int AxePower = 180 / 5;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Grax");
            Tooltip.SetDefault("Hitting an enemy will greatly boost your defense, melee damage and melee crit for a short time\n" +
                "Right click to use without hammering down walls or chopping down trees");
        }

        public override void SetDefaults()
        {
            Item.damage = 472;
            Item.knockBack = 8f;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.hammer = HammerPower;
            Item.axe = AxePower;
            Item.tileBoost += 5;

            Item.width = 62;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.axe = 0;
                Item.hammer = 0;
            }
            else
            {
                Item.axe = AxePower;
                Item.hammer = HammerPower;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfernaCutter>().
                AddRecipeGroup("LunarHamaxe").
                AddIngredient<MolluskHusk>(10).
                AddIngredient<DraedonBar>(5).
                AddIngredient<UeliaceBar>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GraxDefense>(), 600);
        }
    }
}
