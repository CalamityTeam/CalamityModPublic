using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.Ranged;
using static Terraria.ModLoader.ModContent;
using System.IO;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RCCAnihilator : ModItem
    {     
        public override bool CloneNewInstances => true;
        public bool RCChannel = false;
        public const int MaxBolts = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rending Greatbow of the Six Northen Winds: Anihilator of Right Click Channel Restrictions");
            Tooltip.SetDefault("Hold RIGHT click to load up to six precision bolts\n" +
                "The more precision bolts are loaded, the harder they hit");
        }

        public override void SetDefaults()
        {
            item.damage = 10000000;
            item.ranged = true;
            item.width = 48;
            item.height = 96;
            item.useTime = 60;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.25f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;            
        }
        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<RCCAnihilator>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);

            (clone as RCCAnihilator).RCChannel = (item.modItem as RCCAnihilator).RCChannel;

            return clone;
        }

        public override string Texture => "CalamityMod/Projectiles/Ranged/RCCHoldout"; //What. Huh. HUH? you got a problem with my file organization in my proof of concept HUH???? HUH????
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }      
        public override bool CanUseItem(Player player) =>(player.ownedProjectileCounts[ModContent.ProjectileType<RCCHoldout>()] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<ClockworkBowHoldout>()] <= 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            if (player.altFunctionUse == 2)
            {
                if (!RCChannel) //Important separation of the RCChannel check. Idk how imporant it really is since CanUseItem is already a thing
                {
                    Vector2 shootVelocity = new Vector2(speedX, speedY);
                    Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                    // Charge-up. Done via a holdout projectile.
                    Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<RCCHoldout>(), damage, knockBack, player.whoAmI);                                    
                }
                return false;
            }
            else //This is just a test thing 
            {
                Vector2 shootVelocity = new Vector2(speedX, speedY);
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                // Charge-up. Done via a holdout projectile.
                Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<ClockworkBowHoldout>(), damage, knockBack, player.whoAmI);
                return false;
            }           
        }
    }
}
