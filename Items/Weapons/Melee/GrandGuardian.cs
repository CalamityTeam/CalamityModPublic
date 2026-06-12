using CalamityMod.Items.BaseItems;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GrandGuardian : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 130;
            Item.damage = 360;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.useTurn = true;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;

            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<GrandGuardianHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GrandGuardianGlow").Value);
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MajesticGuard>().
                AddIngredient(ItemID.FragmentNebula, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
