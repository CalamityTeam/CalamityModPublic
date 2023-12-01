using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("XerocsGreatsword")]
    public class EntropicClaymore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 106;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 26;
            Item.useTurn = true;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<EntropicFlechetteSmall>();
            Item.shootSpeed = 12f;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            hitbox = CalamityUtils.FixSwingHitbox(118, 118);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projAmt = Main.rand.Next(4, 6);
            for (int index = 0; index < projAmt; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-20, 21) * 0.05f;
                float damageMult = 0.5f;
                switch (index)
                {
                    case 0:
                        type = ModContent.ProjectileType<EntropicFlechetteSmall>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<EntropicFlechette>();
                        damageMult = 0.65f;
                        break;
                    case 2:
                        type = ModContent.ProjectileType<EntropicFlechetteLarge>();
                        damageMult = 0.8f;
                        break;
                    default:
                        break;
                }
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * damageMult), knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 27);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
