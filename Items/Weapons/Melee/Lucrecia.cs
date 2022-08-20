using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee.Shortswords;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucrecia");
            Tooltip.SetDefault("Finesse\n" +
                "Striking an enemy makes you immune for a short time\n" +
                "Fires a DNA chain\n" +
                "Benefits 66% less from melee speed bonuses");
            SacrificeTotal = 1;
            ItemID.Sets.BonusAttackSpeedMultiplier[Item.type] = 0.33f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useTurn = false;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.width = 58;
            Item.height = 58;
            Item.damage = 90;
            Item.knockBack = 8.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LucreciaProj>();
            Item.shootSpeed = 2f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CoreofCalamity>().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
