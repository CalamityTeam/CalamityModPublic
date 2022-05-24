using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StormRuler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Ruler");
            Tooltip.SetDefault("Only a storm can fell a greatwood\n" +
                "Fires beams that generate tornadoes on death\n" +
                "Tornadoes suck enemies in");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 82;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<StormRulerProj>();
            Item.shootSpeed = 20f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 187, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormSaber>().
                AddIngredient<WindBlade>().
                AddIngredient<CoreofCinder>(3).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
