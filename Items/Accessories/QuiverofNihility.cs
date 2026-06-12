using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class QuiverofNihility : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance<RangedDamageClass>() += 5;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidField = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidFieldGenerator>()] >= 4)
                    return;

                var source = player.GetSource_Accessory(Item);
                int count = player.ownedProjectileCounts[ModContent.ProjectileType<VoidFieldGenerator>()];
                do
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<VoidFieldGenerator>(), 0, 0f, Main.myPlayer, count);
                    count++;
                }
                while (count < 4);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyQuiver").
                AddIngredient(ItemID.FragmentVortex, 5).
                AddIngredient<DarkPlasma>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.55f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
