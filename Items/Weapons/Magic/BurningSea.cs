using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BurningSea : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public const float ChargeTime = 240f;
        public const float BurnOutTime = 560f;
        public const int BurnOutReuseDelay = 150;
        public const float FizzleOutTime = 40f;

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
            CalamityItemSets.ExtraDebuffTooltip_Player[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 56;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IncineratingFireball>();
            Item.shootSpeed = 5f;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.Calamity().burningSeaBurnOut <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<UnholyCore>(5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
