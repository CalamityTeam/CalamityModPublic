using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GildedProboscis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gilded Proboscis");
            Tooltip.SetDefault("Heals the player on hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.damage = 315;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 19;
            Item.knockBack = 8.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 66;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<GildedProboscisProj>();
            Item.shootSpeed = 13f;
            Item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
