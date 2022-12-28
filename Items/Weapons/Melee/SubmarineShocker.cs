using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee.Shortswords;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SubmarineShocker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Submarine Shocker");
            Tooltip.SetDefault("Enemies release electric sparks on hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useTurn = false;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 60;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<SubmarineShockerProj>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
    }
}
