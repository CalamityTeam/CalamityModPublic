using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonRage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Rage");
            Tooltip.SetDefault("Twirls a baton that causes explosions on enemy hits\n" +
            "Every ten hits will summon a ring of fireballs");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1275;
            Item.knockBack = 7.5f;
            Item.useAnimation = Item.useTime = 25;
            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<DragonRageStaff>();

            Item.width = 128;
            Item.height = 140;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }
    }
}
