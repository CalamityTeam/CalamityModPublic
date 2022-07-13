using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("TheEmpyrean")]
    public class GodsBellows : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Bellows");
            Tooltip.SetDefault("70% chance to not consume gel");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 24;
            Item.useTime = 6;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(0, 95, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicFire>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 70)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
