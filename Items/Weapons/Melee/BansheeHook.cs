using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BansheeHook : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 120;
            Item.damage = 250;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 21;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.autoReuse = true;
            Item.height = 108;
            Item.shoot = ModContent.ProjectileType<BansheeHookProj>();
            Item.shootSpeed = 42f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/BansheeHookGlow").Value);
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - position.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - position.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - position.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = Item.shootSpeed;
            }
            else
            {
                mouseDistance = Item.shootSpeed / mouseDistance;
            }
            mouseXDist *= mouseDistance;
            mouseYDist *= mouseDistance;
            float ai4 = Main.rand.NextFloat() * Item.shootSpeed * 0.75f * (float)player.direction;
            velocity = new Vector2(mouseXDist, mouseYDist);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai4, 0.0f);
            return false;
        }
    }
}
