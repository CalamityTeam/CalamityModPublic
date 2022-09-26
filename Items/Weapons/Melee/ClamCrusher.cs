using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ClamCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clam Crusher");
            Tooltip.SetDefault("Launches a huge clam that stuns enemies for a short amount of time\n" +
                               "Starts being affected by gravity and does much more damage after being airborne for a while");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 40;
            Item.height = 60;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<ClamCrusherFlail>();
            Item.shootSpeed = 18f;
        }
    }
}
