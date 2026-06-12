using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Grax : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        private const int HammerPower = 110;
        private const int AxePower = 180 / 5;

        public static float DamageBoost = 0.15f;
        public static int DefenseBoost = 15;
        public static int GraxBoostDuration = CalamityUtils.SecondsToFrames(15);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), DefenseBoost, GraxBoostDuration.FramesToSeconds());
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.damage = 472;
            Item.knockBack = 8f;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.hammer = HammerPower;
            Item.axe = AxePower;
            Item.tileBoost += 5;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            //TODO - Pressure the tmod devs into adding another method akin to ModifyToolStats that gets ran early enough to work.
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
                AddIngredient<UelibloomBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<GraxBoost>(), GraxBoostDuration);
        }
    }
}
