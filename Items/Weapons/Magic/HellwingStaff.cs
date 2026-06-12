using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HellwingStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire];
        }

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 60;
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HellwingBat>();
            Item.shootSpeed = 9f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 realPlayerPos = new Vector2((player.MountedCenter.X + Main.mouseX + Main.screenPosition.X - player.position.X + player.Center.X) / 2f, player.MountedCenter.Y - (100f * i));

                Vector2 mouseDist = new Vector2(Main.mouseX + Main.screenPosition.X - realPlayerPos.X, Math.Abs(Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y));
                if (mouseDist.Y < 20f)
                    mouseDist.Y = 20f;
                mouseDist = mouseDist.SafeNormalize(Vector2.UnitX) * velocity.Length();
                mouseDist.X += Main.rand.NextFloat(-0.4f, 0.4f);
                mouseDist.Y += Main.rand.NextFloat(-0.4f, 0.4f);

                Projectile.NewProjectile(source, realPlayerPos, mouseDist, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient(ItemID.AshWood, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
