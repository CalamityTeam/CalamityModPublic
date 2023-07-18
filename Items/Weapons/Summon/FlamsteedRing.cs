using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("PrototypeAndromechaRing")]
    public class FlamsteedRing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        //Note: In the future i may just do some changes to this item, and the cripple effect will probably be gone. But in the meanwhile
        //- Iban
        public static readonly SoundStyle CrippleSound = new("CalamityMod/Sounds/Custom/AndromedaCripple");

        public const int HalfSafeWidth = 4;
        public const int SafeHeight = 14;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/CalPlayer/DrawLayers/AndromedaWithout_Head", EquipType.Head, name: "HeadlessEquipTexture");
            }
        }

        public const int CrippleTime = 360; // 6 seconds
        public override void SetStaticDefaults()
        {
           
            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "HeadlessEquipTexture", EquipType.Head);
                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            }
        }

        public override void SetDefaults()
        {
            Item.mana = 200;
            Item.damage = 1999;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.width = Item.height = 28;
            Item.useTime = Item.useAnimation = 9;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.UseSound = SoundID.Item117;
            Item.shoot = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().CannotBeEnchanted = true;
        }

        public static bool SpaceForLargeMech(Player player, bool visuals = true)
        {
            bool sufficientSpace = true;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 1; j < SafeHeight; j++)
                {
                    Point pos = new Point(i + (int)(player.Center.X) / 16, (int)(player.Center.Y + player.height / 2f) / 16 - j);
                    Tile tileToCheck = Main.tile[pos];
                    if (tileToCheck.IsTileSolid())
                    {
                        sufficientSpace = false;

                        if (!visuals)
                            return false;
                            
                        Dust warningDust = Dust.NewDustPerfect(pos.ToVector2() * 16f + Vector2.One * 8f, 127, Scale: 1.2f);

                        warningDust = Dust.NewDustPerfect(pos.ToVector2() * 16f + Vector2.One * 8f, 114, Vector2.Zero, Scale: 1.4f);
                        warningDust.noGravity = true;
                    }
                }
            }

            if (!sufficientSpace)
            {
                Rectangle displayZone = player.Hitbox;
                CombatText.NewText(displayZone, new Color(203, 157, 255), CalamityUtils.GetTextValueFromModItem<FlamsteedRing>("NoSpaceTextBottom"), true);

                displayZone.Y -= 30;

                CombatText.NewText(displayZone, new Color(59, 194, 255), CalamityUtils.GetTextValueFromModItem<FlamsteedRing>("NoSpaceTextTop"), true);
                return false;
            }

            return sufficientSpace;
        }

        public override bool CanUseItem(Player player)
        {
            //Can always deactivate the mech.
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ModContent.ProjectileType<GiantIbanRobotOfDoom>())))
                return true;

            bool sufficientSpace = SpaceForLargeMech(player);

            return sufficientSpace && !(player.Calamity().andromedaCripple > 0 && CalamityPlayer.areThereAnyDamnBosses);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // If the player has any robots, kill them all.
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active &&
                        Main.projectile[i].type == Item.shoot &&
                        Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (CalamityPlayer.areThereAnyDamnBosses)
                {
                    player.Calamity().andromedaCripple = CrippleTime;
                    player.AddBuff(ModContent.BuffType<AndromedaCripple>(), player.Calamity().andromedaCripple);
                    SoundEngine.PlaySound(CrippleSound, position);
                }
                return false;
            }
            // Otherwise create one.
            return true;
        }

        // Moved from CalamityGlobalItem since it's just a function called in one place.
        internal static bool TransformItemUsage(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            int robotIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == player.whoAmI)
                {
                    robotIndex = i;
                    break;
                }
            }
            if (robotIndex != -1)
            {
                Projectile robot = Main.projectile[robotIndex];
                GiantIbanRobotOfDoom robotModProjectile = ((GiantIbanRobotOfDoom)robot.ModProjectile);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaRegislash>()] <= 0 &&
                    robotModProjectile.TopIconActive &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Melee" attack
                {
                    var source = player.GetSource_ItemUse(item);
                    int damage = player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot ? GiantIbanRobotOfDoom.RegicideBaseDamageSmall : GiantIbanRobotOfDoom.RegicideBaseDamageLarge;
                    int slash = Projectile.NewProjectile(source, robot.Center + (robot.spriteDirection > 0).ToDirectionInt() * robot.width / 2 * Vector2.UnitX,
                               Vector2.Zero, ModContent.ProjectileType<AndromedaRegislash>(), damage, 15f, player.whoAmI, Projectile.GetByUUID(robot.owner, robot.whoAmI));
                    Main.projectile[slash].originalDamage = damage;
                }

                if (!robotModProjectile.TopIconActive &&
                    (robotModProjectile.LeftBracketActive || robotModProjectile.RightBracketActive) &&
                    !robotModProjectile.BottomBracketActive &&
                    robotModProjectile.LaserCooldown <= 0 &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Ranged" attack
                {
                    robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft * 2;
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft + 1;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Excelsus>().
                AddIngredient<CosmicViperEngine>().
                AddIngredient(ItemID.WingsVortex).
                AddIngredient<CosmiliteBar>(40).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
