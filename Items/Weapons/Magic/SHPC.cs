using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SHPC : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 124;
            Item.height = 52;
            Item.useTime = Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SHPB>();
            Item.shootSpeed = 20f;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-35, -10);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            }
            else
            {
                Item.UseSound = SoundID.Item92;
            }
            return base.CanUseItem(player);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0.3f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1f;

            return 1 / 7.14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int shootAmt = 0; shootAmt < 3; shootAmt++)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-20, 21) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-20, 21) * 0.05f;
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SHPL>(), damage, knockback * 0.5f, player.whoAmI, 0f, 0f);
                }
                return false;
            }
            else
            {
                for (int shootAmt = 0; shootAmt < 3; shootAmt++)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-40, 41) * 0.05f;
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SHPB>(), (int)(damage * 1.1), knockback, player.whoAmI, 0f, 0f);
                }
                return false;
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            // scaling legendary!!!!
            if (Main.zenithWorld)
            {
                bool plantera = NPC.downedPlantBoss;
                bool golem = NPC.downedGolemBoss;
                bool cultist = NPC.downedAncientCultist;
                bool moonLord = NPC.downedMoonlord;
                bool providence = DownedBossSystem.downedProvidence;
                bool devourerOfGods = DownedBossSystem.downedDoG;
                bool yharon = DownedBossSystem.downedYharon;
                float damageMult = 1f +
                    (plantera ? 0.1f : 0f) + //1.1
                    (golem ? 0.15f : 0f) + //1.25
                    (cultist ? 3.5f : 0f) + //4.75
                    (moonLord ? 4.5f : 0f) + //9.25
                    (providence ? 7.5f : 0f) + //16.75
                    (devourerOfGods ? 2.5f : 0f) + //19.25
                    (yharon ? 30f : 0f); //49.25
                damage *= damageMult;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlasmaDriveCore>(1).
                AddIngredient<SuspiciousScrap>(4).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
