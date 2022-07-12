using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("UrchinSpear")]
    public class RedtideSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redtide Spear");
            Tooltip.SetDefault("Poisons enemies on hit\n"+
                               "Aiming the spear in front of you while running holds it down with increased knockback\n"+
                               "Releasing the attack button after the charge makes an upwards slash, sending enemies flying into the air\n"+
                               //Lore tooltip time. Dark souls.
                               "[c/5C95A1:The people of the sea were adept hunters, but they abhorred unnecessary violence.]\n" +
                               "[c/5C95A1:Unfortunately, neighboring nations were brutish, so they fashioned their tools for war.]");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.damage = 33;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.useTime = 25;
            Item.knockBack = 5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<RedtideSpearProjectile>();
            Item.shootSpeed = 4f;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(4).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class SpearChargePlayer : ModPlayer
    {
        public bool ChargingKnockbackResist = false;

        public override void PostUpdateMiscEffects()
        {
            if (ChargingKnockbackResist)
            {
                Player.noKnockback = true;
                ChargingKnockbackResist = false;
            }
        }
    }
}
